using System.Linq;
using UnityEngine;
using Verse;

namespace RimNauts2.Things.PlaceWorker {
    public class PlaceWorker_StellarEjector : Verse.PlaceWorker {
        public override void DrawGhost(
            ThingDef def,
            IntVec3 center,
            Rot4 rot,
            Color ghostCol,
            Thing thing = null
        ) {
            Map map = Find.CurrentMap;

            IntVec3 loc_south = center + IntVec3.South.RotatedBy(rot);
            Room room_south = loc_south.GetRoom(map);
            bool vacuum_south = room_south == null || room_south.OpenRoofCount > 0 || room_south.TouchesMapEdge;

            if (!vacuum_south) return;

            IntVec3 loc_north = center + IntVec3.North.RotatedBy(rot);
            Room room_north = loc_north.GetRoom(map);
            bool vacuum_north = room_north == null || room_north.OpenRoofCount > 0 || room_north.TouchesMapEdge;

            if (vacuum_north) return;

            GenDraw.DrawFieldEdges(room_north.Cells.ToList());
        }
    }
}
