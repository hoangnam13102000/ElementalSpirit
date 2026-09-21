using System;
using System.Linq;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.AI;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Projectile;
using ElementalSpirit.Domain.Skill;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.Factories;
using ElementalSpirit.Presentation.Assets;
using System.Drawing;
using Xunit;

namespace ElementalSpirit.Tests;

public class BossStageTests
{
    [Fact]
    public void CreateEarthForestCampaign_IncludesFinalForestBossStage()
    {
        var stages = StageData.CreateEarthForestCampaign();

        Assert.Contains(stages, s => s.Name.Contains("Final Forest", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void EarthForest3_HasStairLikePlatforms_AndPitWalls()
    {
        var stage = StageData.CreateEarthForest3();

        Assert.True(stage.Platforms.Count >= 8, $"Expected stage 3 to contain multiple collision platforms but found {stage.Platforms.Count}.");
        Assert.Contains(stage.Platforms, p => p.Name == "LeftUpperStair");
        Assert.Contains(stage.Platforms, p => p.Name == "CenterBridge");
        Assert.Contains(stage.Walls, w => w.Name == "LeftPitWall");
        Assert.Contains(stage.Walls, w => w.Name == "RightPitWall");
    }

    [Fact]
    public void FinalBossStage_DoesNotSpawnGorgonInWaveList()
    {
        var stages = StageData.CreateEarthForestCampaign();
        var finalStage = stages.Last();

        Assert.All(finalStage.Waves.SelectMany(w => w.Spawns), spawn =>
            Assert.NotEqual(EnemyType.Gorgon, spawn.EnemyType));
    }

    [Fact]
    public void GorgonBoss_HasHighHealthForBossFight()
    {
        var boss = EnemyFactory.Create(EnemyType.Gorgon, 640f, 200f);

        Assert.Equal(1000, boss.MaxHealth);
        Assert.Equal(1000, boss.Health);
    }

    [Fact]
    public void GorgonBoss_AnimationUsesFrameSizedImages()
    {
        var controller = GorgonAnimationLoader.CreateController();
        var currentImage = controller.CurrentImage;

        Assert.NotNull(currentImage);
        Assert.True(currentImage.Width <= 128, $"Expected Gorgon animation frame width <= 128 but got {currentImage.Width}.");
        Assert.True(currentImage.Height <= 128, $"Expected Gorgon animation frame height <= 128 but got {currentImage.Height}.");
    }

    [Fact]
    public void Enemies_DoNotTakeDamageFromEnemyProjectiles()
    {
        var boss = new GorgonBoss(0f, 0f);
        var projectile = new EnemyProjectile(0f, 0f, 0f, 0f, 999, lifetime: 2f, radius: 80f);

        var projectiles = new ProjectileManager();
        projectiles.Add(projectile);

        var enemies = new EnemyManager();
        enemies.Add(boss);

        var collision = new CollisionManager();
        collision.CheckCollisions(projectiles, enemies);

        Assert.Equal(1000, boss.Health);
    }

    [Fact]
    public void GorgonNuclearExplosion_UsesSizedStraightLineProjectile()
    {
        var projectile = BossProjectileFactory.CreateGorgonNuclearExplosion(
            originX: 0f,
            originY: 0f,
            targetX: 300f,
            targetY: 0f,
            damage: 18);

        Assert.Equal(EnemyProjectileType.NuclearExplosion, projectile.Type);
        Assert.Equal(36, projectile.Width);
        Assert.Equal(36, projectile.Height);
        Assert.True(projectile.VelocityX > 0f);
        Assert.Equal(0f, projectile.VelocityY);
    }

    [Fact]
    public void GorgonBlueSpread_CreatesFiveProjectiles()
    {
        var projectiles = BossProjectileFactory.CreateGorgonSpread(
            originX: 100f,
            originY: 100f,
            targetX: 400f,
            targetY: 100f,
            damage: 18);

        Assert.Equal(5, projectiles.Length);
        Assert.All(projectiles, projectile =>
        {
            Assert.Equal(EnemyProjectileType.BlueOrb, projectile.Type);
            Assert.True(projectile.VelocityX > 0f);
        });
    }

    [Fact]
    public void GorgonBoss_ChasesPlayerBeyondOldDetectionRange()
    {
        var boss = new GorgonBoss(0f, 300f);
        var target = new TestEnemyTarget(1000f, 300f, 32, 32);

        boss.Update(1f, groundY: 470f, target);

        Assert.Equal(90f, boss.X);
        Assert.Equal(EnemyBehaviorState.Chase, boss.BehaviorState);
    }

    [Fact]
    public void GorgonBoss_KeepsMovingDuringSkillCooldown()
    {
        var boss = new GorgonBoss(0f, 300f);
        var target = new TestEnemyTarget(200f, 300f, 32, 32);

        boss.Update(0.1f, groundY: 470f, target);

        Assert.True(boss.X > 0f);
        Assert.Equal(EnemyBehaviorState.Chase, boss.BehaviorState);
    }

    [Fact]
    public void GorgonBoss_PausesAfterTakingDamage()
    {
        var boss = new GorgonBoss(0f, 300f);
        var target = new TestEnemyTarget(1000f, 300f, 32, 32);

        boss.TakeDamage(10);
        boss.Update(0.1f, groundY: 470f, target);

        Assert.True(boss.IsHurt);
        Assert.Equal(0f, boss.X);

        boss.Update(0.3f, groundY: 470f, target);
        boss.Update(0.1f, groundY: 470f, target);

        Assert.False(boss.IsHurt);
        Assert.True(boss.X > 0f);
    }

    [Fact]
    public void SlashSkill_CanBeActivatedByShortId()
    {
        var manager = new SkillManager(new TestProjectileLoadout());

        Assert.True(manager.TryActivate("slash"));
    }

    [Fact]
    public void SlashSkill_KeepsWindSlashIcon_WhenSelected()
    {
        var loadout = new TestProjectileLoadout();
        var manager = new SkillManager(loadout);

        Assert.True(manager.TryActivate("slash"));

        var slash = manager.Skills.Single(skill => skill.Id == "slash.wind");
        Assert.False(string.IsNullOrWhiteSpace(slash.IconAssetKey));
        Assert.Contains("WindSlash", slash.IconAssetKey, StringComparison.Ordinal);
        Assert.Equal(ProjectileType.Slash, loadout.CurrentProjectileType);

        Assert.True(manager.TryActivate("slash"));
        Assert.Equal(ProjectileType.Slash, loadout.CurrentProjectileType);
    }

    [Fact]
    public void PortalManager_ShowsForwardPortal_WhenEnemiesAreCleared_AndPlayerIsClose()
    {
        var manager = new PortalManager();
        manager.RebuildPortalsForStage(0, 2, 1280f, 720f, 540f);

        var playerBounds = new RectangleF(1160f, 420f, 30f, 30f);
        manager.UpdatePortalVisibility(allEnemiesCleared: true, playerBounds: playerBounds, playerFacing: FacingDirection.Right);

        var forwardPortal = manager.Portals.Single(p => p.Type == PortalType.ForwardPortal);
        Assert.True(forwardPortal.IsVisible);
    }

    [Fact]
    public void PortalManager_BackPortal_RemainsVisible_AtAllTimes()
    {
        var manager = new PortalManager();
        manager.RebuildPortalsForStage(1, 3, 1280f, 720f, 540f);

        var playerBounds = new RectangleF(1000f, 300f, 30f, 30f);
        manager.UpdatePortalVisibility(allEnemiesCleared: true, playerBounds: playerBounds, playerFacing: FacingDirection.Right);

        var backPortal = manager.Portals.Single(p => p.Type == PortalType.BackPortal);
        Assert.True(backPortal.IsVisible);
    }

    [Fact]
    public void PortalManager_OpensForwardPortal_AfterStageClear_WithoutProximityOrFacingRequirement()
    {
        var manager = new PortalManager();
        manager.RebuildPortalsForStage(1, 3, 1280f, 720f, 540f);

        var playerBounds = new RectangleF(100f, 300f, 30f, 30f);
        manager.UpdatePortalVisibility(allEnemiesCleared: true, playerBounds: playerBounds, playerFacing: FacingDirection.Left);

        var forwardPortal = manager.Portals.Single(p => p.Type == PortalType.ForwardPortal);
        Assert.True(forwardPortal.IsVisible);

        manager.UpdatePortalVisibility(allEnemiesCleared: true, playerBounds: playerBounds, playerFacing: FacingDirection.Right);
        Assert.True(forwardPortal.IsVisible);
    }

    [Fact]
    public void StoneGateAnimationLoader_LoadsFrames_FromSpriteSheet()
    {
        var controller = StoneGateAnimationLoader.CreateController();

        Assert.NotNull(controller.CurrentImage);
        Assert.True(controller.CurrentImage.Width > 0);
        Assert.True(controller.CurrentImage.Height > 0);

        controller.Dispose();
    }

    [Fact]
    public void AssetLoader_ReturnsIndependentCopies_ForRepeatedBackgroundLoads()
    {
        var first = AssetLoader.Get("EarthForest.png");
        var second = AssetLoader.Get("EarthForest.png");

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotSame(first, second);

        first!.Dispose();

        Assert.False(second!.Size.IsEmpty);
        Assert.True(second.Width > 0 && second.Height > 0);

        second.Dispose();
        AssetLoader.DisposeAll();
    }

    private sealed class TestProjectileLoadout : IProjectileLoadout
    {
        public ProjectileType CurrentProjectileType { get; private set; } = ProjectileType.Basic;

        public void SetProjectileType(ProjectileType projectileType) => CurrentProjectileType = projectileType;
    }

    private sealed class TestEnemyTarget : IEnemyTarget
    {
        public TestEnemyTarget(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X { get; }
        public float Y { get; }
        public int Width { get; }
        public int Height { get; }
        public bool IsAlive => true;
    }
}
