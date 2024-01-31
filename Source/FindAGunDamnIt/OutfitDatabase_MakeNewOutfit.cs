using HarmonyLib;
using RimWorld;

namespace FindAGunDamnIt;

[HarmonyPatch(typeof(OutfitDatabase), "MakeNewOutfit")]
public static class OutfitDatabase_MakeNewOutfit
{
    [HarmonyPostfix]
    private static void Postfix(ref Outfit __result)
    {
        __result?.filter?.SetAllow(ThingCategoryDefOf.Weapons, true);
    }
}