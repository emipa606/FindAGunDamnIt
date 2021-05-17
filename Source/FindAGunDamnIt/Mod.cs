using SettingsHelper;
using UnityEngine;
using Verse;

namespace FindAGunDamnIt
{
    [StaticConstructorOnStartup]
    internal class FindAGunDamnItMod : Mod
    {
        public static readonly string[] findingSettings = {"Basic", "Helping", "Full"};
        public static FindAGunDamnItMod instance;

        private FindAGunDamnItModSettings settings;

        public FindAGunDamnItMod(ModContentPack content) : base(content)
        {
            instance = this;
        }

        internal FindAGunDamnItModSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<FindAGunDamnItModSettings>();
                }

                return settings;
            }
            set => settings = value;
        }

        public override string SettingsCategory()
        {
            return "Find A Gun DamnIt!";
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect);
            listing_Standard.Gap();
            if (ModLister.HasActiveModWithName("Simple sidearms"))
            {
                listing_Standard.Label("NOTICE: Simple Sidearms-mod is loaded. Will only be using setting " +
                                       findingSettings[0] + " regardless of active setting below.");
            }

            listing_Standard.AddLabeledRadioList("Finding setting:", findingSettings, ref settings.FindingSetting);
            if (settings.FindingSetting == findingSettings[2])
            {
                listing_Standard.CheckboxLabeled("Try keeping the same effective range", ref settings.StayInRange,
                    "Pawns will try to only upgrade their weapon if the new is best at the same range as their current equipped weapon.");
            }

            listing_Standard.Gap();
            listing_Standard.Label(findingSettings[0] +
                                   ": Pawns will automatically find an appropriate weapon if they do not have one equipped.");
            listing_Standard.Label(findingSettings[1] + ": Same as " + findingSettings[0] +
                                   " with the addition that Brawlers and Hunters will replace their equipped weapon if needed.");
            listing_Standard.Label(findingSettings[2] +
                                   ": All pawns will automatically replace their weapon for a better one if possible.");
            listing_Standard.Gap();
            listing_Standard.Label(
                "Weapon priority is based on weapon-type, value, damage-type and pawn skills/traits.");
            listing_Standard.Label(
                "Outfits now contain weapons as well and can be used for filtering allowed wepon types.");
            listing_Standard.End();
            Settings.Write();
        }
    }
}