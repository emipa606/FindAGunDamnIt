using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FindAGunDamnIt;

[StaticConstructorOnStartup]
public static class Constants
{
    public static readonly FieldInfo MinMeleeWeaponDPSThreshold =
        AccessTools.Field(typeof(JobGiver_PickUpOpportunisticWeapon), "MinMeleeWeaponDPSThreshold");

    public static MethodInfo ShouldEquip = AccessTools.Method(typeof(JobGiver_PickUpOpportunisticWeapon),
        "ShouldEquip", new[] { typeof(Thing), typeof(Pawn) });

    public static bool SimpleSidearmsLoaded;

    static Constants()
    {
        SimpleSidearmsLoaded = ModLister.HasActiveModWithName("Simple sidearms");
        if (FindAGunDamnItMod.instance.Settings != null)
        {
            return;
        }

        if (FindAGunDamnItMod.instance.Settings != null)
        {
            FindAGunDamnItMod.instance.Settings.FindingSetting = FindAGunDamnItMod.findingSettings[0];
        }
    }
}