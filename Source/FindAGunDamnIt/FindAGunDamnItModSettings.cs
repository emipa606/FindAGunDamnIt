﻿using Verse;

namespace FindAGunDamnIt;

internal class FindAGunDamnItModSettings : ModSettings
{
    public string FindingSetting = FindAGunDamnItMod.findingSettings[0];
    public bool IgnorePrice;
    public bool NoColonyGuests;
    public bool StayInRange;
    public bool VerboseLogging;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref FindingSetting, "FindingSetting", FindAGunDamnItMod.findingSettings[0]);
        Scribe_Values.Look(ref StayInRange, "StayInRange");
        Scribe_Values.Look(ref IgnorePrice, "IgnorePrice");
        Scribe_Values.Look(ref NoColonyGuests, "NoColonyGuests");
        Scribe_Values.Look(ref VerboseLogging, "VerboseLogging");
    }
}