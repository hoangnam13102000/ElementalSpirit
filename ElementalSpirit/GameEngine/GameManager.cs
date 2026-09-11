using System.Drawing;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;

namespace ElementalSpirit.GameEngine
{
    public class GameManager
    {
        public Player Player { get; }
        public InputManager Input { get; }
        public ProjectileManager Projectiles { get; }
        public EnemyManager Enemies { get; }
        public CollisionManager Collision { get; }
        public SpawnManager Spawn { get; }
        public WaveManager Waves { get; }

        public RectangleF PlayArea { get; private set; }
        public float FacingDirection { get; private set; } = 1f;

        public bool IsStageCompleted { get; private set; }

        public GameManager()
        {
            Input = new InputManager();
            Projectiles = new ProjectileManager();
            Enemies = new EnemyManager();
            Collision = new CollisionManager();
            Spawn = new SpawnManager(Enemies);
            Waves = new WaveManager(Spawn, Enemies);

            Player = new Player(180f, 360f);

            // Load Stage 1
            var stage1 = StageData.CreateEarthForest();
            Waves.LoadStage(stage1);

            Waves.OnStageCompleted += () => IsStageCompleted = true;
        }

        public void SetPlayArea(float width, float height)
        {
            PlayArea = new RectangleF(0, 0, width, height);
            Spawn.SetSpawnArea(width, height);
        }

        public void Update(float deltaTime)
        {
            if (IsStageCompleted) return;

            // Movement
            var (dirX, dirY) = Input.GetMovementDirection();
            if (dirX != 0)
                FacingDirection = dirX > 0 ? 1f : -1f;

            if (dirX != 0 || dirY != 0)
                Player.Move(dirX, dirY, deltaTime);

            Player.ClampToBounds(PlayArea.Left, PlayArea.Top, PlayArea.Right, PlayArea.Bottom);
            Player.Update(deltaTime);

            // Shoot
            if (Input.IsKeyDown(System.Windows.Forms.Keys.Space) && Player.CanAttack())
            {
                Shoot();
                Player.ResetAttackCooldown();
            }

            // Systems
            Projectiles.Update(deltaTime);
            Enemies.Update(deltaTime);
            Collision.CheckCollisions(Projectiles, Enemies);
            Waves.Update(deltaTime);
        }

        private void Shoot()
        {
            float spawnX = Player.X + Player.Width / 2f;
            float spawnY = Player.Y + Player.Height / 2f - 3f;

            var bullet = ProjectileFactory.CreatePlayerProjectile(
                spawnX, spawnY, FacingDirection, Player.Damage);

            Projectiles.Add(bullet);
        }

        public void HandleKeyDown(System.Windows.Forms.Keys key)
        {
            Input.KeyDown(key);

            // Phím tắt test nhanh (có thể xóa sau)
            if (key == System.Windows.Forms.Keys.N)
                Waves.ForceNextWave();
        }

        public void HandleKeyUp(System.Windows.Forms.Keys key)
        {
            Input.KeyUp(key);
        }
    }
}