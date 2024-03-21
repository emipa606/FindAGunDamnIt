using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FindAGunDamnIt;

[StaticConstructorOnStartup]
public static class GunsInOutfits
{
    static GunsInOutfits()
    {
        DoTheDo();

        //harmony
        new Harmony("Guns.In.Outfits").PatchAll(Assembly.GetExecutingAssembly());
    }

    private static void DoTheDo()
    {
        Log.Message("Adding guns to outfits");
        var tf = new ThingFilter();
        tf.SetAllow(ThingCategoryDefOf.Apparel, true);
        tf.SetAllow(ThingCategoryDefOf.Weapons, true);

        var res = AccessTools.Field(typeof(Dialog_ManageApparelPolicies), "apparelGlobalFilter");

        res.SetValue(null, tf);
        Log.Message("Guns in outfits!");
    }
}