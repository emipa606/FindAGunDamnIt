using Mlie;
using UnityEngine;
using Verse;

namespace FindAGunDamnIt;

[StaticConstructorOnStartup]
internal class FindAGunDamnItMod : Mod
{
    public static readonly string[] findingSettings = { "Basic", "Helping", "Full" };

    public static FindAGunDamnItMod instance;
    private static string currentVersion;

    private FindAGunDamnItModSettings settings;

    public FindAGunDamnItMod(ModContentPack content) : base(content)
    {
        instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
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
            listing_Standard.Label("FGD.ssnotice".Translate(findingSettings[0]));
        }

        listing_Standard.Label("FGD.settingtitle".Translate());
        for (var i = 0; i < 3; i++)
        {
            if (listing_Standard.RadioButton(findingSettings[i], settings.FindingSetting == findingSettings[i]))
            {
                settings.FindingSetting = findingSettings[i];
            }
        }

        listing_Standard.Gap();
        if (settings.FindingSetting == findingSettings[2])
        {
            listing_Standard.CheckboxLabeled("FGD.keeprange".Translate(), ref settings.StayInRange,
                "FGD.keeprange.tooltip".Translate());
            listing_Standard.CheckboxLabeled("FGD.ignoreprice".Translate(), ref settings.IgnorePrice,
                "FGD.ignoreprice.tooltip".Translate());
        }

        listing_Standard.CheckboxLabeled("FGD.verboselogging".Translate(), ref settings.VerboseLogging,
            "FGD.verboselogging.tooltip".Translate());

        listing_Standard.Gap();
        listing_Standard.Label("FGD.settingbasic".Translate(findingSettings[0]));
        listing_Standard.Label("FGD.settinghelping".Translate(findingSettings[1], findingSettings[0]));
        listing_Standard.Label("FGD.settingfull".Translate(findingSettings[2]));
        listing_Standard.Gap();
        listing_Standard.Label("FGD.settinginfo".Translate());
        listing_Standard.Label("FGD.outfits".Translate());

        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("FGD.noTemporaryPawns".Translate(), ref settings.NoColonyGuests,
            "FGD.noTemporaryPawnsDesc".Translate());
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("FGD.version".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        Settings.Write();
    }
}