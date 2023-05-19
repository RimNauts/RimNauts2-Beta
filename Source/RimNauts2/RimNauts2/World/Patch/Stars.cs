using HarmonyLib;
using System.Collections;
using UnityEngine;
using Verse;

namespace RimNauts2.World.Patch {
    [HarmonyPatch(typeof(RimWorld.Planet.WorldLayer_Stars), "Regenerate")]
    class Stars {
        public static bool Prefix(ref RimWorld.Planet.WorldLayer_Stars __instance, ref IEnumerable __result) {
            __result = bam(__instance);
            return false;
        }

        public static IEnumerable bam(RimWorld.Planet.WorldLayer_Stars __instance) {
            __instance.dirty = false;
            __instance.ClearSubMeshes(MeshParts.All);

            __instance.calculatedForStartingTile = (Find.GameInitData != null) ? Find.GameInitData.startingTile : (-1);
            __instance.calculatedForStaticRotation = __instance.UseStaticRotation;
            __instance.FinalizeMesh(MeshParts.All);

            yield break;
        }
    }
}
