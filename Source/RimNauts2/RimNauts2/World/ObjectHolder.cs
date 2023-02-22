﻿using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RimNauts2.World {
    [StaticConstructorOnStartup]
    public class ObjectHolder : RimWorld.Planet.MapParent {
        public Objects.NEO visual_object;
        public bool keep_after_abandon;
        public Type type;
        string texture_path;
        Vector3 orbit_position;
        float orbit_speed;
        Vector3 draw_size;
        int period;
        int time_offset;
        int orbit_direction;
        float color;
        float rotation_angle;
        Vector3 current_position;

        public override void ExposeData() {
            base.ExposeData();
            if (visual_object != null) {
                type = visual_object.type;
                texture_path = visual_object.texture_path;
                orbit_position = visual_object.orbit_position;
                orbit_speed = visual_object.orbit_speed;
                draw_size = visual_object.draw_size;
                period = visual_object.period;
                time_offset = visual_object.time_offset;
                orbit_direction = visual_object.orbit_direction;
                color = visual_object.color;
                rotation_angle = visual_object.rotation_angle;
                current_position = visual_object.current_position;
            }
            Scribe_Values.Look(ref keep_after_abandon, "keep_after_abandon");
            Scribe_Values.Look(ref type, "type");
            Scribe_Values.Look(ref texture_path, "texture_path");
            Scribe_Values.Look(ref orbit_position, "orbit_position");
            Scribe_Values.Look(ref orbit_speed, "orbit_speed");
            Scribe_Values.Look(ref draw_size, "draw_size");
            Scribe_Values.Look(ref period, "period");
            Scribe_Values.Look(ref time_offset, "time_offset");
            Scribe_Values.Look(ref orbit_direction, "orbit_direction");
            Scribe_Values.Look(ref color, "color");
            Scribe_Values.Look(ref rotation_angle, "rotation_angle");
            Scribe_Values.Look(ref current_position, "current_position");
        }

        public override Vector3 DrawPos => get_position();

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject) {
            alsoRemoveWorldObject = true;
            if ((from ob in Find.World.worldObjects.AllWorldObjects
                 where ob is RimWorld.Planet.TravelingTransportPods pods && ((int) typeof(RimWorld.Planet.TravelingTransportPods).GetField("initialTile", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ob) == Tile || pods.destinationTile == Tile)
                 select ob).Count() > 0) {
                return false;
            }
            return base.ShouldRemoveMapNow(out alsoRemoveWorldObject);
        }

        public override void PostRemove() {
            base.PostRemove();
            if (keep_after_abandon) {
                ObjectHolder object_holder = (ObjectHolder) RimWorld.Planet.WorldObjectMaker.MakeWorldObject(
                    DefDatabase<RimWorld.WorldObjectDef>.GetNamed("RimNauts2_ObjectHolder")
                );
                object_holder.Tile = Tile;
                object_holder.def.mapGenerator = def.mapGenerator;
                object_holder.def.label = def.label;
                object_holder.def.description = def.description;
                object_holder.keep_after_abandon = keep_after_abandon;
                object_holder.visual_object = visual_object;
                object_holder.type = type;
                Find.WorldObjects.Add(object_holder);
            }
            Find.World.grid.tiles.ElementAt(Tile).biome = DefDatabase<RimWorld.BiomeDef>.GetNamed("Ocean");
        }

        public Vector3 get_position() {
            if (visual_object != null) {
                return visual_object.current_position;
            } else if (type != Type.None && RimNauts_GameComponent.render_manager != null) {
                visual_object = RimNauts_GameComponent.render_manager.populate(
                    type,
                    texture_path,
                    orbit_position,
                    orbit_speed,
                    draw_size,
                    period,
                    time_offset,
                    orbit_direction,
                    color,
                    rotation_angle,
                    current_position
                );
                visual_object.object_holder = true;
                return visual_object.current_position;
            } else return Vector3.zero;
        }

        public void add_visual_object(
            Type type,
            string texture_path = null,
            Vector3? orbit_position = null,
            float? orbit_speed = null,
            Vector3? draw_size = null,
            int? period = null,
            int? time_offset = null,
            int? orbit_direction = null,
            float? color = null,
            float? rotation_angle = null,
            Vector3? current_position = null
        ) {
            visual_object = RimNauts_GameComponent.render_manager.populate(
                type,
                texture_path,
                orbit_position,
                orbit_speed,
                draw_size,
                period,
                time_offset,
                orbit_direction,
                color,
                rotation_angle,
                current_position
            );
            visual_object.object_holder = true;
            this.type = type;
        }
    }
}