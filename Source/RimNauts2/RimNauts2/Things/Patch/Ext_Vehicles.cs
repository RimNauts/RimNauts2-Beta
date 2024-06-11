using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace RimNauts2.Things.Patch {
    public class Ext_Vehicles {
        public static void Init(Harmony harmony) {
            _ = new PatchClassProcessor(harmony, typeof(Ext_Vehicles_IsRoofed)).Patch();
            _ = new PatchClassProcessor(harmony, typeof(Ext_Vehicles_IsRoofRestricted)).Patch();
            _ = new PatchClassProcessor(harmony, typeof(Ext_Vehicles_IsRoofRestricted_2)).Patch();
        }
    }

    [HarmonyPatch]
    static class Ext_Vehicles_IsRoofed {
        public static bool Prepare() => TargetMethod() != null;

        public static MethodBase TargetMethod() => AccessTools.Method("Vehicles.Ext_Vehicles:IsRoofed");

        public static bool Prefix(IntVec3 cell, Map map, ref bool __result) {
            if (map.roofGrid.RoofAt(cell) != Defs.Loader.roof_magnetic_field) return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch]
    static class Ext_Vehicles_IsRoofRestricted {
        public static bool Prepare() => TargetMethod() != null;

        public static MethodBase TargetMethod() {
            var type = AccessTools.TypeByName("Vehicles.Ext_Vehicles");
            if (type == null) return null;

            var method = AccessTools.Method(type, "IsRoofRestricted", new Type[] { typeof(IntVec3), typeof(Map), typeof(bool) });
            if (method == null) return null;

            return method;
        }

        public static bool Prefix(IntVec3 cell, Map map, bool canRoofPunch, ref bool __result) {
            if (map.roofGrid.RoofAt(cell) != Defs.Loader.roof_magnetic_field) return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch]
    static class Ext_Vehicles_IsRoofRestricted_2 {
        public static bool Prepare() => TargetMethod() != null;

        public static MethodBase TargetMethod() {
            var type = AccessTools.TypeByName("Vehicles.Ext_Vehicles");
            if (type == null) return null;

            var vehicleDefType = AccessTools.TypeByName("Vehicles.VehicleDef");
            if (vehicleDefType == null) return null;

            var method = AccessTools.Method(type, "IsRoofRestricted", new Type[] { vehicleDefType, typeof(IntVec3), typeof(Map) });
            if (method == null) return null;

            return method;
        }

        public static bool Prefix(object vehicleDef, IntVec3 cell, Map map, ref bool __result) {
            if (map.roofGrid.RoofAt(cell) != Defs.Loader.roof_magnetic_field) return true;

            __result = false;
            return false;
        }
    }
}
