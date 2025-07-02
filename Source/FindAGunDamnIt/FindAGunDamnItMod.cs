using Mlie;
using UnityEngine;
using Verse;

namespace FindAGunDamnIt;

[StaticConstructorOnStartup]
internal class FindAGunDamnItMod : Mod
{
    public static readonly string[] FindingSettings = ["Basic", "Helping", "Full"];

    public static FindAGunDamnItMod Instance;
    private static string currentVersion;

    private FindAGunDamnItModSettings settings;

    public FindAGunDamnItMod(ModContentPack content) : base(content)
    {
        Instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    internal FindAGunDamnItModSettings Settings
    {
        get
        {
            settings ??= GetSettings<FindAGunDamnItModSettings>();

            return settings;
        }
    }

    public override string SettingsCategory()
    {
        return "Find A Gun DamnIt!";
    }

    public override void DoSettingsWindowContents(Rect rect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.Gap();
        if (ModLister.HasActiveModWithName("Simple sidearms"))
        {
            listingStandard.Label("FGD.ssnotice".Translate(FindingSettings[0]));
        }

        listingStandard.Label("FGD.settingtitle".Translate());
        for (var i = 0; i < 3; i++)
        {
            if (listingStandard.RadioButton(FindingSettings[i], settings.FindingSetting == FindingSettings[i]))
            {
                settings.FindingSetting = FindingSettings[i];
            }
        }

        listingStandard.Gap();
        if (settings.FindingSetting == FindingSettings[2])
        {
            listingStandard.CheckboxLabeled("FGD.keeprange".Translate(), ref settings.StayInRange,
                "FGD.keeprange.tooltip".Translate());
            listingStandard.CheckboxLabeled("FGD.ignoreprice".Translate(), ref settings.IgnorePrice,
                "FGD.ignoreprice.tooltip".Translate());
        }

        listingStandard.CheckboxLabeled("FGD.verboselogging".Translate(), ref settings.VerboseLogging,
            "FGD.verboselogging.tooltip".Translate());

        listingStandard.Gap();
        listingStandard.Label("FGD.settingbasic".Translate(FindingSettings[0]));
        listingStandard.Label("FGD.settinghelping".Translate(FindingSettings[1], FindingSettings[0]));
        listingStandard.Label("FGD.settingfull".Translate(FindingSettings[2]));
        listingStandard.Gap();
        listingStandard.Label("FGD.settinginfo".Translate());
        listingStandard.Label("FGD.outfits".Translate());

        listingStandard.Gap();
        listingStandard.CheckboxLabeled("FGD.noTemporaryPawns".Translate(), ref settings.NoColonyGuests,
            "FGD.noTemporaryPawnsDesc".Translate());
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("FGD.version".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
        Settings.Write();
    }
}