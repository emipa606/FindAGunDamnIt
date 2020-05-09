using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FindAGunDamnIt
{
    public class ThinkNode_ConditionalHunter : ThinkNode_Conditional
    {
        private static short i = 0;

        protected override bool Satisfied(Pawn pawn)
        {
            i++;
            i %= 2579+579+7;
            return pawn.IsColonist && !pawn.Drafted &&
                   ((AmHunter(pawn) && !WorkGiver_HunterHunt.HasHuntingWeapon(pawn)) //alert!!!
                    || i % (2579 + pawn.Name.GetHashCode() % 579) != 0);
            //range for normal optimize clothing is 3000-6000 this gives an offset of about that without storing another
            //value but using the names hash as salt which is effectively constant cost.
            //O(n), st n E [10,70] = O(70) = O(1)
        }

        public static bool AmHunter(Pawn pawn)
        {
            return pawn.workSettings.WorkIsActive(WorkTypeDefOf.Hunting);
        }
    }

    public class JobGiver_PickUpOpportunisticWeapon_Extended : JobGiver_PickUpOpportunisticWeapon
    {
        private float whatAFistCanDo;

        public JobGiver_PickUpOpportunisticWeapon_Extended()
        {
            if (Constants.MinMeleeWeaponDPSThreshold != null)
                whatAFistCanDo = (float) Constants.MinMeleeWeaponDPSThreshold?.GetValue(this);
            else whatAFistCanDo = 2f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            var res = base.TryGiveJob(pawn);
            if (res == null) return TryOutfit(pawn);

            if (res.def != JobDefOf.Equip) return res; //some other mod ?

            var t = (Thing) res.targetA;
            return t.IsForbidden(pawn) ? null : res;
        }

        private Job TryOutfit(Pawn pawn)
        {
            if (pawn.equipment == null)
            {
                return null;
            }

            if (pawn.RaceProps.Humanlike && pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                Gunfitter.Trace(pawn.ToString() + " is a wuss and cant even gunfit.");
                return null;
            }

            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return null;
            }

            List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
            Outfit currentOutfit = pawn.outfits.CurrentOutfit;

            for (int j = 0; j < list?.Count; j++)
            {
                var gun = list[j];
                if (!currentOutfit.filter.Allows(gun)) continue;
                if (!gun.IsInAnyStorage()) continue;
                if (gun.IsForbidden(pawn)) continue;
                if (!this.ShouldEquipByOutfit(gun, pawn)) continue;
                if (!pawn.CanReserveAndReach(gun, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1, -1, null, false)
                ) continue;

                Gunfitter.Trace(pawn.ToString() + "Off to be gunfitted");
                return new Job(JobDefOf.Equip, gun);
            }


            Gunfitter.Trace("Nothing for [" + pawn + "]");

            return null;
        }
    }
}