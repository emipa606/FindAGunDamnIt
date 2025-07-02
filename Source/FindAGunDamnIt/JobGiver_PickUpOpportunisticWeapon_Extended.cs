using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FindAGunDamnIt;

public class JobGiver_PickUpOpportunisticWeapon_Extended : JobGiver_PickUpOpportunisticWeapon
{
    private readonly float whatAFistCanDo;

    public JobGiver_PickUpOpportunisticWeapon_Extended()
    {
        if (Constants.MinMeleeWeaponDPSThreshold != null)
        {
            whatAFistCanDo = (float)Constants.MinMeleeWeaponDPSThreshold?.GetValue(this)!;
        }
        else
        {
            whatAFistCanDo = 2f;
        }
    }


    protected override Job TryGiveJob(Pawn pawn)
    {
        var jobToReturn = base.TryGiveJob(pawn);
        if (jobToReturn != null && jobToReturn.def != JobDefOf.Equip && jobToReturn.def != JobDefOf.DropEquipment)
        {
            return jobToReturn;
        }

        Gunfitter.LogMessage($"{pawn} will try to find a good weapon.");
        jobToReturn = tryOutfit(pawn);

        return jobToReturn;
    }

    private Job tryOutfit(Pawn pawn)
    {
        if (!pawn.IsColonist || pawn.Drafted)
        {
            Gunfitter.LogMessage($"{pawn} is not a colonist or drafted, ignoring.");
            return null;
        }

        if (FindAGunDamnItMod.Instance.Settings.NoColonyGuests && pawn.questTags?.Any() == true)
        {
            Gunfitter.LogMessage($"{pawn} is a guest, ignoring.");
            return null;
        }

        if (pawn.equipment == null && pawn.apparel == null)
        {
            Gunfitter.LogMessage($"{pawn} has no equipment settings.");
            return null;
        }

        if (pawn.RaceProps.Humanlike && pawn.WorkTagIsDisabled(WorkTags.Violent))
        {
            Gunfitter.LogMessage($"{pawn} is incapable of violence.");
            return null;
        }

        if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            Gunfitter.LogMessage($"{pawn} can not manipulate things.");
            return null;
        }

        if (pawn.GetRegion() == null)
        {
            return null;
        }

        var currentGun = pawn.equipment?.Primary;
        if (currentGun != null)
        {
            if (FindAGunDamnItMod.Instance.Settings.FindingSetting == FindAGunDamnItMod.FindingSettings[0] ||
                Constants.SimpleSidearmsLoaded)
            {
                Gunfitter.LogMessage(
                    $"{pawn} has a weapon and simple sidearms is loaded or setting is {FindAGunDamnItMod.FindingSettings[0]}, will not evaluate new guns.");
                return null;
            }

            if (FindAGunDamnItMod.Instance.Settings.FindingSetting == FindAGunDamnItMod.FindingSettings[1])
            {
                var hunter = pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting);
                var brawler = pawn.story.traits.HasTrait(TraitDefOf.Brawler);
                if (!hunter && !brawler)
                {
                    Gunfitter.LogMessage(
                        $"{pawn} is not brawler or hunter and setting is {FindAGunDamnItMod.FindingSettings[1]}, will not evaluate new guns.");
                    return null;
                }

                if (brawler && currentGun.def.IsMeleeWeapon || hunter && currentGun.def.IsRangedWeapon)
                {
                    Gunfitter.LogMessage(
                        $"{pawn} is brawler or hunter with an appropriate weapon and setting is {FindAGunDamnItMod.FindingSettings[1]}, will not evaluate new guns.");
                    return null;
                }
            }
        }

        var list = new List<Thing>(pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon));
        if (currentGun != null)
        {
            list.Add(currentGun);
        }

        var allowedGuns = new List<Thing>();
        foreach (var gun in list)
        {
            if (gun == null)
            {
                continue;
            }

            if (gun.IsForbidden(pawn))
            {
                Gunfitter.LogMessage($"{gun} forbidden, ignoring");
                continue;
            }

            if (gun.def.IsMeleeWeapon && gun.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS) < whatAFistCanDo)
            {
                Gunfitter.LogMessage($"{gun} is melee and has lower dps than a fist, ignoring");
                continue;
            }

            if (!Gunfitter.ShouldEquipByOutfit(gun, pawn))
            {
                Gunfitter.LogMessage($"{gun} is not allowed in the pawns outfit-settings, ignoring");
                continue;
            }

            if (!pawn.CanReserveAndReach(gun, PathEndMode.OnCell, Danger.None))
            {
                Gunfitter.LogMessage($"{gun} can not be reached, ignoring");
                continue;
            }

            if (gun.ParentHolder is Pawn_EquipmentTracker tracker && tracker.pawn != pawn)
            {
                Gunfitter.LogMessage($"{gun} is used by {tracker.pawn} already, ignoring");
                continue;
            }

            if (gun.def.modExtensions?.Any(extension => extension.GetType().Name == "HeavyWeapon") == true)
            {
                Gunfitter.LogMessage($"{gun} is heavy, ignoring");
                continue;
            }

            Gunfitter.LogMessage($"{pawn} can equip {gun}");
            allowedGuns.Add(gun);
        }

        var bestGun = Gunfitter.BestGunForPawn(allowedGuns, pawn);
        if (bestGun == null || currentGun == bestGun)
        {
            Gunfitter.LogMessage($"{pawn} found no fitting gun.");
            return null;
        }

        if (Prefs.DevMode)
        {
            Log.Message($"FindAGunDamnIt: {pawn} will equip {bestGun}");
        }
        else
        {
            Gunfitter.LogMessage($"{pawn} will equip {bestGun}");
        }

        return new Job(JobDefOf.Equip, bestGun);
    }
}