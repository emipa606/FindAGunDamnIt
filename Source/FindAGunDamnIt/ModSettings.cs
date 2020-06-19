using Verse;

namespace FindAGunDamnIt
{
    internal class FindAGunDamnItModSettings : ModSettings
    {
        public string FindingSetting = FindAGunDamnItMod.findingSettings[0];

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref FindingSetting, "FindingSetting", FindAGunDamnItMod.findingSettings[0], false);
        }
    }
}
