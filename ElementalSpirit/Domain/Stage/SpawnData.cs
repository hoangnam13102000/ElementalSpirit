using ElementalSpirit.Factories;

namespace ElementalSpirit.Domain.Stage
{
    public class SpawnData
    {
        public EnemyType EnemyType { get; set; }
        public int Count { get; set; }
        public float SpawnInterval { get; set; } = 0.6f; // giây giữa mỗi con

        public SpawnData(EnemyType type, int count, float interval = 0.6f)
        {
            EnemyType = type;
            Count = count;
            SpawnInterval = interval;
        }
    }
}