using ElementalSpirit.Domain.Loot;
using ElementalSpirit.Data;
using ElementalSpirit.Domain.BossEncounter;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.Domain.SaveData;
using ElementalSpirit.Domain.Skill;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;
using ElementalSpirit.GameEngine.Abstractions;
using ElementalSpirit.Presentation.Forms;
using ElementalSpirit.Services;
using ElementalSpirit.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        public ISkillManager Skills { get; }
        public PlayerWallet Wallet { get; }
        public Inventory Inventory { get; }
        public IUpgradeService Upgrades { get; }
        public IShopService Shop { get; }
        public IBossEncounterManager BossEncounter { get; }
        public IReadOnlyList<LootDrop> Loot => _loot;
        public IPortalManager Portals { get; }

        public RectangleF PlayArea { get; private set; }
        // Mặt đất chính của stage hiện tại (theo StageData.GroundTopRatio), được cập nhật mỗi khi
        // đổi stage hoặc đổi kích thước vùng chơi - xem RefreshGroundY().
        public float GroundY { get; private set; }

        // Xu ly va cham nen nhieu tang (da, cau treo, khoang trong ...) cho Player,
        // dua tren TerrainPlatform cua stage hien tai thay vi 1 duong GroundY phang.
        private readonly TerrainCollisionResolver _terrainResolver = new();

        // Va chạm ngang của Player với thành lỗ / vách (TerrainWall.BlocksPlayer).
        private readonly TerrainWallResolver _wallResolver = new();

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

        // Flag đánh dấu stage boss cuối đã kích hoạt encounter
        private bool _bossEncounterTriggered;
        private readonly List<LootDrop> _loot = new();
        private readonly Random _random = new();

        // ---- Chuyển cảnh giữa các background (EarthForest -> EarthForest2 -> EarthForest3) ----
        private readonly List<StageData> _stageSequence;
        private readonly HashSet<int> _clearedStages = new();
        private int _stageIndex;
        private float _transitionTimer;
        private const float RunOutDuration = 0.6f;   // thời gian tối đa chạy ra mép phải
        private const float FadeDuration = 0.55f;    // thời gian crossfade giữa 2 background
        private const float RunInDuration = 0.6f;    // thời gian chạy vào từ mép trái
        private GameForm? _gameForm;

        public void SetGameForm(GameForm gameForm)
        {
            _gameForm = gameForm;
        }

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
            Player player,
            ISkillManager skills,
            IShopService shop,
            IUpgradeService upgrades,
            PlayerWallet wallet,
            Inventory inventory,
            IBossEncounterManager bossEncounter,
            IPortalManager portals)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Projectiles = projectiles ?? throw new ArgumentNullException(nameof(projectiles));
            Enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));
            Collision = collision ?? throw new ArgumentNullException(nameof(collision));
            Spawn = spawn ?? throw new ArgumentNullException(nameof(spawn));
            Waves = waves ?? throw new ArgumentNullException(nameof(waves));
            Spirits = spirits ?? throw new ArgumentNullException(nameof(spirits));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Skills = skills ?? throw new ArgumentNullException(nameof(skills));
            Shop = shop ?? throw new ArgumentNullException(nameof(shop));
            Upgrades = upgrades ?? throw new ArgumentNullException(nameof(upgrades));
            Wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            BossEncounter = bossEncounter ?? throw new ArgumentNullException(nameof(bossEncounter));
            Portals = portals ?? throw new ArgumentNullException(nameof(portals));

            var starter = EquipmentCatalog.Find("wpn_basic_wand");
            if (starter != null)
            {
                Inventory.Add(starter.Clone());
                Inventory.TryEquip(starter.Id);
            }

            RefreshPlayerEquipmentStats();

            Player.OnAttackHitFrame += OnPlayerAttackHitFrame;
            Player.OnFireCastFrame += OnPlayerFireCastFrame;
            Player.OnDeath += OnPlayerDeath;

            _stageSequence = StageData.CreateEarthForestCampaign();
            _stageIndex = 0;
            Waves.LoadStage(_stageSequence[_stageIndex]);
            Waves.OnStageCompleted += HandleStageCompleted;

            // Đăng ký lắng nghe sự kiện boss encounter
            BossEncounter.OnPreBossDialogueStarted += OnPreBossDialogueStarted;
            BossEncounter.OnBossEncounterCompleted += OnBossEncounterCompleted;

            // Đăng ký lắng nghe sự kiện cổng di chuyển
            Portals.OnPortalTriggered += OnPortalTriggered;

        }

        public void Dispose()
        {

        }

        private void HandleStageCompleted()
        {
            _clearedStages.Add(_stageIndex);

            bool isLastStage = _stageIndex >= _stageSequence.Count - 1;

            if (isLastStage)
            {
                IsStageCompleted = true;
                Services.AudioManager.Instance.PlaySfx("win.mp3");
                return;

            }
            // Không tự động chuyển cảnh nữa.
            // Cổng ForwardPortal sẽ được Portals.UpdatePortalVisibility() hiện ra
            // khi stage này đã từng clear, kể cả khi người chơi quay lại màn cũ.
            SetStatus("Đã tiêu diệt hết quái! Cổng di chuyển đã mở.");

        }

        private void UpdateStageTransition(float deltaTime)
        {
            // Ẩn cổng trong lúc chuyển cảnh
            Portals.HideAll();

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

                            // Khôi phục hiển thị cổng sau khi chuyển cảnh xong
                            bool allCleared = Enemies.Enemies.Count == 0 &&
                                              Waves.State == WaveState.WaitingForClear;
                            bool stagePreviouslyCleared = _clearedStages.Contains(_stageIndex);
                            Portals.UpdatePortalVisibility(allCleared || stagePreviouslyCleared, Player.Bounds, Player.Facing);
                        }
                        else
                        {
                            // Không cập nhật Waves cho stage cuối (tránh spawn boss sớm)
                            bool isLastStage = _stageIndex >= _stageSequence.Count - 1;
                            if (!isLastStage)
                            {
                                Waves.Update(deltaTime);
                            }
                        }
                        break;
                    }
            }
        }

        private void AdvanceToNextStage()
        {
            TransitionToStage(_stageIndex + 1, PortalType.ForwardPortal);
        }

        /// <summary>
        /// Xử lý khi người chơi đi vào cổng di chuyển.
        /// Dựa vào loại cổng và stage đích để thực hiện chuyển stage.
        /// </summary>
        private void OnPortalTriggered(Portal portal)
        {
            if (IsInStageTransition) return;
            if (IsStageCompleted) return;
            if (portal.TargetStageIndex < 0 || portal.TargetStageIndex >= _stageSequence.Count) return;

            // Cổng forward chỉ mở khi stage đã clear hoặc đã từng clear trước đó, dù người chơi đã quay lại màn cũ.
            bool currentStageCleared = Enemies.Enemies.Count == 0 &&
                (Waves.State == WaveState.WaitingForClear || Waves.State == WaveState.StageCompleted);
            bool stageWasPreviouslyCleared = _clearedStages.Contains(_stageIndex);
            if (portal.Type == PortalType.ForwardPortal && !currentStageCleared && !stageWasPreviouslyCleared) return;

            TransitionToStage(portal.TargetStageIndex, portal.Type);
        }

        /// <summary>
        /// Chuyển sang stage chỉ định và đặt vị trí người chơi phù hợp.
        /// - BackPortal: xuất hiện ở mép phải, hướng sang trái
        /// - ForwardPortal: xuất hiện ở mép trái, hướng sang phải
        /// </summary>
        private void TransitionToStage(int targetStageIndex, PortalType portalType)
        {
            if (targetStageIndex < 0 || targetStageIndex >= _stageSequence.Count) return;

            _stageIndex = targetStageIndex;
            RefreshGroundY();
            var targetStage = _stageSequence[_stageIndex];

            Enemies.Clear();
            _loot.Clear();
            Projectiles.Clear();
            Waves.LoadStage(targetStage);
            Spawn.SetTerrain(targetStage.Platforms, PlayArea);
            Spawn.SetSpawnSafetyTarget(Player, 220f);
            Enemies.SetTerrain(targetStage.Platforms, PlayArea);
            Enemies.SetWalls(targetStage.Walls, PlayArea);

            // Xây dựng lại cổng cho stage mới
            Portals.RebuildPortalsForStage(
                _stageIndex,
                _stageSequence.Count,
                PlayArea.Width,
                PlayArea.Height,
                GroundY);

            // Đặt vị trí người chơi dựa vào hướng đi
            if (portalType == PortalType.BackPortal)
            {
                // Đi về màn trước → xuất hiện ở mép phải, hướng sang trái
                Player.ResetPosition(PlayArea.Right - Player.Width - 90f, GroundY - Player.Height);
                Player.SetFacing(FacingDirection.Left);
            }
            else
            {
                // Đi qua màn sau → xuất hiện ở mép trái, hướng sang phải
                Player.ResetPosition(PlayArea.Left + 90f, GroundY - Player.Height);
                Player.SetFacing(FacingDirection.Right);
            }

            Player.ResolveGroundCollision(GroundY);

            // Nếu stage này đã từng clear trước đó, cổng forward phải được giữ lại khi quay lại màn cũ.
            bool stagePreviouslyCleared = _clearedStages.Contains(_stageIndex);
            Portals.UpdatePortalVisibility(
                stagePreviouslyCleared || Enemies.Enemies.Count == 0 &&
                (Waves.State == WaveState.WaitingForClear || Waves.State == WaveState.StageCompleted),
                Player.Bounds,
                Player.Facing);

            // Nếu là stage cuối, kích hoạt boss encounter
            bool isLastStage = _stageIndex >= _stageSequence.Count - 1;
            if (isLastStage && !_bossEncounterTriggered)
            {
                _bossEncounterTriggered = true;
                BossEncounter.StartEncounter();
            }

            SetStatus($"Đã đến: {targetStage.Name}");
        }

        public void SetPlayArea(float width, float height)
        {
            PlayArea = new RectangleF(0, 0, width, height);
            RefreshGroundY();
            Spawn.SetSpawnArea(width, height);
            Spawn.SetTerrain(_stageSequence[_stageIndex].Platforms, PlayArea);
            Spawn.SetSpawnSafetyTarget(Player, 220f);
            Enemies.SetTerrain(_stageSequence[_stageIndex].Platforms, PlayArea);
            Enemies.SetWalls(_stageSequence[_stageIndex].Walls, PlayArea);

            // Xây dựng cổng cho stage hiện tại
            Portals.RebuildPortalsForStage(
                _stageIndex,
                _stageSequence.Count,
                width,
                height,
                GroundY);

            if (Player.Y + Player.Height > GroundY)
                Player.ResolveGroundCollision(GroundY);
        }

        /// <summary>
        /// Tính lại GroundY theo stage hiện tại. Mỗi stage có mặt đất riêng (StageData.GroundTopRatio),
        /// nên mọi chỗ dùng GroundY (điểm xuất hiện của Player, cổng, mặt đất dự phòng của quái...)
        /// đều tự khớp với mặt đất thật của stage đó.
        /// </summary>
        private void RefreshGroundY()
        {
            GroundY = PlayArea.Top + PlayArea.Height * _stageSequence[_stageIndex].GroundTopRatio;
        }

        /// <summary>
        /// Chặn Player đi xuyên ngang qua thành lỗ (TerrainWall.BlocksPlayer) khi đang ở dưới mặt đất,
        /// vd: đang đứng dưới đáy lỗ ở màn 3 thì phải nhảy lên khỏi mép chứ không thể đi xuyên qua nền đất.
        /// </summary>
        private void ResolvePlayerWalls(float previousX)
        {
            var walls = _stageSequence[_stageIndex].Walls;
            if (walls.Count == 0) return;

            float resolvedX = _wallResolver.ResolvePlayerHorizontalPosition(
                Player.Bounds, previousX, walls, PlayArea);

            if (resolvedX != Player.X)
                Player.RestoreHorizontalPosition(resolvedX);
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
            Player.ResolveGroundCollision(GroundY);
            SetStatus("Ban roi xuong nuoc! Quay lai vi tri an toan.");
        }

        private void OnSlimeAttackHit(Slime slime)
        {
            if (Player.IsDead || Player.IsInvulnerable) return;
            if (slime.CanHitTarget(Player))
            {
                Player.TakeDamage(slime.Damage);
                Services.AudioManager.Instance.PlaySfx("hurt.mp3");
            }
        }

        private void OnBossProjectileCast(GorgonBoss boss)
        {
            if (Player.IsDead || Player.IsInvulnerable) return;

            float targetX = Player.X + Player.Width / 2f;
            float targetY = Player.Y + Player.Height / 2f;

            if (boss.CurrentSkill == GorgonBossSkill.NuclearExplosion)
            {
                float groundOriginX = boss.X + boss.Width / 2f;
                float groundY = GroundY - 36f;
                Projectiles.Add(BossProjectileFactory.CreateGorgonNuclearExplosion(
                    groundOriginX,
                    groundY,
                    targetX,
                    groundY,
                    boss.Damage));
                return;
            }

            float mouthX = boss.X + boss.Width * (boss.Facing == FacingDirection.Right ? 0.82f : 0.18f);
            float mouthY = boss.Y + boss.Height * 0.38f;

            foreach (var projectile in BossProjectileFactory.CreateGorgonSpread(
                         mouthX,
                         mouthY,
                         targetX,
                         targetY,
                         boss.Damage))
            {
                Projectiles.Add(projectile);
            }
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

            // Kiểm tra kích hoạt màn boss - đưa lên đầu trước khi cập nhật bất cứ thứ gì
            TryTriggerBossEncounter();

            // Nếu đang trong đoạn thoại boss, không cập nhật Waves
            bool inBossDialogue = BossEncounter.CurrentState == BossEncounterState.PreBossDialogue ||
                                  BossEncounter.CurrentState == BossEncounterState.PostBossDialogue ||
                                  BossEncounter.CurrentState == BossEncounterState.SpiritRescue;

            bool combatEnabled = BossEncounter.CurrentState == BossEncounterState.NotStarted ||
                                 BossEncounter.CurrentState == BossEncounterState.BossFight;

            if (combatEnabled)
            {
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
                if (jumpKeyNow && !_jumpKeyWasPressed && Player.TryJump()) 
                    Services.AudioManager.Instance.PlaySfx("jump.mp3");
                _jumpKeyWasPressed = jumpKeyNow;

                bool attackKeyNow = Input.IsKeyDown(Keys.Space) || Input.IsKeyDown(Keys.J);
                if (attackKeyNow && !_attackKeyWasPressed) TryStartAttack();
                _attackKeyWasPressed = attackKeyNow;

                bool fireKeyNow = Input.IsKeyDown(Keys.F) || Input.IsKeyDown(Keys.K);
                if (fireKeyNow && !_fireKeyWasPressed) TryStartFire();
                _fireKeyWasPressed = fireKeyNow;

                float previousFootY = Player.Y + Player.Height;
                float previousX = Player.X;
                Player.Update(deltaTime);
                ResolvePlayerWalls(previousX);
                ResolvePlayerTerrain(previousFootY);
                Player.ClampHorizontalBounds(PlayArea.Left, PlayArea.Right);
                HandlePlayerFallRecovery();
                CheckLootPickup();

                Spirits.Update(deltaTime, Player);
                Skills.Update(deltaTime);
                Projectiles.Update(deltaTime);

                foreach (var e in Enemies.Enemies)
                {
                    switch (e)
                    {
                        case Slime slime:
                            slime.OnAttackHit -= OnSlimeAttackHit;
                            slime.OnAttackHit += OnSlimeAttackHit;
                            break;
                        case GorgonBoss boss:
                            boss.OnBossProjectileCast -= OnBossProjectileCast;
                            boss.OnBossProjectileCast += OnBossProjectileCast;
                            break;
                    }
                    e.OnDied -= OnEnemyDied;
                    e.OnDied += OnEnemyDied;
                }

                Enemies.Update(deltaTime, GroundY, PlayArea.Left, PlayArea.Right, Player);
                Collision.CheckCollisions(Projectiles, Enemies);
                CheckEnemyProjectileCollision();
                CheckPlayerEnemyCollision();

                // Chỉ cập nhật Waves khi không đang trong đoạn thoại boss
                if (!inBossDialogue)
                {
                    Waves.Update(deltaTime);
                }

                // Cập nhật trạng thái hiển thị cổng dựa trên điều kiện hết quái hoặc stage đã từng clear trước đó.
                bool allEnemiesCleared = Enemies.Enemies.Count == 0 &&
                                         (Waves.State == WaveState.WaitingForClear || Waves.State == WaveState.StageCompleted);
                bool stagePreviouslyCleared = _clearedStages.Contains(_stageIndex);
                Portals.UpdatePortalVisibility(allEnemiesCleared || stagePreviouslyCleared, Player.Bounds, Player.Facing);

                // Kiểm tra người chơi có đi vào cổng không
                Portals.CheckPlayerInteraction(Player.Bounds);
            }
            else
            {
                // Khi đang thoại boss, ẩn cổng đi
                Portals.HideAll();

                // Không để phím dùng để kết thúc thoại tự động kích hoạt kỹ năng/đòn đánh.
                _jumpKeyWasPressed = Input.IsJumpPressed();
                _attackKeyWasPressed = Input.IsKeyDown(Keys.Space) || Input.IsKeyDown(Keys.J);
                _fireKeyWasPressed = Input.IsKeyDown(Keys.F) || Input.IsKeyDown(Keys.K);
            }

            // Cập nhật BossEncounterManager
            BossEncounter.Update(deltaTime);

            if (BossEncounter.ActiveSpeechBubble.IsVisible && BossEncounter.CurrentBoss != null)
            {
                float bossCenterX = BossEncounter.CurrentBoss.X + BossEncounter.CurrentBoss.Width / 2f;
                float bossCenterY = BossEncounter.CurrentBoss.Y;
                BossEncounter.ActiveSpeechBubble.SetPositionFromAnchor(bossCenterX, bossCenterY);
            }

            if (BossEncounter.PlayerSpeechBubble.IsVisible)
            {
                float playerCenterX = Player.X + Player.Width / 2f;
                float playerCenterY = Player.Y;
                BossEncounter.PlayerSpeechBubble.SetPositionFromAnchor(playerCenterX, playerCenterY);
            }
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
        private void OnPlayerDeath() => Services.AudioManager.Instance.PlaySfx("lose.mp3");
        private void OnEnemyDied(Domain.Enemy.Enemy enemy) 
        { 
            float dropX = enemy.X + enemy.Width / 2f - 9f; 
            float dropY = enemy.Y + enemy.Height - 18f; 
            if (enemy is GorgonBoss) { _loot.Add(LootDrop.CreateGold(dropX, dropY, 100));
            } else {
                _loot.Add(LootDrop.CreateGold(dropX, dropY, 10));
                if (_random.NextDouble() < 0.30) 
                { 
                    _loot.Add(LootDrop.CreateEquipment(dropX + 16f, dropY, "acc_swift_boots"));
                }
            } 
        }

        private void SpawnPlayerProjectile()
        {
            float dir = Player.Facing == FacingDirection.Right ? 1f : -1f;
            float spawnX = Player.X + Player.Width / 2f + dir * 16f;
            float spawnY = Player.Y + Player.Height / 2f - 3f;

            Projectiles.Add(ProjectileFactory.CreatePlayerProjectile(
                spawnX, spawnY, dir, Player.Damage, Player.CurrentProjectileType));

            // ✅ Gom chung logic phát âm thanh theo loại đạn vào 1 chỗ, tách biệt hẳn khỏi WindBarrage
            if (Player.CurrentProjectileType == ProjectileType.Slash)
            {
                Services.AudioManager.Instance.PlaySfx("slash.mp3");
            }
            else if (Player.CurrentProjectileType == ProjectileType.Basic)
            {
                Services.AudioManager.Instance.PlaySfx("shot.mp3");
            }

            // ✅ WindBarrage xử lý riêng, không liên quan gì đến việc chọn âm thanh
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
                    Services.AudioManager.Instance.PlaySfx("hurt.mp3");
                    break;
                }
            }
        }

        private void CheckEnemyProjectileCollision()
        {
            if (Player.IsDead || Player.IsInvulnerable) return;

            foreach (var projectile in Projectiles.Projectiles)
            {
                if (!projectile.IsAlive || projectile is not EnemyProjectile) continue;

                if (Player.Bounds.IntersectsWith(projectile.Bounds))
                {
                    Player.TakeDamage(projectile.Damage);
                    Services.AudioManager.Instance.PlaySfx("hurt.mp3");
                    projectile.Kill();
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
            if (Enemies.Enemies.Count == 0 &&
       (Waves.State == WaveState.WaitingForClear || Waves.State == WaveState.StageCompleted))
            {
                if (!_clearedStages.Contains(_stageIndex))
                {
                    _clearedStages.Add(_stageIndex);
                }
            }

            var data = new GameSaveData
            {
                StageIndex = _stageIndex,
                StageName = _stageSequence[_stageIndex].Name,
                PlayerHp = Player.CurrentHp,
                Gold = Wallet.Gold,
                SpiritShards = Wallet.SpiritShards,
                Crystals = Wallet.Crystals,
                SavedAtUtc = DateTime.UtcNow,
                RemainingEnemyCount = Enemies.Enemies.Count
            };

            foreach (var item in Inventory.Items)
                data.OwnedEquipmentIds.Add(item.Id);
            data.ClearedStages = _clearedStages.ToList();

            data.EquippedWeaponId = Inventory.GetEquipped(EquipmentSlot.Weapon)?.Id;
            data.EquippedArmorId = Inventory.GetEquipped(EquipmentSlot.Armor)?.Id;
            data.EquippedAccessoryId = Inventory.GetEquipped(EquipmentSlot.Accessory)?.Id;

            foreach (var spirit in Spirits.Unlocked)
                data.Spirits.Add(new SpiritSaveData { Id = spirit.Id, Level = spirit.Level });

            data.EquippedSpiritSlot1Id = Spirits.Equipped.Count > 0 ? Spirits.Equipped[0]?.Id : null;
            data.EquippedSpiritSlot2Id = Spirits.Equipped.Count > 1 ? Spirits.Equipped[1]?.Id : null;

            return data;
        }

        public void ApplySaveData(GameSaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            int targetIndex = Math.Clamp(data.StageIndex, 0, _stageSequence.Count - 1);
            _stageIndex = targetIndex;
            _clearedStages.Clear(); 
            foreach (var idx in data.ClearedStages) 
            _clearedStages.Add(idx);
            RefreshGroundY();

            TransitionPhase = StageTransitionPhase.None;
            IsStageCompleted = false;
            Enemies.Clear();
            _loot.Clear();
            Projectiles.Clear();
            Waves.LoadStage(_stageSequence[_stageIndex]);

            if (data.RemainingEnemyCount >= 0) 
            { 
                Enemies.Clear(); if (data.RemainingEnemyCount == 0)
                { 
                    Waves.RestoreState(WaveState.StageCompleted); 
                } else {
                    Spawn.Spawn(new SpawnData(EnemyType.Slime, data.RemainingEnemyCount, 0.1f));
                    Waves.RestoreState(WaveState.WaitingForClear); 
                }
            }

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

                while (spirit.Level < savedSpirit.Level)
                {
                    int levelBefore = spirit.Level;
                    spirit.Upgrade();
                    if (spirit.Level == levelBefore) break;
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

            // Xây dựng lại cổng cho stage được load từ save
            if (PlayArea.Width > 0 && PlayArea.Height > 0)
            {
                Portals.RebuildPortalsForStage(
                    _stageIndex,
                    _stageSequence.Count,
                    PlayArea.Width,
                    PlayArea.Height,
                    GroundY);
            }
        }

        private void SetStatus(string message)
        {
            StatusMessage = message;
            _statusMessageTimer = 2.5f;
        }

        public void HandleKeyDown(Keys key)
        {
            Input.KeyDown(key);

            if (key is Keys.Space or Keys.Enter)
            {
                if (BossEncounter.CurrentState == BossEncounterState.PreBossDialogue ||
                    BossEncounter.CurrentState == BossEncounterState.PostBossDialogue ||
                    BossEncounter.CurrentState == BossEncounterState.SpiritRescue)
                {
                    BossEncounter.AdvanceDialogue();
                    return;
                }
                if (IsBossDialogueActive()) return;
            }

            if (key == Keys.Escape)
            {
                if (BossEncounter.CurrentState == BossEncounterState.PreBossDialogue ||
                    BossEncounter.CurrentState == BossEncounterState.PostBossDialogue ||
                    BossEncounter.CurrentState == BossEncounterState.SpiritRescue)
                {
                    BossEncounter.SkipDialogue();
                    return;
                }
            }

            if (IsBossDialogueActive()) return;

            if (key is Keys.D1 or Keys.NumPad1)
                ActivateSkill("slash");
            else if (key is Keys.D2 or Keys.NumPad2)
            {
                if (ActivateSkill("waterfall"))
                    ApplySkillDamage();
            }

#if DEBUG
            if (key == Keys.N) Waves.ForceNextWave();
            if (key == Keys.D3) { Spirits.Equip(0, Spirits.Unlocked[0]); Spirits.Equip(1, Spirits.Unlocked[1]); SetStatus("Equipped: Terra + Ignis"); }
            else if (key == Keys.D4) { Spirits.Equip(0, Spirits.Unlocked[2]); Spirits.Equip(1, Spirits.Unlocked[3]); SetStatus("Equipped: Aqua + Zephyr"); }
            if (key == Keys.G) { Wallet.AddGold(100); Wallet.AddSpiritShards(10); SetStatus("+100 Gold, +10 Spirit Shards"); }
#endif
        }

        public void HandleKeyUp(Keys key) => Input.KeyUp(key);

        private bool ActivateSkill(string skillId)
        {
            if (!Skills.TryActivate(skillId)) return false;
            Player.StartSkillCast();

            if (skillId == "waterfall")
                Services.AudioManager.Instance.PlaySfx("waterfall.mp3");

            return true;
        }

        private void ApplySkillDamage()
        {
            float direction = Player.Facing == FacingDirection.Right ? 1f : -1f;
            float originX = Player.X + Player.Width / 2f;
            float originY = Player.Y + Player.Height;

            var damageAreas = Skills.CreateDamageAreas(
                originX,
                originY,
                direction,
                Player.Damage);

            foreach (var enemy in Enemies.Enemies)
            {
                if (!enemy.IsAlive) continue;

                foreach (var damageArea in damageAreas)
                {
                    if (damageArea.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        enemy.TakeDamage(damageArea.Damage);
                        break;
                    }
                }
            }
        }

        private void OnPreBossDialogueStarted(BossDialogueScene _)
        {
            // Boss phải xuất hiện cùng người chơi trong lúc đọc thoại,
            // nhưng chưa được cập nhật cho đến khi chuyển sang BossFight.
            Player.ResetPosition(Player.X, GroundY - Player.Height);
            Player.ResolveGroundCollision(GroundY);

            foreach (var enemy in Enemies.Enemies)
            {
                if (enemy is GorgonBoss) return;
            }

            var boss = Spawn.SpawnSingle(
                EnemyType.Gorgon,
                PlayArea.Right - 250f,
                GroundY - 170f);

            if (boss is GorgonBoss gorgon)
            {
                BossEncounter.RegisterBoss(gorgon);
            }
        }

        private bool IsBossDialogueActive()
        {
            return BossEncounter.CurrentState == BossEncounterState.PreBossDialogue ||
                   BossEncounter.CurrentState == BossEncounterState.PostBossDialogue ||
                   BossEncounter.CurrentState == BossEncounterState.SpiritRescue;
        }

        /// <summary>
        /// Khi hoàn thành màn boss: đánh dấu stage hoàn thành.
        /// </summary>
        private void OnBossEncounterCompleted()
        {
            IsStageCompleted = true;
            Services.AudioManager.Instance.PlaySfx("win.mp3");
        }

        private void TryTriggerBossEncounter()
        {
            if (_bossEncounterTriggered) return;
            if (BossEncounter.CurrentState != BossEncounterState.NotStarted) return;

            bool isBossStage = _stageIndex == _stageSequence.Count - 1;
            bool waveReadyToStart = Waves.State == WaveState.WaitingToStart;

            if (isBossStage && waveReadyToStart)
            {
                _bossEncounterTriggered = true;
                Enemies.Clear();
                _loot.Clear();
                Projectiles.Clear();
                BossEncounter.StartEncounter();
            }
        }

        private void CheckLootPickup() 
        { 
            for (int i = _loot.Count - 1; i >= 0; i--) 
            { 
                var loot = _loot[i];
                if (!Player.Bounds.IntersectsWith(loot.Bounds)) continue; 
                if (loot.Type == LootType.Gold) 
                {
                    Wallet.AddGold(loot.GoldAmount);
                    SetStatus($"+{loot.GoldAmount} vàng");
                } else if (loot.Type == LootType.Equipment && loot.EquipmentId != null) 
                { 
                    var template = Data.EquipmentCatalog.Find(loot.EquipmentId); 
                    if (template != null) 
                    {
                        Inventory.Add(template.Clone());
                        SetStatus($"Nhặt được: {template.Name}");
                    } 
                }
                _loot.RemoveAt(i);
            }
        }

    }
}