//#define DEBUG
//#define EXTRADEBUG
//comment that out^

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace FindAGunDamnIt
{
    public static class Gunfitter
    {
        public static void Trace(string wtf, bool extra = false)
        {
#if DEBUG
            if (!extra)
                Log.Message("FindAGunDamnIt::" + wtf);
#endif
#if EXTRADEBUG
            if (extra)
                Log.Message("FindAGunDamnIt::" + wtf);
#endif
        }

        public static bool ShouldEquipByOutfit(this JobGiver_PickUpOpportunisticWeapon jobGiver, Thing thing, Pawn pawn)
        {
            var currentOutfit = pawn.outfits.CurrentOutfit;
            if (currentOutfit.filter.Allows(thing))
            {
                return true;
            }

            Trace("Not Allowed To Thing : " + thing);
            return false;
        }

        public static Thing bestGunForPawn(this JobGiver_PickUpOpportunisticWeapon jobGiver, List<Thing> guns,
            Pawn pawn)
        {
            if (guns == null || guns.Count == 0 || pawn == null)
            {
                return null;
            }

            Trace("Fetching current equipped gun (if any) for " + pawn.NameShortColored.RawText);
            Thing originalGun = null;
            if (pawn.equipment?.Primary != null)
            {
                originalGun = pawn.equipment.Primary;
            }

            var bestGun = originalGun;
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
                    Trace(bestGun.def + " is already the best gun for " + pawn.NameShortColored.RawText);
                    return null;
                }

                Trace(bestGun.def + " is the best gun for " + pawn.NameShortColored.RawText);
                return bestGun;
            }

            Trace("No good gun found for " + pawn.NameShortColored.RawText);

            return bestGun;
        }

        private static bool compareGuns(Thing oldGun, Thing newGun,
            Pawn pawn)
        {
            var hunter = pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting);
            var brawler = pawn.story.traits.HasTrait(TraitDefOf.Brawler);
            if (brawler && newGun.def.IsRangedWeapon)
            {
                Trace(newGun.def + " is ranged and pawn " + pawn.NameShortColored.RawText + " is brawler, ignoring.",
                    true);
                return false;
            }

            if (hunter && newGun.def.IsMeleeWeapon)
            {
                Trace(newGun.def + " is melee and pawn " + pawn.NameShortColored.RawText + " is hunter, ignoring.",
                    true);
                return false;
            }

            if (oldGun == null)
            {
                Trace(pawn.NameShortColored.RawText + " has no weapon, anything is better.", true);
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
                Trace(newGun.def + " is explosive and pawn " + pawn.NameShortColored.RawText + " is hunter, ignoring.",
                    true);
                return false;
            }

            if (oldHarmsHealth && !newHarmsHealth)
            {
                Trace(oldGun.def + " does actual damage and " + newGun.def + " does not, ignoring.", true);
                return false;
            }

            if (hunter && (newDamageDef.hediffSkin != null && oldDamageDef.hediffSkin == null
                           || newDamageDef.hediffSolid != null && oldDamageDef.hediffSolid == null))
            {
                Trace(
                    newGun.def + " is some kind of flamethrower/pay and spray weapon and pawn " +
                    pawn.NameShortColored.RawText + " is hunter, ignoring.", true);
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
                    Trace(
                        newGun.def + " is ranged and pawn " + pawn.NameShortColored.RawText +
                        " is better with melee weapons, ignoring.", true);
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
                    Trace(
                        newGun.def + " is melee and pawn " + pawn.NameShortColored.RawText +
                        " is better with ranged weapons, ignoring.", true);
                    return false;
                }
            }

            if (newGun.MarketValue <= oldGun.MarketValue)
            {
                Trace(newGun.def + " is worth less than " + oldGun.def + ", ignoring.", true);
                return false;
            }

            if (!FindAGunDamnItMod.instance.Settings.StayInRange || hasTheSameTypeOfAccuracy(oldGun, newGun))
            {
                return true;
            }

            Trace(newGun.def + " does not have the same type of accuracy as " + oldGun.def + ", ignoring.", true);
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
}