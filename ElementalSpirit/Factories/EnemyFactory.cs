using System;
using ElementalSpirit.Domain.Enemy;
using ElementalSpirit.Domain.Enemy.NormalEnemy;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Factories
{
    public enum EnemyType
    {
        Slime,
        Gorgon
    }

    public static class EnemyFactory
    {
        public static Enemy Create(EnemyType type, float x, float y)
        {
            return type switch
            {
                EnemyType.Slime => CreateSlime(x, y),
                EnemyType.Gorgon => CreateGorgon(x, y),
                _ => throw new ArgumentException($"Unknown enemy type: {type}")
            };
        }

        private static Slime CreateSlime(float x, float y)
        {
            SlimeAnimationController? controller = null;
            try
            {
                controller = SlimeAnimationLoader.CreateController();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EnemyFactory] Slime anim: {ex.Message}");
            }

            var slime = new Slime(x, y, controller);
            return slime;
        }

        private static GorgonBoss CreateGorgon(float x, float y)
        {
            GorgonBossAnimationController? controller = null;
            try
            {
                controller = GorgonAnimationLoader.CreateController();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EnemyFactory] Gorgon anim: {ex.Message}");
            }

            return new GorgonBoss(x, y, controller);
        }
    }
}