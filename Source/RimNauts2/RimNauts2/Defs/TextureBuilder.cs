using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimNauts2.Defs {
    public class TextureBuilder {
        public List<TextureBuilderLayer> layers = new List<TextureBuilderLayer>();
        private int width = 0;
        private int height = 0;

        public Texture2D build() {
            if (layers.NullOrEmpty()) return null;
            Color[] pixels = null;
            foreach (var layer in layers) {
                Color[] layer_pixels = get_layer_pixels(layer);
                if (layer.color != null) layer_pixels = add_color(layer_pixels, layer.color.Value);
                pixels = combine_pixels(pixels, layer_pixels);
            }
            Texture2D texture = Assets.get_readable_texture(Assets.get_texture("Satellites/PlanetParts/sphere1"));
            texture.SetPixels(pixels);
            return texture;
        }

        private Color[] combine_pixels(Color[] pixels, Color[] layer_pixels) {
            if (pixels.NullOrEmpty()) return layer_pixels;
            for (int i = 0; i < pixels.Length; i++) {
                if (pixels[i].a == 0 || layer_pixels[i].a == 0) continue;
                pixels[i] = layer_pixels[i];
            }
            return pixels;
        }

        private Color[] add_color(Color[] pixels, Color color) {
            for (int i = 0; i < pixels.Length; i++) {
                //if (pixels[i].a == 0) continue;
                Log.Message(pixels[i].ToString());
                pixels[i] = pixels[i].RGBMultiplied(0.5f);
                pixels[i] = new Color(color.r * pixels[i].r, color.g * pixels[i].g, color.b * pixels[i].b, pixels[i].a);
                pixels[i] = color;
            }
            return pixels;
        }

        private Color[] get_layer_pixels(TextureBuilderLayer layer) {
            if (layer.texture_paths.NullOrEmpty()) return null;
            Texture2D texture = Assets.get_texture(layer.texture_paths.RandomElement());
            if (texture.NullOrBad()) return null;
            if ((width != 0 || height != 0) && (width != texture.width || height != texture.height)) return null;
            // get readable version of the texture
            texture = Assets.get_readable_texture(texture);
            return texture.GetPixels();
        }
    }

    public struct TextureBuilderLayer {
        public List<string> texture_paths;
        public Color? color;
    }
}
