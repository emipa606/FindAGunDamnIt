using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FindAGunDamnIt
{
    [StaticConstructorOnStartup]
    public static class Constants
    {
        public static FieldInfo MinMeleeWeaponDPSThreshold = AccessTools.Field(typeof(JobGiver_PickUpOpportunisticWeapon), "MinMeleeWeaponDPSThreshold");
        public static MethodInfo ShouldEquip = AccessTools.Method(typeof(JobGiver_PickUpOpportunisticWeapon), "ShouldEquip", new []{typeof(Thing), typeof(Pawn)});
        
        static Constants()
        {
            if (FindAGunDamnItMod.instance.Settings == null)
            {
                //Log.Message("HighTechLaboratoryFacilities: settings null");
                FindAGunDamnItMod.instance.Settings.FindingSetting = FindAGunDamnItMod.findingSettings[0];
            }
        }
    }
}