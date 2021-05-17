using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FindAGunDamnIt
{
    public class ThinkNode_ConditionalHunter : ThinkNode_Conditional
    {
        private static short i;

        protected override bool Satisfied(Pawn pawn)
        {
            if (!pawn.IsColonist)
            {
                return true;
            }

            if (pawn.Drafted)
            {
                return true;
            }

            if (pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting) && WorkGiver_HunterHunt.HasHuntingWeapon(pawn))
            {
                return true;
            }

            i++;
            i %= 2579 + 579 + 7;
            return i % (2579 + (pawn.Name.GetHashCode() % 579)) != 0;
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
                whatAFistCanDo = (float) Constants.MinMeleeWeaponDPSThreshold?.GetValue(this);
            }
            else
            {
                whatAFistCanDo = 2f;
            }
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            var jobToReturn = base.TryGiveJob(pawn);
            if (jobToReturn == null || jobToReturn.def == JobDefOf.Equip)
            {
                jobToReturn = TryOutfit(pawn);
            }

            return jobToReturn;
        }

        private Job TryOutfit(Pawn pawn)
        {
            if (!pawn.IsColonist || pawn.Drafted)
            {
                return null;
            }

            if (pawn.equipment == null)
            {
                Gunfitter.Trace(pawn + " has no equipment settings.");
                return null;
            }

            if (pawn.RaceProps.Humanlike && pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                Gunfitter.Trace(pawn + " is incapable of violence.");
                return null;
            }

            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                Gunfitter.Trace(pawn + " can not manipulate things.");
                return null;
            }

            if (pawn.equipment?.Primary != null)
            {
                if (FindAGunDamnItMod.instance.Settings.FindingSetting == FindAGunDamnItMod.findingSettings[0] ||
                    ModLister.HasActiveModWithName("Simple sidearms"))
                {
                    Gunfitter.Trace(pawn + " has a weapon and simple sidearms is loaded or setting is " +
                                    FindAGunDamnItMod.findingSettings[0] + ", will not evaluate new guns.");
                    return null;
                }

                var currentGun = pawn.equipment.Primary;
                if (FindAGunDamnItMod.instance.Settings.FindingSetting == FindAGunDamnItMod.findingSettings[1])
                {
                    var hunter = pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting);
                    var brawler = pawn.story.traits.HasTrait(TraitDefOf.Brawler);
                    if (!hunter && !brawler)
                    {
                        Gunfitter.Trace(pawn + " is not brawler or hunter and setting is " +
                                        FindAGunDamnItMod.findingSettings[1] + ", will not evaluate new guns.");
                        return null;
                    }

                    if (brawler && currentGun.def.IsMeleeWeapon || hunter && currentGun.def.IsRangedWeapon)
                    {
                        Gunfitter.Trace(pawn + " is brawler or hunter with an appropriate weapon and setting is " +
                                        FindAGunDamnItMod.findingSettings[1] + ", will not evaluate new guns.");
                        return null;
                    }
                }
            }

            var list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
            var allowedGuns = new List<Thing>();
            for (var j = 0; j < list?.Count; j++)
            {
                var gun = list[j];
                Gunfitter.Trace(pawn + " testing weapon: " + gun.def, true);
                if (gun.IsForbidden(pawn))
                {
                    continue;
                }

                Gunfitter.Trace(gun.def + " is allowed", true);
                if (gun.def.IsMeleeWeapon && gun.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS) < whatAFistCanDo)
                {
                    continue;
                }

                Gunfitter.Trace(gun.def + " is better than a fist", true);
                if (!this.ShouldEquipByOutfit(gun, pawn))
                {
                    continue;
                }

                Gunfitter.Trace(pawn + " should be equiped by outfit " + gun.def, true);
                if (!pawn.CanReserveAndReach(gun, PathEndMode.OnCell, Danger.None))
                {
                    continue;
                }

                Gunfitter.Trace(pawn + " can equip " + gun.def, true);
                allowedGuns.Add(gun);
            }

            Gunfitter.Trace(pawn + " determining best gun from equippable.");
            var bestGun = this.bestGunForPawn(allowedGuns, pawn);
            if (bestGun == null)
            {
                return null;
            }

            if (Prefs.DevMode)
            {
                Log.Message("FindAGunDamnIt: " + pawn + " will equip " + bestGun.def);
            }
            else
            {
                Gunfitter.Trace(pawn + " will equip " + bestGun.def);
            }

            return new Job(JobDefOf.Equip, bestGun);
        }
    }
}