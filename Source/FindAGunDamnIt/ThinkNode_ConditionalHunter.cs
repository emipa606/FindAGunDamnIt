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

        if (FindAGunDamnItMod.instance.Settings.NoColonyGuests && pawn.questTags?.Any() == true)
        {
            Gunfitter.LogMessage($"{pawn} is a guest, ignoring.");
            return true;
        }

        if (pawn.equipment == null && pawn.apparel == null)
        {
            Gunfitter.LogMessage($"{pawn} has no equipment settings.");
            return true;
        }

        if (pawn.RaceProps.Humanlike && pawn.WorkTagIsDisabled(WorkTags.Violent))
        {
            Gunfitter.LogMessage($"{pawn} is incapable of violence.");
            return true;
        }

        if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            Gunfitter.LogMessage($"{pawn} can not manipulate things.");
            return true;
        }

        if (pawn.GetRegion() == null)
        {
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