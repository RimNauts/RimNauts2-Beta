﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Verse;

namespace RimNauts2.World {
    [StaticConstructorOnStartup]
    public class RenderManager : RimWorld.Planet.WorldObject {
        public int prev_tick = -1;
        public Vector3 prev_cam_pos = new Vector3();
        public List<Objects.NEO> visual_objects = new List<Objects.NEO>();
        public int total_objects = 0;
        public Matrix4x4[] cached_matrices = new Matrix4x4[0];
        public Material[] cached_materials = new Material[0];
        public bool materials_dirty = true;
        private List<Type> expose_type = new List<Type>();
        private List<string> expose_texture_path = new List<string>();
        private List<Vector3> expose_orbit_position = new List<Vector3>();
        private List<float> expose_orbit_speed = new List<float>();
        private List<Vector3> expose_draw_size = new List<Vector3>();
        private List<int> expose_period = new List<int>();
        private List<int> expose_time_offset = new List<int>();
        private List<int> expose_orbit_direction = new List<int>();
        private List<float> expose_color = new List<float>();
        private List<float> expose_rotation_angle = new List<float>();
        private List<Vector3> expose_current_position = new List<Vector3>();
        //public MaterialPropertyBlock properties = new MaterialPropertyBlock();

        public override void PostAdd() {
            base.PostAdd();
            // increase far clipping plane to see asteroids further back when zoomed out
            Find.WorldCamera.farClipPlane = 1600.0f;
        }

        public override void ExposeData() {
            base.ExposeData();
            for (int i = 0; i < total_objects; i++) {
                expose_type.Add(visual_objects[i].type);
                expose_texture_path.Add(visual_objects[i].texture_path);
                expose_orbit_position.Add(visual_objects[i].orbit_position);
                expose_orbit_speed.Add(visual_objects[i].orbit_speed);
                expose_draw_size.Add(visual_objects[i].draw_size);
                expose_period.Add(visual_objects[i].period);
                expose_time_offset.Add(visual_objects[i].time_offset);
                expose_orbit_direction.Add(visual_objects[i].orbit_direction);
                expose_color.Add(visual_objects[i].color);
                expose_rotation_angle.Add(visual_objects[i].rotation_angle);
                expose_current_position.Add(visual_objects[i].current_position);
            }
            Scribe_Values.Look(ref total_objects, "total_objects");
            Scribe_Collections.Look(ref expose_type, "expose_type", LookMode.Value);
            Scribe_Collections.Look(ref expose_texture_path, "expose_texture_path", LookMode.Value);
            Scribe_Collections.Look(ref expose_orbit_position, "expose_orbit_position", LookMode.Value);
            Scribe_Collections.Look(ref expose_orbit_speed, "expose_orbit_speed", LookMode.Value);
            Scribe_Collections.Look(ref expose_draw_size, "expose_draw_size", LookMode.Value);
            Scribe_Collections.Look(ref expose_period, "expose_period", LookMode.Value);
            Scribe_Collections.Look(ref expose_time_offset, "expose_time_offset", LookMode.Value);
            Scribe_Collections.Look(ref expose_orbit_direction, "expose_orbit_direction", LookMode.Value);
            Scribe_Collections.Look(ref expose_color, "expose_color", LookMode.Value);
            Scribe_Collections.Look(ref expose_rotation_angle, "expose_rotation_angle", LookMode.Value);
            Scribe_Collections.Look(ref expose_current_position, "expose_current_position", LookMode.Value);
            for (int i = 0; i < total_objects; i++) {
                add(
                    expose_type[i],
                    expose_texture_path[i],
                    expose_orbit_position[i],
                    expose_orbit_speed[i],
                    expose_draw_size[i],
                    expose_period[i],
                    expose_time_offset[i],
                    expose_orbit_direction[i],
                    expose_color[i],
                    expose_rotation_angle[i],
                    expose_current_position[i]
                );
            }
            recache();
            expose_type = new List<Type>();
            expose_texture_path = new List<string>();
            expose_orbit_position = new List<Vector3>();
            expose_orbit_speed = new List<float>();
            expose_draw_size = new List<Vector3>();
            expose_period = new List<int>();
            expose_time_offset = new List<int>();
            expose_orbit_direction = new List<int>();
            expose_color = new List<float>();
            expose_rotation_angle = new List<float>();
            expose_current_position = new List<Vector3>();
        }

        public override void Draw() {
            int tick = Find.TickManager.TicksGame;
            Vector3 cam_pos = Find.WorldCamera.transform.position;
            bool unpaused = tick != prev_tick;
            bool camera_moved = cam_pos != prev_cam_pos;
            // update cache
            recache_materials();
            // update objects
            if (unpaused || camera_moved) {
                Vector3 center = Find.WorldCameraDriver.CurrentlyLookingAtPointOnSphere;
                Parallel.For(0, total_objects, i => {
                    visual_objects[i].update();
                    if (unpaused) visual_objects[i].update_when_unpaused(tick);
                    if (camera_moved) visual_objects[i].update_when_camera_moved();
                    cached_matrices[i] = visual_objects[i].get_transformation_matrix(center);
                });
                prev_tick = tick;
                prev_cam_pos = cam_pos;
            }
            // draw objects
            /*Graphics.DrawMeshInstanced(
                MeshPool.plane10,
                submeshIndex: 0,
                cached_materials[0],
                cached_matrices,
                count: total_objects,
                properties: null,
                ShadowCastingMode.Off,
                receiveShadows: false,
                RimWorld.Planet.WorldCameraManager.WorldLayer + 1000,
                camera: null,
                LightProbeUsage.BlendProbes,
                lightProbeProxyVolume: null
            );*/
            for (int i = 0; i < total_objects; i++) {
                Graphics.DrawMesh(
                    MeshPool.plane10,
                    cached_matrices[i],
                    cached_materials[i],
                    RimWorld.Planet.WorldCameraManager.WorldLayer,
                    camera: null,
                    submeshIndex: 0,
                    properties: null,
                    ShadowCastingMode.Off,
                    receiveShadows: false,
                    probeAnchor: null,
                    lightProbeUsage: LightProbeUsage.BlendProbes,
                    lightProbeProxyVolume: null
                );
            }
        }

        public Objects.NEO add(Type type) {
            Objects.NEO neo = type.neo();
            visual_objects.Add(neo);
            return neo;
        }

        public Objects.NEO add(
            Type type,
            string texture_path,
            Vector3 orbit_position,
            float orbit_speed,
            Vector3 draw_size,
            int period,
            int time_offset,
            int orbit_direction,
            float color,
            float rotation_angle,
            Vector3 current_position
        ) {
            Objects.NEO neo = type.neo(
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
            visual_objects.Add(neo);
            return neo;
        }

        public void populate(Type type, int amount) {
            total_objects += amount;
            for (int i = 0; i < amount; i++) add(type);
            recache();
        }

        public void depopulate(Type type) {
            int removed_amount = visual_objects.RemoveAll(visual_object => visual_object.type == type);
            total_objects -= removed_amount;
            recache();
        }

        public void recache() {
            cached_matrices = new Matrix4x4[total_objects];
            materials_dirty = true;
            //Vector4[] colors = new Vector4[total_objects];
            //for (int i = 0; i < total_objects; i++) colors[i] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //properties.SetVectorArray("_Colors", colors);
        }

        public void recache_materials() {
            if (!materials_dirty) return;
            materials_dirty = false;
            cached_materials = new Material[total_objects];
            for (int i = 0; i < total_objects; i++) cached_materials[i] = visual_objects[i].get_material();
        }
    }
}