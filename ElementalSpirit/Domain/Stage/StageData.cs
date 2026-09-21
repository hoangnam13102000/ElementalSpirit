using System.Collections.Generic;
using ElementalSpirit.Factories;

namespace ElementalSpirit.Domain.Stage
{
    public class StageData
    {
        public int StageNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BackgroundImageName { get; set; } = string.Empty;
        public List<WaveData> Waves { get; set; } = new();

        public List<TerrainPlatform> Platforms { get; set; } = new();
        public List<TerrainWall> Walls { get; set; } = new();

        /// <summary>
        /// Vị trí mặt đất chính của stage (tỉ lệ theo chiều cao vùng chơi). Dùng cho điểm xuất hiện của Player,
        /// vị trí cổng, mặt đất dự phòng của quái... Mặc định 0.78 như các màn khác; màn 3 có mặt đất cao hơn.
        /// </summary>
        public float GroundTopRatio { get; set; } = 0.78f;

        private static void AddPlatform(StageData stage, string name, float minXRatio, float maxXRatio, float topRatio, float bottomRatio)
        {
            stage.Platforms.Add(new TerrainPlatform(name, minXRatio, maxXRatio, topRatio, bottomRatio));
        }

        private static void AddWall(StageData stage, string name, float xRatio, float topRatio, float bottomRatio, bool blocksPlayer = false)
        {
            stage.Walls.Add(new TerrainWall(name, xRatio, topRatio, bottomRatio, blocksPlayer));
        }

        public static StageData CreateEarthForest()
        {
            var stage = new StageData
            {
                StageNumber = 1,
                Name = "Earth Forest",
                BackgroundImageName = "EarthForest.png"
            };

            stage.Platforms.Add(new TerrainPlatform(
                name: "Ground",
                minXRatio: 0.0f,
                maxXRatio: 1.0f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftTowerTop",
                minXRatio: 0.10f,
                maxXRatio: 0.19f,
                topRatio: 0.38f,
                bottomRatio: 0.45f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftLowerStone",
                minXRatio: 0.15f,
                maxXRatio: 0.21f,
                topRatio: 0.62f,
                bottomRatio: 0.69f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftMiddleStone",
                minXRatio: 0.19f,
                maxXRatio: 0.27f,
                topRatio: 0.57f,
                bottomRatio: 0.64f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftRightStone",
                minXRatio: 0.27f,
                maxXRatio: 0.34f,
                topRatio: 0.63f,
                bottomRatio: 0.70f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightLowerRock",
                minXRatio: 0.87f,
                maxXRatio: 1.0f,
                topRatio: 0.56f,
                bottomRatio: 0.64f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightTowerTop",
                minXRatio: 0.93f,
                maxXRatio: 1.0f,
                topRatio: 0.35f,
                bottomRatio: 0.43f));

            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 6, 0.7f));
            stage.Waves.Add(wave1);

            return stage;
        }

        /// <summary>
        /// Khu vực 2: cầu treo trong rừng. Tiếp nối độ khó ngay sau EarthForest.
        /// </summary>
        public static StageData CreateEarthForest2()
        {
            var stage = new StageData
            {
                StageNumber = 2,
                Name = "Earth Forest - Canopy Bridge",
                BackgroundImageName = "EarthForest2.png"
            };

            stage.Walls.Add(new TerrainWall(
                name: "LeftPitWall",
                xRatio: 0.4583f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            stage.Walls.Add(new TerrainWall(
                name: "RightPitWall",
                xRatio: 0.6111f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftGround",
                minXRatio: 0.0f,
                maxXRatio: 0.4583f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            // Rieng da ben phai, doi xung voi ben trai.
            stage.Platforms.Add(new TerrainPlatform(
                name: "RightGround",
                minXRatio: 0.6111f,
                maxXRatio: 1.0f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftRootStep",
                minXRatio: 0.132f,
                maxXRatio: 0.204f,
                topRatio: 0.617f,
                bottomRatio: 0.70f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftRubbleUpperStep",
                minXRatio: 0.1875f,
                maxXRatio: 0.256f,
                topRatio: 0.53f,
                bottomRatio: 0.60f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftRubbleLowerStep",
                minXRatio: 0.267f,
                maxXRatio: 0.328f,
                topRatio: 0.60f,
                bottomRatio: 0.66f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftPillarTop",
                minXRatio: 0.088f,
                maxXRatio: 0.206f,
                topRatio: 0.348f,
                bottomRatio: 0.44f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "CanopyBridgeLeft",
                minXRatio: 0.30f,
                maxXRatio: 0.40f,
                topRatio: 0.40f,
                bottomRatio: 0.47f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "CanopyBridgeLeftSlope",
                minXRatio: 0.40f,
                maxXRatio: 0.50f,
                topRatio: 0.43f,
                bottomRatio: 0.50f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "CanopyBridgeCenter",
                minXRatio: 0.50f,
                maxXRatio: 0.60f,
                topRatio: 0.46f,
                bottomRatio: 0.53f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "CanopyBridgeRightSlope",
                minXRatio: 0.60f,
                maxXRatio: 0.70f,
                topRatio: 0.43f,
                bottomRatio: 0.50f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "CanopyBridgeRight",
                minXRatio: 0.70f,
                maxXRatio: 0.80f,
                topRatio: 0.40f,
                bottomRatio: 0.47f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightLowerStepLeft",
                minXRatio: 0.836f,
                maxXRatio: 0.89f,
                topRatio: 0.56f,
                bottomRatio: 0.63f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightLowerStepCenter",
                minXRatio: 0.89f,
                maxXRatio: 0.95f,
                topRatio: 0.54f,
                bottomRatio: 0.63f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightLowerStepRight",
                minXRatio: 0.95f,
                maxXRatio: 1.0f,
                topRatio: 0.527f,
                bottomRatio: 0.63f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightUpperStep",
                minXRatio: 0.840f,
                maxXRatio: 1.0f,
                topRatio: 0.366f,
                bottomRatio: 0.465f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightRockSmallStep",
                minXRatio: 0.93f,
                maxXRatio: 0.97f,
                topRatio: 0.34f,
                bottomRatio: 0.40f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "RightRockSmallStepEdge",
                minXRatio: 0.97f,
                maxXRatio: 1.0f,
                topRatio: 0.31f,
                bottomRatio: 0.37f));

            stage.Platforms.Add(new TerrainPlatform(
                name: "PitStone",
                minXRatio: 0.457f,
                maxXRatio: 0.513f,
                topRatio: 0.90f,
                bottomRatio: 0.97f));

            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 8, 0.55f));
            stage.Waves.Add(wave1);

            return stage;
        }

        public static StageData CreateEarthForest3()
        {
            // Mặt đất thật của màn 3 (nền thấp có dãy ống) nằm ở 0.66 chiều cao ảnh nền 720x404,
            // cao hơn mặt đất 0.78 của các màn khác -> phải khai báo GroundTopRatio riêng để Player,
            // cổng và quái xuất hiện đúng trên mặt đất thay vì bị "chôn" bên trong nền rồi rơi xuyên xuống.
            const float groundTop = 0.66f;

            // Cái lỗ (vết ống vỡ) trên nền, đo trực tiếp trên EarthForest3.png:
            //   mép trái x = 470px (0.653), mép phải x = 507px (0.704),
            //   đáy lỗ là mép ống vỡ bên dưới, y ≈ 303px (0.75).
            const float holeLeft = 0.653f;
            const float holeRight = 0.704f;
            const float holeFloorTop = 0.75f;

            var stage = new StageData
            {
                StageNumber = 3,
                Name = "Earth Forest - Ruined Stairway",
                BackgroundImageName = "EarthForest3.png",
                GroundTopRatio = groundTop
            };

            // Nền thấp chạy liền mạch 2 bên, chừa đúng khoảng trống của cái lỗ.
            AddPlatform(stage, "GroundLeft", 0.00f, holeLeft, groundTop, 1.0f);
            AddPlatform(stage, "GroundRight", holeRight, 1.0f, groundTop, 1.0f);

            // Đáy của lỗ: Player rơi vào sẽ đứng được trên đáy này, không rơi xuyên xuống dưới màn hình.
            // Tên có chứa "Pit" để SpawnManager không cho quái xuất hiện trong lỗ.
            AddPlatform(stage, "PitFloor", holeLeft, holeRight, holeFloorTop, 0.80f);

            // 2 thành lỗ: chặn Player đi xuyên ngang qua nền đất khi đang ở dưới mặt đất
            // (phải nhảy lên khỏi mép), đồng thời chặn quái đi vào lỗ như vách vực ở màn 2.
            AddWall(stage, "PitWallLeft", holeLeft, groundTop, 1.0f, blocksPlayer: true);
            AddWall(stage, "PitWallRight", holeRight, groundTop, 1.0f, blocksPlayer: true);

            // Cầu thang đá đổ nát (Terrain) - 5 bậc lớn xấp xỉ đường viền bậc thang trong ảnh,
            // đủ thấp để nhảy lên từng bậc (chênh lệch mỗi bậc luôn nhỏ hơn tầm nhảy của Player).
            AddPlatform(stage, "LeftStairTop", 0.00f, 0.105f, 0.05f, 0.16f);
            AddPlatform(stage, "LeftStairUpper", 0.06f, 0.19f, 0.16f, 0.28f);
            AddPlatform(stage, "LeftStairMid", 0.14f, 0.27f, 0.28f, 0.40f);
            AddPlatform(stage, "LeftStairLower", 0.22f, 0.35f, 0.40f, 0.52f);
            AddPlatform(stage, "LeftStairBase", 0.30f, 0.40f, 0.52f, 0.66f);

            // 2 phiến đá nổi (Platform) giữa màn, đúng vị trí trong ảnh nền.
            AddPlatform(stage, "CenterPlatform", 0.335f, 0.615f, 0.35f, 0.50f);
            AddPlatform(stage, "RightPlatform", 0.788f, 1.0f, 0.36f, 0.51f);

            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 10, 0.45f));
            stage.Waves.Add(wave1);

            return stage;
        }

        public static StageData CreateFinalForestBossStage()
        {
            var stage = new StageData
            {
                StageNumber = 4,
                Name = "Final Forest - Gorgon Boss",
                BackgroundImageName = "FinalForest.png"
            };

            stage.Platforms.Add(new TerrainPlatform(
                name: "Ground",
                minXRatio: 0.0f,
                maxXRatio: 1.0f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            var wave1 = new WaveData(1);
            stage.Waves.Add(wave1);

            return stage;
        }

        public static List<StageData> CreateEarthForestCampaign()
        {
            return new List<StageData>
            {
                CreateEarthForest(),
                CreateEarthForest2(),
                CreateEarthForest3(),
                CreateFinalForestBossStage()
            };
        }
    }
}