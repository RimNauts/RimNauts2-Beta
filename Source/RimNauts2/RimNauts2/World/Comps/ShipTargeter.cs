using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimNauts2.World.Comps {
    public class ShipTargeter_Properties : RimWorld.WorldObjectCompProperties {
        public ShipTargeter_Properties() => compClass = typeof(Targeter);
    }

    public class Targeter : RimWorld.Planet.WorldObjectComp {
        public ShipTargeter_Properties Props => (ShipTargeter_Properties) props;
        public int target_tile = -1;
        public bool valid = false;

        public override void PostExposeData() {
            base.PostExposeData();
            Scribe_Values.Look(ref target_tile, "target_tile", -1);
            Scribe_Values.Look(ref valid, "valid", false);
        }

        public override IEnumerable<Gizmo> GetGizmos() {
            ObjectHolder parent = this.parent as ObjectHolder;
            if (Defs.Loader.object_metadata[parent.type].ship) {
                Command_Action cmd = new Command_Action {
                    defaultLabel = "Choose target",
                    defaultDesc = "",
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true),
                    action = new Action(target)
                };
                yield return cmd;
            }
        }

        public void target() {
            Find.WorldSelector.ClearSelection();
            Find.WorldTargeter.BeginTargeting(
                action: new Func<RimWorld.Planet.GlobalTargetInfo, bool>(get_tile_target),
                canTargetTiles: false
            );
        }

        public bool get_tile_target(RimWorld.Planet.GlobalTargetInfo target) {
            if (!target.IsValid || !Cache.exists(target.Tile)) return false;
            target_tile = target.Tile;
            valid = true;
            return true;
        }

        public bool valid_target() {
            if (!valid) return false;
            if (target_tile == -1) {
                valid = false;
                return false;
            }
            if (!Cache.exists(target_tile)) {
                target_tile = -1;
                valid = false;
                return false;
            }
            return true;
        }

        public Vector3 get_coords() {
            return Cache.get(target_tile).position;
        }
    }
}
