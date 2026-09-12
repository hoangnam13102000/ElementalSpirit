using System.Collections.Generic;
using ElementalSpirit.Factories;

namespace ElementalSpirit.Domain.Stage
{
    public class StageData
    {
        public int StageNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BackgroundImageName { get; set; } = string.Empty;
        public List<WaveData> Waves { get; set; } = new();

        /// <summary>
        /// Ground and platform definitions for this stage.
        /// The first platform is always the main ground plane.
        /// Uses ratio-based coordinates (0.0 to 1.0) so it scales with any play area size.
        /// </summary>
        public List<TerrainPlatform> Platforms { get; set; } = new();

        public static StageData CreateEarthForest()
        {
            var stage = new StageData
            {
                StageNumber = 1,
                Name = "Earth Forest",
                BackgroundImageName = "EarthForest.png"
            };

            // Main ground plane: spans full width, aligned with the visible path in background
            stage.Platforms.Add(new TerrainPlatform(
                name: "Ground",
                minXRatio: 0.0f,
                maxXRatio: 1.0f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            // Wave 1
            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 6, 0.7f));
            stage.Waves.Add(wave1);

            // Wave 2
            var wave2 = new WaveData(2);
            wave2.Spawns.Add(new SpawnData(EnemyType.Slime, 8, 0.55f));
            stage.Waves.Add(wave2);

            // Wave 3
            var wave3 = new WaveData(3);
            wave3.Spawns.Add(new SpawnData(EnemyType.Slime, 10, 0.45f));
            stage.Waves.Add(wave3);

            // Wave 4
            var wave4 = new WaveData(4);
            wave4.Spawns.Add(new SpawnData(EnemyType.Slime, 12, 0.4f));
            stage.Waves.Add(wave4);

            return stage;
        }
    }
}