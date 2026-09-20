using System;
using System.Windows.Forms;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Services;
using ElementalSpirit.Services.Abstractions;
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
            var player = new Player(PlayerConstants.DefaultStartX, PlayerConstants.DefaultStartY);
            var skills = new SkillManager(player);
            var shop = new ShopService();
            var upgrades = new UpgradeService();
            var wallet = new PlayerWallet(250, 20, 2);
            var inventory = new Inventory();
            var localization = LocalizationManager.Instance;
            var bossEncounter = new BossEncounterManager(localization);
            var portals = new PortalManager();
            var audio = new GameAudioService();

            return new GameManager(
                input, projectiles, enemies, collision, spawn, waves, spirits, player, skills,
                shop, upgrades, wallet, inventory,
                bossEncounter,
                portals,
                audio);
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ILocalizationService localization = LocalizationManager.Instance;
            ISaveGameService saveGameService = new SaveGameService();

            Application.Run(new MainMenuForm(localization, saveGameService));
            Services.AudioManager.Instance.Dispose();
        }
    }
}