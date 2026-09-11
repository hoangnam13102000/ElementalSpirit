using System.Collections.Generic;
using ElementalSpirit.Factories;

namespace ElementalSpirit.Domain.Stage
{
    public class StageData
    {
        public int StageNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<WaveData> Waves { get; set; } = new();

        public static StageData CreateEarthForest()
        {
            var stage = new StageData
            {
                StageNumber = 1,
                Name = "Earth Forest"
            };

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

            // Wave 4 - nhiều hơn
            var wave4 = new WaveData(4);
            wave4.Spawns.Add(new SpawnData(EnemyType.Slime, 12, 0.4f));
            stage.Waves.Add(wave4);

            return stage;
        }
    }
}