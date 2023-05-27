using UnityEngine;
using Verse;

namespace RimNauts2.Defs {
    public class CelestialObject : Def {
        public int type;
        public Vector3 orbit_position = Vector3.zero;
        public Vector3 orbit_spread = Vector3.one;
        public Vector2 speed_between = Vector2.one;
        public Vector2 size_between = Vector2.one;
        public Vector2 brightness_between = Vector2.one;
        public bool object_holder = false;
        public TextureBuilder texture_builder;
        public Trail trail = null;

        public float speed() => Rand.Range(speed_between.x, speed_between.y);

        public float size() => Rand.Range(size_between.x, size_between.y);

        public float brightness() => Rand.Range(brightness_between.x, brightness_between.y);

        public Texture2D texture() => texture_builder.build();
    }
}
