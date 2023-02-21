﻿using System;
using UnityEngine;
using Verse;

namespace RimNauts2.World.Objects {
    public class AsteroidCrashing : NEO {
        public bool out_of_bounds_direction_towards_surface;
        public float out_of_bounds_offset;
        public float current_out_of_bounds;
        public float crash_course;

        public AsteroidCrashing() : base(Type.AsteroidCrashing) {
            out_of_bounds_direction_towards_surface = Rand.Bool;
            if (out_of_bounds_direction_towards_surface) {
                out_of_bounds_offset = out_of_bounds_offset = Rand.Range(1.0f, 10.0f);
            } else out_of_bounds_offset = Rand.Range(-10.0f, 1.0f);
            current_out_of_bounds = out_of_bounds_offset;

        }

        public AsteroidCrashing(
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
        ) : base(
            Type.AsteroidCrashing,
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
        ) {
            out_of_bounds_direction_towards_surface = Rand.Bool;
            if (out_of_bounds_direction_towards_surface) {
                out_of_bounds_offset = out_of_bounds_offset = Rand.Range(1.0f, 10.0f);
            } else out_of_bounds_offset = Rand.Range(-10.0f, 1.0f);
            current_out_of_bounds = out_of_bounds_offset;
        }

        public override void update() {
            base.update();
        }

        public override void update_when_unpaused(int tick) {
            if (out_of_bounds_direction_towards_surface) {
                current_out_of_bounds -= 0.00015f;
                if (current_out_of_bounds <= 0.42f) current_out_of_bounds = out_of_bounds_offset;
            } else {
                current_out_of_bounds += 0.0002f;
                if (current_out_of_bounds >= 2.4f) current_out_of_bounds = out_of_bounds_offset;
            }
            base.update_when_unpaused(tick);
        }

        public override void update_when_camera_moved() {
            base.update_when_camera_moved();
        }

        public override void update_position(int tick) {
            get_crash_course();
            float time = orbit_speed * orbit_direction * tick + time_offset;
            float num1 = 6.28f / period * time;
            float num2 = Math.Abs(orbit_position.y) / 2;
            current_position.x = (orbit_position.x - num2) * (float) Math.Cos(num1) * crash_course;
            current_position.z = (orbit_position.z - num2) * (float) Math.Sin(num1) * crash_course;
        }

        public void get_crash_course() {
            if (out_of_bounds_direction_towards_surface) {
                crash_course = Math.Min(1.0f, current_out_of_bounds);
            } else crash_course = Math.Max(1.0f, current_out_of_bounds);
        }
    }
}