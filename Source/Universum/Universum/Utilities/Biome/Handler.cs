﻿using System.Linq;

namespace Universum.Utilities.Biome {
    public static class Handler {
        private static int total_biomes_found;
        private static int total_configurations_found;
        private static Properties[] biomes;

        public static Properties get_properties(this RimWorld.BiomeDef biome_def) {
            try {
                return biomes[biome_def.index];
            } catch {
                return new Properties();
            }
        }

        public static void init() {
            biomes = Properties.GetAll();
            // set stats
            total_biomes_found = biomes.Count();
            total_configurations_found = 0;
            foreach (Properties biome in biomes) {
                total_configurations_found += biome.allowed_utilities.Count();
            }
            // print stats
            Logger.print(
                Importance.Info,
                key: "Biome_Handler_Stats",
                prefix: false,
                args: new Verse.NamedArgument[] { total_biomes_found, total_configurations_found }
            );
        }
    }
}
