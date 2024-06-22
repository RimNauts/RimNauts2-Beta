using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimNauts2.Things.Comps {
    public class Eject_Properties : CompProperties {
        public Eject_Properties() => compClass = typeof(Eject);
    }

    public class Eject : ThingComp {
        public Eject_Properties Props => (Eject_Properties) props;
        public Building.Ejector Parent => (Building.Ejector) parent;
        public bool open = false;
        public RimWorld.CompPowerTrader compPowerTrader;

        public RimWorld.CompPowerTrader getCompPowerTrader {
            get {
                if (compPowerTrader != null) return compPowerTrader;
                compPowerTrader = Parent.GetComp<RimWorld.CompPowerTrader>();
                return compPowerTrader;
            }
        }

        public override void PostExposeData() {
            base.PostExposeData();
            Scribe_Values.Look(ref open, "open", true);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra() {
            bool powered = getCompPowerTrader.PowerOn;

            if (open) {
                Command_Action cmd = new Command_Action {
                    defaultLabel = "RimNauts.ejector_close_label".Translate(),
                    defaultDesc = "RimNauts.ejector_close_desc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Icons/RimNauts2_Ejector_Close", true),
                    action = new Action(toggle)
                };

                if (!powered) cmd.Disable("RimNauts.ejector_missing_power".Translate());

                yield return cmd;
            } else {
                Command_Action cmd = new Command_Action {
                    defaultLabel = "RimNauts.ejector_open_label".Translate(),
                    defaultDesc = "RimNauts.ejector_open_desc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Icons/RimNauts2_Ejector_Open", true),
                    action = new Action(toggle)
                };

                if (!powered) cmd.Disable("RimNauts.ejector_missing_power".Translate());

                yield return cmd;
            }
        }

        public void toggle() {
            open = !open;

            Parent.updateRoomData();
            Parent.rooms[0]?.Notify_RoofChanged();
            Parent.rooms[1]?.Notify_RoofChanged();

            if (open) Parent.equalizingValue = 0f;
        }
    }
}
