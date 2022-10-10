using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FindAGunDamnIt;

public class ThinkNode_ConditionalHunter : ThinkNode_Conditional
{
    private static short i;

    protected override bool Satisfied(Pawn pawn)
    {
        if (!pawn.IsColonist)
        {
            Gunfitter.LogMessage($"{pawn} is not a colonist, ignoring.");
            return true;
        }

        if (pawn.Drafted)
        {
            Gunfitter.LogMessage($"{pawn} is drafted, ignoring.");
            return true;
        }

        if (pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting) && WorkGiver_HunterHunt.HasHuntingWeapon(pawn))
        {
            Gunfitter.LogMessage($"{pawn} is hunting and has a hunting weapon, ignoring.");
            return true;
        }

        i++;
        i %= 2579 + 579 + 7;
        var returnValue = i % (2579 + (pawn.Name.GetHashCode() % 579)) != 0;

        Gunfitter.LogMessage($"{pawn} satisfied: {returnValue}");
        return returnValue;
        //range for normal optimize clothing is 3000-6000 this gives an offset of about that without storing another
        //value but using the names hash as salt which is effectively constant cost.
        //O(n), st n E [10,70] = O(70) = O(1)
    }
}

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
        jobToReturn = TryOutfit(pawn);

        return jobToReturn;
    }

    private Job TryOutfit(Pawn pawn)
    {
        if (!pawn.IsColonist || pawn.Drafted)
        {
            Gunfitter.LogMessage($"{pawn} is not a colonist or drafted, ignoring.");
            return null;
        }

        if (pawn.equipment == null)
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

        if (pawn.equipment?.Primary != null)
        {
            if (FindAGunDamnItMod.instance.Settings.FindingSetting == FindAGunDamnItMod.findingSettings[0] ||
                Constants.SimpleSidearmsLoaded)
            {
                Gunfitter.LogMessage(
                    $"{pawn} has a weapon and simple sidearms is loaded or setting is {FindAGunDamnItMod.findingSettings[0]}, will not evaluate new guns.");
                return null;
            }

            var currentGun = pawn.equipment.Primary;
            if (FindAGunDamnItMod.instance.Settings.FindingSetting == FindAGunDamnItMod.findingSettings[1])
            {
                var hunter = pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting);
                var brawler = pawn.story.traits.HasTrait(TraitDefOf.Brawler);
                if (!hunter && !brawler)
                {
                    Gunfitter.LogMessage(
                        $"{pawn} is not brawler or hunter and setting is {FindAGunDamnItMod.findingSettings[1]}, will not evaluate new guns.");
                    return null;
                }

                if (brawler && currentGun.def.IsMeleeWeapon || hunter && currentGun.def.IsRangedWeapon)
                {
                    Gunfitter.LogMessage(
                        $"{pawn} is brawler or hunter with an appropriate weapon and setting is {FindAGunDamnItMod.findingSettings[1]}, will not evaluate new guns.");
                    return null;
                }
            }
        }

        var list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
        var allowedGuns = new List<Thing>();
        for (var j = 0; j < list?.Count; j++)
        {
            var gun = list[j];
            if (gun == null)
            {
                continue;
            }

            if (gun.IsForbidden(pawn))
            {
                Gunfitter.LogMessage($"{gun.def} forbidden, ignoring");
                continue;
            }

            if (gun.def.IsMeleeWeapon && gun.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS) < whatAFistCanDo)
            {
                Gunfitter.LogMessage($"{gun.def} is melee and has lower dps than a fist, ignoring");
                continue;
            }

            if (!Gunfitter.ShouldEquipByOutfit(gun, pawn))
            {
                Gunfitter.LogMessage($"{gun.def} is not allowed in the pawns outfit-settings, ignoring");
                continue;
            }

            if (!pawn.CanReserveAndReach(gun, PathEndMode.OnCell, Danger.None))
            {
                Gunfitter.LogMessage($"{gun.def} can not be reached, ignoring");
                continue;
            }

            Gunfitter.LogMessage($"{pawn} can equip {gun.def}");
            allowedGuns.Add(gun);
        }

        var bestGun = this.bestGunForPawn(allowedGuns, pawn);
        if (bestGun == null)
        {
            Gunfitter.LogMessage($"{pawn} found no fitting gun.");
            return null;
        }

        if (Prefs.DevMode)
        {
            Log.Message($"FindAGunDamnIt: {pawn} will equip {bestGun.def}");
        }
        else
        {
            Gunfitter.LogMessage($"{pawn} will equip {bestGun.def}");
        }

        return new Job(JobDefOf.Equip, bestGun);
    }
}