using System;
using System.Drawing;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Domain.Enemy.NormalEnemy
{
    public class Slime : Enemy
    {
        private readonly Random _random = new();
        private readonly float _speed = 70f;

        // Huong di chuyen hien tai
        private float _dirX = 0f;
        private float _dirY = 0f;

        // Thoi gian giu huong hien tai
        private float _changeDirectionTimer = 0f;
        private float _changeDirectionInterval = 0.8f;

        // Gioi han bien di chuyen (de khong bay lung tung qua xa)
        // SUA: gioi han theo dai "nen dat" (con duong trong anh nen), khop voi GameManager.GroundTop/GroundBottom,
        // de slime chi chay tren mat dat, khong bay len troi hay lot xuong vach da.
        private float _minX = 400f;
        private float _maxX = 1200f;
        private float _minY = 460f;
        private float _maxY = 550f;

        public Slime(float x, float y)
            : base(x, y, maxHealth: 30, damage: 8)
        {
            Width = 40;
            Height = 32;

            // ===== THEM: Load hinh anh slime tu Resources =====
            // AssetLoader se tu dong tim file trong cac thu muc con cua Resources/Images
            // Vi du: Resources/Images/Enemies/Slime.png
            // Neu load that bai, Image se la null va GameForm se fallback ve hinh tron
            Image = AssetLoader.Get("Slime.png");
            // =================================================

            // Khoi tao huong ngau nhien ngay tu dau
            ChooseNewDirection();
        }

        public override void Update(float deltaTime)
        {
            if (!IsAlive) return;

            _changeDirectionTimer -= deltaTime;

            // Den luc doi huong
            if (_changeDirectionTimer <= 0)
            {
                ChooseNewDirection();
            }

            // Di chuyen
            X += _dirX * _speed * deltaTime;
            Y += _dirY * _speed * deltaTime;

            // Giua trong vung hop ly (khong de no chay ra ngoai man hinh qua nhieu)
            if (X < _minX) { X = _minX; _dirX = Math.Abs(_dirX); }
            if (X > _maxX) { X = _maxX; _dirX = -Math.Abs(_dirX); }
            if (Y < _minY) { Y = _minY; _dirY = Math.Abs(_dirY); }
            if (Y > _maxY) { Y = _maxY; _dirY = -Math.Abs(_dirY); }
        }

        private void ChooseNewDirection()
        {
            // Chon 1 trong 8 huong 
            int choice = _random.Next(0, 9);
            switch (choice)
            {
                case 0: _dirX = 0; _dirY = -1; break; // len
                case 1: _dirX = 0; _dirY = 1; break; // xuong
                case 2: _dirX = -1; _dirY = 0; break; // trai
                case 3: _dirX = 1; _dirY = 0; break; // phai
                case 4: _dirX = -1; _dirY = -1; break; // len-trai
                case 5: _dirX = 1; _dirY = -1; break; // len-phai
                case 6: _dirX = -1; _dirY = 1; break; // xuong-trai
                case 7: _dirX = 1; _dirY = 1; break; // xuong-phai
                default: _dirX = 0; _dirY = 0; break; // dung yen mot chut
            }

            if (_dirX != 0 && _dirY != 0)
            {
                float length = (float)Math.Sqrt(2);
                _dirX /= length;
                _dirY /= length;
            }

            // Thoi gian giu huong nay (0.6 → 1.4 giay)
            _changeDirectionInterval = 0.6f + (float)_random.NextDouble() * 0.8f;
            _changeDirectionTimer = _changeDirectionInterval;
        }
    }
}