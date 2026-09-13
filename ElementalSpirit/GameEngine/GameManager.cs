using System;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Data;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;
using ElementalSpirit.Services;
using ElementalSpirit.GameEngine.Abstractions;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public class GameManager
    {
        public Player Player { get; }
        public IInputManager Input { get; }
        public IProjectileManager Projectiles { get; }
        public IEnemyManager Enemies { get; }
        public ICollisionManager Collision { get; }
        public ISpawnManager Spawn { get; }
        public IWaveManager Waves { get; }
        public ISpiritManager Spirits { get; }
        public PlayerWallet Wallet { get; }
        public Inventory Inventory { get; }
        public IUpgradeService Upgrades { get; }
        public IShopService Shop { get; }

        public RectangleF PlayArea { get; private set; }
        public float GroundY { get; private set; }
        private const float GroundTopRatio = 0.78f;

        public bool IsStageCompleted { get; private set; }
        public bool IsPaused { get; set; }
        public string StatusMessage { get; private set; } = "";
        private float _statusMessageTimer;
        private bool _jumpKeyWasPressed;
        private bool _attackKeyWasPressed;
        private bool _fireKeyWasPressed;

        public GameManager(
            IInputManager input,
            IProjectileManager projectiles,
            IEnemyManager enemies,
            ICollisionManager collision,
            ISpawnManager spawn,
            IWaveManager waves,
            ISpiritManager spirits,
            IShopService shop,
            IUpgradeService upgrades,
            PlayerWallet wallet,
            Inventory inventory)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Projectiles = projectiles ?? throw new ArgumentNullException(nameof(projectiles));
            Enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));
            Collision = collision ?? throw new ArgumentNullException(nameof(collision));
            Spawn = spawn ?? throw new ArgumentNullException(nameof(spawn));
            Waves = waves ?? throw new ArgumentNullException(nameof(waves));
            Spirits = spirits ?? throw new ArgumentNullException(nameof(spirits));
            Shop = shop ?? throw new ArgumentNullException(nameof(shop));
            Upgrades = upgrades ?? throw new ArgumentNullException(nameof(upgrades));
            Wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));

            var starter = EquipmentCatalog.Find("wpn_basic_wand");
            if (starter != null)
            {
                Inventory.Add(starter.Clone());
                Inventory.TryEquip(starter.Id);
            }

            Player = new Player(PlayerConstants.DefaultStartX, PlayerConstants.DefaultStartY);
            RefreshPlayerEquipmentStats();

            Player.OnAttackHitFrame += OnPlayerAttackHitFrame;
            Player.OnFireCastFrame += OnPlayerFireCastFrame;

            Waves.LoadStage(StageData.CreateEarthForest());
            Waves.OnStageCompleted += () => IsStageCompleted = true;
        }

        private void OnSlimeAttackHit(Slime slime)
        {
            if (Player.IsDead || Player.IsInvulnerable) return;
            if (Player.Bounds.IntersectsWith(slime.Bounds))
                Player.TakeDamage(slime.Damage);
        }

        public void SetPlayArea(float width, float height)
        {
            PlayArea = new RectangleF(0, 0, width, height);
            GroundY = PlayArea.Top + PlayArea.Height * GroundTopRatio;
            Spawn.SetSpawnArea(width, height);
            if (Player.Y + Player.Height > GroundY)
                Player.ResolveGroundCollision(GroundY);
        }

        public void Update(float deltaTime)
        {
            if (_statusMessageTimer > 0f)
            {
                _statusMessageTimer -= deltaTime;
                if (_statusMessageTimer <= 0f) StatusMessage = "";
            }
            if (IsPaused || IsStageCompleted) return;

            var (dirX, _) = Input.GetMovementDirection();

            Player.SetWantsToRun(Input.IsKeyDown(Keys.ShiftKey) ||
                                 Input.IsKeyDown(Keys.LShiftKey) ||
                                 Input.IsKeyDown(Keys.RShiftKey));

            if (!Player.IsAttacking && !Player.IsFiring && !Player.IsHurt && !Player.IsDead)
            {
                Player.MoveHorizontal(dirX, deltaTime);
            }
            else
            {
                if (Player.IsAttacking || Player.IsFiring)
                    Player.ApplyVelocityDamping(0.85f);
                else if (Player.IsHurt)
                    Player.ApplyVelocityDamping(0.9f);
            }

            bool jumpKeyNow = Input.IsJumpPressed();
            if (jumpKeyNow && !_jumpKeyWasPressed) Player.TryJump();
            _jumpKeyWasPressed = jumpKeyNow;

            bool attackKeyNow = Input.IsKeyDown(Keys.Space) || Input.IsKeyDown(Keys.J);
            if (attackKeyNow && !_attackKeyWasPressed) TryStartAttack();
            _attackKeyWasPressed = attackKeyNow;

            bool fireKeyNow = Input.IsKeyDown(Keys.F) || Input.IsKeyDown(Keys.K);
            if (fireKeyNow && !_fireKeyWasPressed) TryStartFire();
            _fireKeyWasPressed = fireKeyNow;

            Player.Update(deltaTime);
            Player.ResolveGroundCollision(GroundY);
            Player.ClampHorizontalBounds(PlayArea.Left, PlayArea.Right);

            Spirits.Update(deltaTime, Player);
            Projectiles.Update(deltaTime);

            // Truyền vị trí player cho slime (để quyết định Attack)
            float pcx = Player.X + Player.Width / 2f;
float pcy = Player.Y + Player.Height / 2f;

foreach (var e in Enemies.Enemies)
{
    if (e is Slime slime)
    {
        slime.SetCombatTarget(pcx, pcy);

        // Subscribe 1 lần (tránh subscribe mỗi frame)
        slime.OnAttackHit -= OnSlimeAttackHit;
        slime.OnAttackHit += OnSlimeAttackHit;
    }
}

Enemies.Update(deltaTime, GroundY, PlayArea.Left, PlayArea.Right);
            Collision.CheckCollisions(Projectiles, Enemies);
            CheckPlayerEnemyCollision();
            Waves.Update(deltaTime);
        }

        private void TryStartAttack()
        {
            if (!Player.CanAttack()) return;
            Player.StartAttack();
            Player.ResetAttackCooldown();
        }

        private void TryStartFire()
        {
            if (!Player.CanFire()) return;
            Player.StartFire();
            Player.ResetAttackCooldown();
        }

        private void OnPlayerAttackHitFrame() => SpawnPlayerProjectile();
        private void OnPlayerFireCastFrame() => SpawnFireballProjectile();

        private void SpawnPlayerProjectile()
        {
            float dir = Player.Facing == FacingDirection.Right ? 1f : -1f;
            float spawnX = Player.X + Player.Width / 2f + dir * 16f;
            float spawnY = Player.Y + Player.Height / 2f - 3f;
            Projectiles.Add(ProjectileFactory.CreatePlayerProjectile(spawnX, spawnY, dir, Player.Damage));

            if (Player.ActiveWindBarrage && Player.ExtraProjectiles > 0)
            {
                float spread = 18f;
                for (int i = 0; i < Player.ExtraProjectiles; i++)
                {
                    int pair = i / 2 + 1;
                    float offsetY = (i % 2 == 0) ? -spread * pair : spread * pair;
                    Projectiles.Add(ProjectileFactory.CreatePlayerProjectile(spawnX, spawnY + offsetY, dir, Player.Damage));
                }
            }
        }

        private void SpawnFireballProjectile()
        {
            float dir = Player.Facing == FacingDirection.Right ? 1f : -1f;
            float spawnX = Player.X + Player.Width / 2f + dir * 20f;
            float spawnY = Player.Y + Player.Height / 2f - 8f;
            int damage = Player.ActiveFireBoost ? (int)(Player.Damage * 1.5f) : Player.Damage;
            Projectiles.Add(ProjectileFactory.CreateFireball(spawnX, spawnY, dir, damage));
        }

        private void CheckPlayerEnemyCollision()
        {
            if (Player.IsDead || Player.IsInvulnerable) return;

            foreach (var e in Enemies.Enemies)
            {
                if (!e.IsAlive) continue;
                if (e is Slime) continue; 

                if (Player.Bounds.IntersectsWith(e.Bounds))
                {
                    Player.TakeDamage(e.Damage);
                    break;
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

        public void HandleKeyDown(Keys key)
        {
            Input.KeyDown(key);
            if (key is Keys.D1 or Keys.NumPad1) Spirits.TryActivate(0, Player);
            else if (key is Keys.D2 or Keys.NumPad2) Spirits.TryActivate(1, Player);
#if DEBUG
            if (key == Keys.N) Waves.ForceNextWave();
            if (key == Keys.D3) { Spirits.Equip(0, Spirits.Unlocked[0]); Spirits.Equip(1, Spirits.Unlocked[1]); SetStatus("Equipped: Terra + Ignis"); }
            else if (key == Keys.D4) { Spirits.Equip(0, Spirits.Unlocked[2]); Spirits.Equip(1, Spirits.Unlocked[3]); SetStatus("Equipped: Aqua + Zephyr"); }
            if (key == Keys.G) { Wallet.AddGold(100); Wallet.AddSpiritShards(10); SetStatus("+100 Gold, +10 Spirit Shards"); }
#endif
        }

        public void HandleKeyUp(Keys key) => Input.KeyUp(key);
    }
}