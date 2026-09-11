using System.Drawing;
using ElementalSpirit.Data;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;
using ElementalSpirit.Services;

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
        public SpiritManager Spirits { get; }
        public PlayerWallet Wallet { get; }
        public Inventory Inventory { get; }
        public UpgradeService Upgrades { get; }
        public ShopService Shop { get; }

        public RectangleF PlayArea { get; private set; }
        public float FacingDirection { get; private set; } = 1f;
        public bool IsStageCompleted { get; private set; }
        public bool IsPaused { get; set; }
        public string StatusMessage { get; private set; } = "";
        private float _statusMessageTimer;

        public GameManager()
        {
            Input = new InputManager();
            Projectiles = new ProjectileManager();
            Enemies = new EnemyManager();
            Collision = new CollisionManager();
            Spawn = new SpawnManager(Enemies);
            Waves = new WaveManager(Spawn, Enemies);
            Spirits = new SpiritManager();
            Wallet = new PlayerWallet(250, 20, 2);
            Inventory = new Inventory();
            Upgrades = new UpgradeService();
            Shop = new ShopService();

            var starter = EquipmentCatalog.Find("wpn_basic_wand");
            if (starter != null)
            {
                Inventory.Add(starter.Clone());
                Inventory.TryEquip(starter.Id);
            }

            Player = new Player(180f, 360f);
            RefreshPlayerEquipmentStats();

            Waves.LoadStage(StageData.CreateEarthForest());
            Waves.OnStageCompleted += () => IsStageCompleted = true;
        }

        public void SetPlayArea(float width, float height)
        {
            PlayArea = new RectangleF(0, 0, width, height);
            Spawn.SetSpawnArea(width, height);
        }

        public void Update(float deltaTime)
        {
            if (_statusMessageTimer > 0f)
            {
                _statusMessageTimer -= deltaTime;
                if (_statusMessageTimer <= 0f) StatusMessage = "";
            }

            if (IsPaused || IsStageCompleted) return;

            var (dirX, dirY) = Input.GetMovementDirection();
            if (dirX != 0) FacingDirection = dirX > 0 ? 1f : -1f;
            if (dirX != 0 || dirY != 0) Player.Move(dirX, dirY, deltaTime);

            Player.ClampToBounds(PlayArea.Left, PlayArea.Top, PlayArea.Right, PlayArea.Bottom);
            Player.Update(deltaTime);
            Spirits.Update(deltaTime, Player);

            if (Input.IsKeyDown(System.Windows.Forms.Keys.Space) && Player.CanAttack())
            {
                Shoot();
                Player.ResetAttackCooldown();
            }

            Projectiles.Update(deltaTime);
            Enemies.Update(deltaTime);
            Collision.CheckCollisions(Projectiles, Enemies);
            Waves.Update(deltaTime);
        }

        private void Shoot()
        {
            float spawnX = Player.X + Player.Width / 2f;
            float spawnY = Player.Y + Player.Height / 2f - 3f;
            Projectiles.Add(ProjectileFactory.CreatePlayerProjectile(spawnX, spawnY, FacingDirection, Player.Damage));

            if (Player.ActiveWindBarrage && Player.ExtraProjectiles > 0)
            {
                float spread = 18f;
                for (int i = 0; i < Player.ExtraProjectiles; i++)
                {
                    int pair = i / 2 + 1;
                    float offsetY = (i % 2 == 0) ? -spread * pair : spread * pair;
                    Projectiles.Add(ProjectileFactory.CreatePlayerProjectile(
                        spawnX, spawnY + offsetY, FacingDirection, Player.Damage));
                }
            }
        }

        public void RefreshPlayerEquipmentStats()
        {
            Player.ApplyEquipmentBonuses(
                Inventory.TotalBonusDamage,
                Inventory.TotalBonusMaxHp,
                Inventory.TotalBonusDefense);
        }

        private void SetStatus(string message)
        {
            StatusMessage = message;
            _statusMessageTimer = 2.5f;
        }

        public void HandleKeyDown(System.Windows.Forms.Keys key)
        {
            Input.KeyDown(key);

            if (key is System.Windows.Forms.Keys.D1 or System.Windows.Forms.Keys.NumPad1)
                Spirits.TryActivate(0, Player);
            else if (key is System.Windows.Forms.Keys.D2 or System.Windows.Forms.Keys.NumPad2)
                Spirits.TryActivate(1, Player);

            if (key == System.Windows.Forms.Keys.N)
                Waves.ForceNextWave();

            if (key == System.Windows.Forms.Keys.D3)
            {
                Spirits.Equip(0, Spirits.Unlocked[0]);
                Spirits.Equip(1, Spirits.Unlocked[1]);
                SetStatus("Equipped: Terra + Ignis");
            }
            else if (key == System.Windows.Forms.Keys.D4)
            {
                Spirits.Equip(0, Spirits.Unlocked[2]);
                Spirits.Equip(1, Spirits.Unlocked[3]);
                SetStatus("Equipped: Aqua + Zephyr");
            }

            if (key == System.Windows.Forms.Keys.G)
            {
                Wallet.AddGold(100);
                Wallet.AddSpiritShards(10);
                SetStatus("+100 Gold, +10 Spirit Shards");
            }
        }

        public void HandleKeyUp(System.Windows.Forms.Keys key) => Input.KeyUp(key);
    }
}