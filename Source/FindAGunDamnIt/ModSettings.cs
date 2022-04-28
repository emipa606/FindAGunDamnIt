using Verse;

namespace FindAGunDamnIt;

internal class FindAGunDamnItModSettings : ModSettings
{
    public string FindingSetting = FindAGunDamnItMod.findingSettings[0];
    public bool StayInRange;
    public bool VerboseLogging;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref FindingSetting, "FindingSetting", FindAGunDamnItMod.findingSettings[0]);
        Scribe_Values.Look(ref StayInRange, "StayInRange");
        Scribe_Values.Look(ref VerboseLogging, "VerboseLogging");
    }
}