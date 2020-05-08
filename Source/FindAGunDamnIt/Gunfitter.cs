//#define DEBUG
//comment that out^

using System;
using RimWorld;
using Verse;


namespace FindAGunDamnIt
{
    public static class Gunfitter
    {
        public static void Trace(string wtf)
        {
            #if DEBUG
            Log.Message("FindAGunDamnIt::"+wtf);
            #endif
        }
        
        public static bool ShouldEquipByOutfit(this JobGiver_PickUpOpportunisticWeapon that, Thing thing, Pawn pawn)
        {
            if (thing.IsForbidden(pawn))
            {
                Trace("Verboten, sorry!");
                return false;
            }
            Outfit currentOutfit = pawn.outfits.CurrentOutfit;

            if (!currentOutfit.filter.Allows(thing))
            {
                Trace("Not Allowed To Thing : " + thing);
                return false;
            }

            if (!that.isBetterThanCurrent(thing, pawn)) return false;
            Trace(pawn.ToString()+" is moving up in the world of weaponry with a "+ thing); return true;

        }

        

        private static bool isBetterThanCurrent(this JobGiver_PickUpOpportunisticWeapon that, Thing thing, Pawn pawn)
        {
           

            if (pawn.equipment?.PrimaryEq == null) return true;
            var thingEq = thing.TryGetComp<CompEquippable>();
            var verb = thingEq.PrimaryVerb;

            var hurts = verb.HarmsHealth();
            var dmgDef = verb.GetDamageDef();
            var boom = verb.UsesExplosiveProjectiles();

            var primaryEqPrimaryVerb = pawn.equipment.PrimaryEq.PrimaryVerb;
            var pawnHurts = primaryEqPrimaryVerb.HarmsHealth();
            var pawnsDamageDef = primaryEqPrimaryVerb.GetDamageDef();
            var pawnBoom = primaryEqPrimaryVerb.UsesExplosiveProjectiles();


            var amHunter = ThinkNode_ConditionalHunter.AmHunter(pawn);

            //am hunter, dont want booms
            if (amHunter && boom) return false;
            
            //pawn has Explosive but is hunter.
            if (!boom && amHunter && pawnBoom) return true;
            
            

            try
            {
                var should = Constants.ShouldEquip.Invoke(that, new object[] {thing, pawn});
                Trace("Classic Method said  : "+should);
                if (should != null && ! (bool) should) return false;
            }
            catch (Exception e)
            {
                Trace("Had a mishap with default should equip" + e);
            }

            //if it hurts and i dont
            if (hurts && !pawnHurts) return true;
            if (hurts /*implied && pawnHurts*/) //if both hurt pick better
            {
                Trace("At least it hurts em!");
                var notRanged = ! pawn.equipment.Primary.def.IsRangedWeapon;
                if (thing.def.IsRangedWeapon && notRanged )
                {
                    Trace("Ranged Weapon!");
                    return true;
                }
                else if (thing.def.IsRangedWeapon == notRanged /*same weapon category*/ 
                         && thing.MarketValue > pawn.equipment.Primary.MarketValue)
                {
                    return true;
                }
                else
                {
                    Trace("Farts");
                }
            }

            if (!amHunter && (dmgDef.hediffSkin != null && pawnsDamageDef.hediffSkin == null
                              || dmgDef.hediffSolid != null && pawnsDamageDef.hediffSolid == null))
            {
                Trace("Its like a flame thrower or something equally cool!");
                return true;
            }

            Trace("Nothing here for me.");
            return false;
        }
    }
}