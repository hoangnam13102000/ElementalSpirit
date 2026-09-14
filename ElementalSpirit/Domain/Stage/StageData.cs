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

            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 6, 0.7f));
            stage.Waves.Add(wave1);

            var wave2 = new WaveData(2);
            wave2.Spawns.Add(new SpawnData(EnemyType.Slime, 8, 0.55f));
            stage.Waves.Add(wave2);

            var wave3 = new WaveData(3);
            wave3.Spawns.Add(new SpawnData(EnemyType.Slime, 10, 0.45f));
            stage.Waves.Add(wave3);

            var wave4 = new WaveData(4);
            wave4.Spawns.Add(new SpawnData(EnemyType.Slime, 12, 0.4f));
            stage.Waves.Add(wave4);

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

            // Nen duoc chia thanh nhieu tang bac thang khop voi hinh nen EarthForest2,
            // do truc tiep tu anh nen (khong con dung 1 mat dat phang full-width nua):
            // dat 2 ben vuc -> mo da vun (buoc dem) -> cau treo -> mo da phia phai.
            // O giua (khoang nuoc/thac) khong co nen nao, nen nhan vat se roi xuong
            // neu buoc vao do ma khong nhay len cac buoc dem/cau.
            //
            // Moi bac deu duoc dat cao hon bac truoc do ~90-95px (o do phan giai
            // chuan) - nam trong tam nhay toi da cua Player hien tai (JumpForce=560,
            // Gravity=1500 => tam nhay ly thuyet ~104px), nen luon nhay toi duoc.

            // Rieng da ben trai, noi nhan vat xuat hien dau man.
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

            // Buoc dem: mo da vun ben trai (ngay duoi dau cau treo), giup nhan vat
            // nhay len tu mat dat truoc khi nhay tiep len cau (thay vi nhay thang
            // 1 buoc rat cao, khong kha thi voi tam nhay hien tai).
            stage.Platforms.Add(new TerrainPlatform(
                name: "LeftRubbleStep",
                minXRatio: 0.2488f,
                maxXRatio: 0.34f,
                topRatio: 0.64f,
                bottomRatio: 0.78f));

            // Cau treo bac ngang khoang trong o giua. Dat theo dung do cao phan
            // sagging (vong xuong) thap nhat cua day cau trong hinh nen - do la
            // phan nhan vat thuc su dat chan len khi di qua cau (2 dau cau/cay
            // cao hon nhieu nhung qua cao de nhay len, > tam nhay toi da).
            stage.Platforms.Add(new TerrainPlatform(
                name: "CanopyBridge",
                minXRatio: 0.3264f,
                maxXRatio: 0.7778f,
                topRatio: 0.50f,
                bottomRatio: 0.60f));

            // Buoc dem: mo da noi thap ben phai, do lai chinh xac tu hinh nen
            // (truoc do dat cao hon mat da ve ~30px, khien nhan vat nhu lo lung
            // phia tren tang da). Thap hon cau 1 chut - buoc xuong tu cau de dang.
            stage.Platforms.Add(new TerrainPlatform(
                name: "RightLowerStep",
                minXRatio: 0.836f,
                maxXRatio: 1.0f,
                topRatio: 0.575f,
                bottomRatio: 0.68f));

            // Mo da noi cao ben phai (tuong duong cot da/mo trang tri phia tren cau) -
            // buoc "thuong" tuy chon, nhay len duoc tu CanopyBridge.
            stage.Platforms.Add(new TerrainPlatform(
                name: "RightUpperStep",
                minXRatio: 0.840f,
                maxXRatio: 1.0f,
                topRatio: 0.38f,
                bottomRatio: 0.45f));

            // Phien da vo (mau cau go cu) nam duoi day khe vuc, ngay canh thac
            // nuoc - thap hon ca mat dat chinh (o day khe vuc). Nhan vat co the
            // roi xuong day (khong can nhay) roi nhay nguoc len mat dat sau do
            // (~88px, trong tam nhay toi da).
            stage.Platforms.Add(new TerrainPlatform(
                name: "PitStone",
                minXRatio: 0.457f,
                maxXRatio: 0.513f,
                topRatio: 0.90f,
                bottomRatio: 0.97f));

            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 8, 0.55f));
            stage.Waves.Add(wave1);

            var wave2 = new WaveData(2);
            wave2.Spawns.Add(new SpawnData(EnemyType.Slime, 10, 0.45f));
            stage.Waves.Add(wave2);

            var wave3 = new WaveData(3);
            wave3.Spawns.Add(new SpawnData(EnemyType.Slime, 12, 0.35f));
            stage.Waves.Add(wave3);

            return stage;
        }

        /// <summary>
        /// Khu vực 3: khu vực cuối của Earth Forest. Màn khó nhất trong chuỗi 3 background.
        /// </summary>
        public static StageData CreateEarthForest3()
        {
            var stage = new StageData
            {
                StageNumber = 3,
                Name = "Earth Forest - Ruined Stairway",
                BackgroundImageName = "EarthForest3.png"
            };

            stage.Platforms.Add(new TerrainPlatform(
                name: "Ground",
                minXRatio: 0.0f,
                maxXRatio: 1.0f,
                topRatio: 0.78f,
                bottomRatio: 1.0f));

            var wave1 = new WaveData(1);
            wave1.Spawns.Add(new SpawnData(EnemyType.Slime, 10, 0.45f));
            stage.Waves.Add(wave1);

            var wave2 = new WaveData(2);
            wave2.Spawns.Add(new SpawnData(EnemyType.Slime, 13, 0.35f));
            stage.Waves.Add(wave2);

            var wave3 = new WaveData(3);
            wave3.Spawns.Add(new SpawnData(EnemyType.Slime, 16, 0.3f));
            stage.Waves.Add(wave3);

            var wave4 = new WaveData(4);
            wave4.Spawns.Add(new SpawnData(EnemyType.Slime, 20, 0.25f));
            stage.Waves.Add(wave4);

            return stage;
        }

        public static List<StageData> CreateEarthForestCampaign()
        {
            return new List<StageData>
            {
                CreateEarthForest(),
                CreateEarthForest2(),
                CreateEarthForest3()
            };
        }
    }
}