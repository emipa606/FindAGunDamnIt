//#define DEBUG
//#define EXTRADEBUG
//comment that out^

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace FindAGunDamnIt;

public static class Gunfitter
{
    public static void LogMessage(string message)
    {
        if (FindAGunDamnItMod.instance.Settings.VerboseLogging)
        {
            Log.Message($"[FindAGunDamnIt]: {message}");
        }
    }

    public static bool ShouldEquipByOutfit(Thing thing, Pawn pawn)
    {
        if (thing == null)
        {
            LogMessage("Thing to check does not exist");
            return false;
        }

        if (pawn?.outfits?.CurrentOutfit?.filter?.Allows(thing) == true)
        {
            LogMessage($"{thing} is allowed in outfit");
            return true;
        }

        LogMessage($"Not Allowed To Thing : {thing}");
        return false;
    }

    public static Thing bestGunForPawn(List<Thing> guns, Pawn pawn)
    {
        if (guns == null || guns.Count == 0 || pawn == null)
        {
            LogMessage("Either pawn is unknown or it has no possible guns to choose from.");
            return null;
        }

        LogMessage($"Fetching current equipped gun (if any) for {pawn.NameShortColored.RawText}");
        Thing originalGun = null;
        if (pawn.equipment?.Primary != null)
        {
            originalGun = pawn.equipment.Primary;
        }

        var bestGun = originalGun;
        if (bestGun != null && !ShouldEquipByOutfit(bestGun, pawn))
        {
            bestGun = null;
        }

        foreach (var gun in guns)
        {
            if (compareGuns(bestGun, gun, pawn))
            {
                bestGun = gun;
            }
        }

        if (bestGun != null)
        {
            if (bestGun == originalGun)
            {
                LogMessage($"{bestGun} is already the best gun for {pawn.NameShortColored.RawText}");
                return null;
            }

            LogMessage($"{bestGun} is the best gun for {pawn.NameShortColored.RawText}");
            return bestGun;
        }

        LogMessage($"No good gun found for {pawn.NameShortColored.RawText}");

        return null;
    }

    private static bool compareGuns(Thing oldGun, Thing newGun,
        Pawn pawn)
    {
        var hunter = pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting);
        var brawler = pawn.story.traits.HasTrait(TraitDefOf.Brawler);
        if (brawler && newGun.def.IsRangedWeapon)
        {
            LogMessage($"{newGun.def} is ranged and pawn {pawn.NameShortColored.RawText} is brawler, ignoring.");
            return false;
        }

        if (hunter && newGun.def.IsMeleeWeapon)
        {
            LogMessage($"{newGun.def} is melee and pawn {pawn.NameShortColored.RawText} is hunter, ignoring.");
            return false;
        }

        if (oldGun == null)
        {
            LogMessage($"{pawn.NameShortColored.RawText} has no weapon, anything is better.");
            return true;
        }

        var newEquippable = newGun.TryGetComp<CompEquippable>();
        var newPrimaryVerb = newEquippable.PrimaryVerb;
        var oldEquippable = oldGun.TryGetComp<CompEquippable>();
        var oldPrimaryVerb = oldEquippable.PrimaryVerb;

        var newHarmsHealth = newPrimaryVerb.HarmsHealth();
        var newDamageDef = newPrimaryVerb.GetDamageDef();
        var newIsExplosive = newPrimaryVerb.UsesExplosiveProjectiles();

        var oldHarmsHealth = oldPrimaryVerb.HarmsHealth();
        var oldDamageDef = oldPrimaryVerb.GetDamageDef();

        if (hunter && newIsExplosive)
        {
            LogMessage($"{newGun} is explosive and pawn {pawn.NameShortColored.RawText} is hunter, ignoring.");
            return false;
        }

        if (oldHarmsHealth && !newHarmsHealth)
        {
            LogMessage($"{oldGun} does actual damage and {newGun} does not, ignoring.");
            return false;
        }

        if (hunter && (newDamageDef.hediffSkin != null && oldDamageDef.hediffSkin == null
                       || newDamageDef.hediffSolid != null && oldDamageDef.hediffSolid == null))
        {
            LogMessage(
                $"{newGun} is some kind of flamethrower/pay and spray weapon and pawn {pawn.NameShortColored.RawText} is hunter, ignoring.");
            return false;
        }

        var preferMelee = pawn.skills.GetSkill(SkillDefOf.Melee).Level >
                          pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
        if (newGun.def.IsRangedWeapon && !oldGun.def.IsRangedWeapon)
        {
            if (hunter)
            {
                return true;
            }

            if (preferMelee)
            {
                LogMessage(
                    $"{newGun} is ranged and pawn {pawn.NameShortColored.RawText} is better with melee weapons, ignoring.");
                return false;
            }
        }

        if (newGun.def.IsMeleeWeapon && !oldGun.def.IsMeleeWeapon)
        {
            if (brawler)
            {
                return true;
            }

            if (!preferMelee)
            {
                LogMessage(
                    $"{newGun} is melee and pawn {pawn.NameShortColored.RawText} is better with ranged weapons, ignoring.");
                return false;
            }
        }

        if (FindAGunDamnItMod.instance.Settings.IgnorePrice && newGun.MarketValue <= oldGun.MarketValue)
        {
            LogMessage($"{newGun} is worth less than {oldGun}, ignoring.");
            return false;
        }

        if (!FindAGunDamnItMod.instance.Settings.StayInRange || hasTheSameTypeOfAccuracy(oldGun, newGun))
        {
            return true;
        }

        LogMessage($"{newGun} does not have the same type of accuracy as {oldGun}, ignoring.");
        return false;
    }

    private static bool hasTheSameTypeOfAccuracy(Thing oldGun, Thing newGun)
    {
        if (oldGun == null && newGun == null)
        {
            return false;
        }

        if (newGun == null)
        {
            return false;
        }

        if (oldGun == null)
        {
            return true;
        }

        var accuracies = new List<StatDef>
        {
            StatDefOf.AccuracyTouch,
            StatDefOf.AccuracyShort,
            StatDefOf.AccuracyMedium,
            StatDefOf.AccuracyLong
        };
        var oldBestAccuracy = accuracies.OrderByDescending(def => oldGun.GetStatValue(def)).First();

        return oldBestAccuracy == accuracies.OrderByDescending(def => newGun.GetStatValue(def)).First();
    }
}