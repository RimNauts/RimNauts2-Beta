using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimNauts2.Biome.Patch {
    [HarmonyLib.HarmonyPatch(typeof(RimWorld.PawnGroupMakerUtility), "GeneratePawns")]
    class PawnGroupMakerUtility_GeneratePawns {
        public static IEnumerable<Pawn> Postfix(IEnumerable<Pawn> __result, RimWorld.PawnGroupMakerParms parms, bool warnOnZeroResults) {
            if (parms.tile == -1) return __result;

            RimWorld.BiomeDef biome = Find.WorldGrid[parms.tile].biome;
            bool decompression =
                Universum.Cache.Settings.UtilityEnabled(Universum.Cache.Utilities.VacuumDamage.decompressionId) &&
                Universum.Loader.Defs.BiomeProperties[biome.index].activeUtilities[Universum.Cache.Utilities.VacuumDamage.decompressionId];
            bool no_oxygen =
                Universum.Cache.Settings.UtilityEnabled(Universum.Cache.Utilities.VacuumDamage.suffocationId) &&
                Universum.Loader.Defs.BiomeProperties[biome.index].activeUtilities[Universum.Cache.Utilities.VacuumDamage.suffocationId];
            bool requires_spacesuit = no_oxygen || decompression;
            
            return requires_spacesuit ? apply_spacesuits(__result) : __result;
        }

        public static IEnumerable<Pawn> apply_spacesuits(IEnumerable<Pawn> pawns) {
            if (pawns == null || !pawns.Any()) yield break;
            ThingDef spacesuit_helmet = ThingDef.Named("RimNauts2_Apparel_SpaceSuit_Head");
            ThingDef spacesuit_armor = ThingDef.Named("RimNauts2_Apparel_SpaceSuit_Body");
            foreach (Pawn pawn in pawns) {
                if (pawn != null && pawn.apparel != null && !pawn.RaceProps.Animal) {
                    Universum.Colony.VacuumWeather.VacuumProtection protection =
                        Universum.Cache.Utilities.VacuumDamage.CheckProtection(pawn);
                    if (protection != Universum.Colony.VacuumWeather.VacuumProtection.All) {
                        try {
                            pawn.apparel.Wear((RimWorld.Apparel) ThingMaker.MakeThing(spacesuit_helmet), false);
                            pawn.apparel.Wear((RimWorld.Apparel) ThingMaker.MakeThing(spacesuit_armor), false);
                        } catch {
                            // ignored
                        }
                    }
                }
                yield return pawn;
            }
        }
    }
}
