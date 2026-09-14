using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ElementalSpirit.Data;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.SaveData;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;
using ElementalSpirit.Services;
using ElementalSpirit.GameEngine.Abstractions;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.GameEngine
{
    /// <summary>
    /// Các pha của animation chuyển cảnh khi tiêu diệt hết quái ở 1 background
    /// và game tự động đưa nhân vật sang background kế tiếp trong chuỗi stage.
    /// </summary>
    public enum StageTransitionPhase
    {
        None,
        RunningOut,  // nhân vật tự chạy về mép phải màn hình hiện tại
        Fading,      // crossfade từ background cũ sang background mới
        RunningIn    // nhân vật chạy vào từ mép trái background mới
    }

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

        // Xu ly va cham nen nhieu tang (da, cau treo, khoang trong ...) cho Player,
        // dua tren TerrainPlatform cua stage hien tai thay vi 1 duong GroundY phang.
        private readonly TerrainCollisionResolver _terrainResolver = new();
        // Neu Player roi qua khoi day man hinh (vd: rot xuong nuoc giua khe vuc)
        // thi dua ve vi tri an toan thay vi roi mai mai ra ngoai tam nhin.
        private const float FallRecoveryMargin = 40f;

        public bool IsStageCompleted { get; private set; }
        public bool IsPaused { get; set; }
        public string StatusMessage { get; private set; } = "";
        private float _statusMessageTimer;
        private bool _jumpKeyWasPressed;
        private bool _attackKeyWasPressed;
        private bool _fireKeyWasPressed;

        // ---- Chuyển cảnh giữa các background (EarthForest -> EarthForest2 -> EarthForest3) ----
        private readonly List<StageData> _stageSequence;
        private int _stageIndex;
        private float _transitionTimer;

        private const float RunOutDuration = 0.6f;   // thời gian tối đa chạy ra mép phải
        private const float FadeDuration = 0.55f;    // thời gian crossfade giữa 2 background
        private const float RunInDuration = 0.6f;    // thời gian chạy vào từ mép trái

        public StageTransitionPhase TransitionPhase { get; private set; } = StageTransitionPhase.None;
        /// <summary>0..1 - dùng để renderer crossfade sang background kế tiếp.</summary>
        public float TransitionProgress { get; private set; }
        /// <summary>Tên file background sắp tới, chỉ có giá trị trong lúc Fading.</summary>
        public string? IncomingBackgroundImageName { get; private set; }
        public bool IsInStageTransition => TransitionPhase != StageTransitionPhase.None;
        public int StageIndex => _stageIndex;
        public int TotalStagesInCampaign => _stageSequence.Count;

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

            _stageSequence = StageData.CreateEarthForestCampaign();
            _stageIndex = 0;
            Waves.LoadStage(_stageSequence[_stageIndex]);
            Waves.OnStageCompleted += HandleStageCompleted;
        }

        /// <summary>
        /// Được gọi khi WaveManager báo hết wave của background hiện tại.
        /// Nếu còn background tiếp theo trong chuỗi -> bắt đầu animation chuyển cảnh.
        /// Nếu đây là background cuối cùng -> coi như hoàn thành toàn bộ campaign.
        /// </summary>
        private void HandleStageCompleted()
        {
            bool isLastStage = _stageIndex >= _stageSequence.Count - 1;
            if (isLastStage)
            {
                IsStageCompleted = true;
                return;
            }

            TransitionPhase = StageTransitionPhase.RunningOut;
            TransitionProgress = 0f;
            _transitionTimer = 0f;
            IncomingBackgroundImageName = _stageSequence[_stageIndex + 1].BackgroundImageName;
            Player.SetWantsToRun(true);
        }

        private void UpdateStageTransition(float deltaTime)
        {
            switch (TransitionPhase)
            {
                case StageTransitionPhase.RunningOut:
                    {
                        Player.MoveHorizontal(1f, deltaTime);
                        Player.Update(deltaTime);
                        Player.ResolveGroundCollision(GroundY);

                        _transitionTimer += deltaTime;
                        bool reachedEdge = Player.X + Player.Width >= PlayArea.Right - 2f;
                        if (reachedEdge || _transitionTimer >= RunOutDuration)
                        {
                            Player.MoveHorizontal(0f, deltaTime);
                            TransitionPhase = StageTransitionPhase.Fading;
                            _transitionTimer = 0f;
                            TransitionProgress = 0f;
                        }
                        break;
                    }

                case StageTransitionPhase.Fading:
                    {
                        _transitionTimer += deltaTime;
                        TransitionProgress = Math.Clamp(_transitionTimer / FadeDuration, 0f, 1f);
                        if (_transitionTimer >= FadeDuration)
                        {
                            AdvanceToNextStage();
                            TransitionPhase = StageTransitionPhase.RunningIn;
                            _transitionTimer = 0f;
                        }
                        break;
                    }

                case StageTransitionPhase.RunningIn:
                    {
                        Player.MoveHorizontal(1f, deltaTime);
                        Player.Update(deltaTime);
                        Player.ResolveGroundCollision(GroundY);
                        Player.ClampHorizontalBounds(PlayArea.Left, PlayArea.Right);

                        _transitionTimer += deltaTime;
                        bool reachedRestSpot = Player.X >= PlayerConstants.DefaultStartX;
                        if (reachedRestSpot || _transitionTimer >= RunInDuration)
                        {
                            Player.MoveHorizontal(0f, deltaTime);
                            Player.SetWantsToRun(false);
                            TransitionPhase = StageTransitionPhase.None;
                            TransitionProgress = 0f;
                            IncomingBackgroundImageName = null;
                        }
                        break;
                    }
            }
        }

        private void AdvanceToNextStage()
        {
            _stageIndex++;
            var nextStage = _stageSequence[_stageIndex];

            Enemies.Clear();
            Projectiles.Clear();
            Waves.LoadStage(nextStage);

            // Nhân vật xuất hiện ở mép trái của background mới, tiếp tục chạy vào giữa màn.
            Player.ResetPosition(PlayArea.Left + 8f, Player.Y);
        }

        public void SetPlayArea(float width, float height)
        {
            PlayArea = new RectangleF(0, 0, width, height);
            GroundY = PlayArea.Top + PlayArea.Height * GroundTopRatio;
            Spawn.SetSpawnArea(width, height);
            if (Player.Y + Player.Height > GroundY)
                Player.ResolveGroundCollision(GroundY);
        }
        /// <summary>
        /// Xac dinh nen (TerrainPlatform) phu hop ben duoi Player dua tren danh sach
        /// nen cua stage hien tai (co the co nhieu tang do cao va khoang trong),
        /// thay vi 1 duong GroundY phang duy nhat nhu truoc. Neu khong tim thay nen
        /// nao tai vi tri hien tai, Player se roi tu do (vd: buoc vao khoang nuoc
        /// giua 2 rieng da o EarthForest2).
        /// </summary>
        private void ResolvePlayerTerrain(float previousFootY)
        {
            var platforms = _stageSequence[_stageIndex].Platforms;
            float footX = Player.X + Player.Width / 2f;
            float currentFootY = Player.Y + Player.Height;

            if (_terrainResolver.TryGetSupportingGroundY(
                    platforms, PlayArea, footX, previousFootY, currentFootY, out float supportY))
            {
                Player.ResolveGroundCollision(supportY);
            }
            else
            {
                Player.LeaveGround();
            }
        }

        /// <summary>
        /// Neu Player roi qua khoi day vung choi (vd: rot xuong khoang nuoc giua khe vuc
        /// ma khong nhay len cau/da kip), dua nhan vat quay lai vi tri an toan dau man
        /// thay vi tiep tuc roi mai mai ra ngoai tam nhin.
        /// </summary>
        private void HandlePlayerFallRecovery()
        {
            if (Player.IsDead) return;
            if (Player.Y < PlayArea.Bottom + FallRecoveryMargin) return;

            Player.ResetPosition(PlayArea.Left + 8f, GroundY - Player.Height);
            SetStatus("Ban roi xuong nuoc! Quay lai vi tri an toan.");
        }

        private void OnSlimeAttackHit(Slime slime)
        {
            if (Player.IsDead || Player.IsInvulnerable) return;
            if (Player.Bounds.IntersectsWith(slime.Bounds))
                Player.TakeDamage(slime.Damage);
        }


        public void Update(float deltaTime)
        {
            if (_statusMessageTimer > 0f)
            {
                _statusMessageTimer -= deltaTime;
                if (_statusMessageTimer <= 0f) StatusMessage = "";
            }
            if (IsPaused) return;

            if (IsInStageTransition)
            {
                UpdateStageTransition(deltaTime);
                Projectiles.Update(deltaTime);
                return;
            }

            if (IsStageCompleted) return;

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

            float previousFootY = Player.Y + Player.Height;
            Player.Update(deltaTime);
            ResolvePlayerTerrain(previousFootY);
            Player.ClampHorizontalBounds(PlayArea.Left, PlayArea.Right);
            HandlePlayerFallRecovery();

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
                if (e is Slime) continue; // damage qua OnAttackHit

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

        /// <summary>
        /// Chụp lại toàn bộ tiến trình hiện tại (khu vực, HP, ví, trang bị, tinh linh)
        /// thành 1 snapshot dữ liệu thuần (GameSaveData) để ISaveGameService ghi ra file.
        /// Chỉ ĐỌC state hiện có qua các property public, không thay đổi gì trong lúc chơi.
        /// </summary>
        public GameSaveData CaptureSaveData()
        {
            var data = new GameSaveData
            {
                StageIndex = _stageIndex,
                StageName = _stageSequence[_stageIndex].Name,
                PlayerHp = Player.CurrentHp,
                Gold = Wallet.Gold,
                SpiritShards = Wallet.SpiritShards,
                Crystals = Wallet.Crystals,
                SavedAtUtc = DateTime.UtcNow
            };

            foreach (var item in Inventory.Items)
                data.OwnedEquipmentIds.Add(item.Id);

            data.EquippedWeaponId = Inventory.GetEquipped(EquipmentSlot.Weapon)?.Id;
            data.EquippedArmorId = Inventory.GetEquipped(EquipmentSlot.Armor)?.Id;
            data.EquippedAccessoryId = Inventory.GetEquipped(EquipmentSlot.Accessory)?.Id;

            foreach (var spirit in Spirits.Unlocked)
                data.Spirits.Add(new SpiritSaveData { Id = spirit.Id, Level = spirit.Level });

            data.EquippedSpiritSlot1Id = Spirits.Equipped.Count > 0 ? Spirits.Equipped[0]?.Id : null;
            data.EquippedSpiritSlot2Id = Spirits.Equipped.Count > 1 ? Spirits.Equipped[1]?.Id : null;

            return data;
        }

        /// <summary>
        /// Áp 1 snapshot GameSaveData đã đọc từ file lên GameManager vừa được khởi tạo
        /// (từ Program.CreateGameManager()), coi như checkpoint tại đầu khu vực đã lưu:
        /// nạp lại đúng khu vực (wave sẽ bắt đầu lại từ đầu khu vực đó), ví tiền, trang bị
        /// đang sở hữu/đang mặc, cấp độ + tinh linh đang trang bị, và HP của nhân vật.
        /// Chỉ dùng các API public đã có sẵn của từng lớp (Inventory.Add/TryEquip,
        /// SpiritManager.Equip, ISpirit.Upgrade, PlayerWallet.LoadFrom, Player.RestoreHp...)
        /// nên không thay đổi logic gameplay hiện có, chỉ tái sử dụng nó.
        /// </summary>
        public void ApplySaveData(GameSaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            int targetIndex = Math.Clamp(data.StageIndex, 0, _stageSequence.Count - 1);
            _stageIndex = targetIndex;
            TransitionPhase = StageTransitionPhase.None;
            IsStageCompleted = false;
            Enemies.Clear();
            Projectiles.Clear();
            Waves.LoadStage(_stageSequence[_stageIndex]);

            foreach (var itemId in data.OwnedEquipmentIds)
            {
                var template = EquipmentCatalog.Find(itemId);
                if (template != null)
                    Inventory.Add(template.Clone());
            }

            if (data.EquippedWeaponId != null) Inventory.TryEquip(data.EquippedWeaponId);
            if (data.EquippedArmorId != null) Inventory.TryEquip(data.EquippedArmorId);
            if (data.EquippedAccessoryId != null) Inventory.TryEquip(data.EquippedAccessoryId);
            RefreshPlayerEquipmentStats();

            Wallet.LoadFrom(data.Gold, data.SpiritShards, data.Crystals);

            foreach (var savedSpirit in data.Spirits)
            {
                var spirit = Spirits.Unlocked.FirstOrDefault(s => s.Id == savedSpirit.Id);
                if (spirit == null) continue;

                // Goi Upgrade() nhieu lan qua dung API cong khai san co thay vi gan
                // thang Level (dang co protected set). Dung vong lap co kiem tra tien
                // trien de tranh treo vo han neu file save bi sua tay voi Level vuot
                // qua muc toi da noi bo cua SpiritBase.
                while (spirit.Level < savedSpirit.Level)
                {
                    int levelBefore = spirit.Level;
                    spirit.Upgrade();
                    if (spirit.Level == levelBefore) break; // da dat cap toi da, khong the nang them
                }
            }

            if (data.EquippedSpiritSlot1Id != null)
            {
                var slot1 = Spirits.Unlocked.FirstOrDefault(s => s.Id == data.EquippedSpiritSlot1Id);
                if (slot1 != null) Spirits.Equip(0, slot1);
            }
            if (data.EquippedSpiritSlot2Id != null)
            {
                var slot2 = Spirits.Unlocked.FirstOrDefault(s => s.Id == data.EquippedSpiritSlot2Id);
                if (slot2 != null) Spirits.Equip(1, slot2);
            }

            Player.ResetPosition(PlayerConstants.DefaultStartX, PlayerConstants.DefaultStartY);
            Player.RestoreHp(data.PlayerHp);
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