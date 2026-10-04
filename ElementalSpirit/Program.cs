using System;
using System.IO;
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
            var wallet = new PlayerWallet(0, 0, 0);
            var inventory = new Inventory();
            var localization = LocalizationManager.Instance;
            var bossEncounter = new BossEncounterManager(localization);
            var portals = new PortalManager();

            return new GameManager(
                input, projectiles, enemies, collision, spawn, waves, spirits, player, skills,
                shop, upgrades, wallet, inventory,
                bossEncounter,
                portals);
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnUiThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            ILocalizationService localization = LocalizationManager.Instance;
            ISaveGameService saveGameService = new SaveGameService();

            try
            {
                Application.Run(new MainMenuForm(localization, saveGameService));
            }
            finally
            {
                Services.AudioManager.Instance.Dispose();
            }
        }

        private static void OnUiThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrorLogger.Log("Unhandled UI-thread exception", e.Exception);
            MessageBox.Show(
                "Đã xảy ra lỗi nghiêm trọng. Ứng dụng sẽ đóng để tránh làm hỏng dữ liệu. Chi tiết đã được ghi vào thư mục nhật ký.",
                "Elemental Spirit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Application.Exit();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                ErrorLogger.Log("Unhandled application exception", exception);
            else
                ErrorLogger.Log("Unhandled application exception", new InvalidOperationException(
                    $"Unhandled object: {e.ExceptionObject}"));
        }
    }
}