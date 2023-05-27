using System;
using UnityEngine;
using Verse;

namespace RimNauts2.World.Objects {
    public abstract class CelestialObject {
        private string name;
        private Defs.CelestialObject def;
        private string def_name;
        private Vector3 position;
        //private Vector3 orbit_position;
        //private float speed;
        private float speed_multiplier;
        //private float size;
        private Vector3 draw_size;
        
        private Vector3 color;
        private float rotation_angle;
        private float orbit_rotation_angle;
        private bool visible;

        private ObjectHolder object_holder;

        private TMPro.TextMeshPro feature_overlay_text;
        private bool feature_overlay_text_active;
        private bool feature_overlay_text_block;

        private TrailRenderer feature_overlay_trail;
        //private float feature_overlay_trail_length;
        //private float feature_overlay_trail_width;
        //private float feature_overlay_trail_brightness;
        //private float feature_overlay_trail_transparency;
        //private Color feature_overlay_trail_color;
        private bool feature_overlay_trail_active;
        private bool feature_overlay_trail_first_render = true;

        private Material material;
        private Quaternion rotation;
        private Quaternion camera_rotation;
    }
}
