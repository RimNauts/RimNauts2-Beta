using System.Collections.Generic;
using Verse;

namespace RimNauts2.Things.Building {
    [StaticConstructorOnStartup]
    public class Ejector : Verse.Building {
        public static HashSet<Ejector> ejectors = new HashSet<Ejector>();
        public static Dictionary<Room, int> roofCount = new Dictionary<Room, int>();
        Comps.Eject eject = null;
        RimWorld.CompPowerTrader power = null;
        private string descEqualizing = "Equalizing";
        public float equalizingValue = 0.0f;
        public Room[] rooms = new Room[2];
        public bool[] outside = new bool[2];
        public bool outerspace = false;

        ~Ejector() {
            ejectors.Remove(this);
        }

        public Comps.Eject Eject {
            get {
                if (eject != null) return eject;
                eject = GetComp<Comps.Eject>();
                return eject;
            }
        }

        public RimWorld.CompPowerTrader Power {
            get {
                if (power != null) return power;
                power = GetComp<RimWorld.CompPowerTrader>();
                return power;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad) {
            base.SpawnSetup(map, respawningAfterLoad);

            outerspace = Universum.Utilities.Cache.allowed_utility(Map, "universum.vacuum");

            ejectors.Add(this);

            updateRoomData();
            rooms[0]?.Notify_RoofChanged();
            rooms[1]?.Notify_RoofChanged();
        }

        public void updateRoomData() {
            IntVec3 cellNorth = Position + IntVec3.North.RotatedBy(Rotation);
            IntVec3 cellSouth = Position + IntVec3.South.RotatedBy(Rotation);

            rooms[0] = cellNorth.GetRoom(Map);
            rooms[1] = cellSouth.GetRoom(Map);

            outside[0] = rooms[0] == null || Patch.Room_OpenRoofCountStopAt.cache[rooms[0]] > 0 || rooms[0].TouchesMapEdge;
            outside[1] = rooms[1] == null || Patch.Room_OpenRoofCountStopAt.cache[rooms[0]] > 0 || rooms[1].TouchesMapEdge;
        }

        public bool hasRoom(Room room) {
            return rooms[0] == room || rooms[1] == room;
        }

        public bool countAsOutside(Room room) {
            if (room == null) return false;

            if (rooms[0] == room) return !outside[0] && outside[0] != outside[1];
            if (rooms[1] == room) return !outside[1] && outside[1] != outside[0];

            return false;
        }

        public override void TickRare() {
            base.TickRare();

            if (Eject.open) {
                // act as a vent to equalize temperature
                GenTemperature.EqualizeTemperaturesThroughBuilding(this, 42f, true);
                Map.gasGrid.EqualizeGasThroughBuilding(this, true);

                if (!outerspace) {
                    equalizingValue = 1.0f;
                    return;
                }

                updateRoomData();

                if (equalizingValue < 1.0f) {
                    equalizingValue += 0.02f;

                    if (outside[0] == outside[1]) equalizingValue = 1.0f;
                }
            }
        }

        public override string GetInspectString() {
            string desc = base.GetInspectString();

            if (!Eject.open) return desc;

            if (!desc.NullOrEmpty()) desc = descEqualizing + " " + (equalizingValue * 100).ToString("0") + "%\n" + desc;

            return desc;
        }
    }
}
