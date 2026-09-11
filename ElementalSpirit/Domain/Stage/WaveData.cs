using System.Collections.Generic;

namespace ElementalSpirit.Domain.Stage
{
    public class WaveData
    {
        public int WaveNumber { get; set; }
        public List<SpawnData> Spawns { get; set; } = new();
        public float DelayBeforeStart { get; set; } = 1.5f; // nghỉ trước khi bắt đầu wave

        public WaveData(int waveNumber)
        {
            WaveNumber = waveNumber;
        }
    }
}