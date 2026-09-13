using System;
using System.Windows.Forms;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Services;
using ElementalSpirit.Presentation.Forms;

namespace ElementalSpirit
{
    static class Program
    {
        public static GameManager CreateGameManager()
        {
            var input = new InputManager();
            var projectiles = new ProjectileManager();
            var enemies = new EnemyManager();
            var collision = new CollisionManager();
            var spawn = new SpawnManager(enemies);
            var waves = new WaveManager(spawn, enemies);
            var spirits = new SpiritManager();
            var shop = new ShopService();
            var upgrades = new UpgradeService();
            var wallet = new PlayerWallet(250, 20, 2);
            var inventory = new Inventory();

            return new GameManager(
                input, projectiles, enemies, collision, spawn, waves, spirits,
                shop, upgrades, wallet, inventory);
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ILocalizationService localization = LocalizationManager.Instance;

            Application.Run(new MainMenuForm(localization));
        }
    }
}