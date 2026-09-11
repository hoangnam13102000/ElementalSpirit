using System;

namespace ElementalSpirit.Domain.Enemy.NormalEnemy
{
    public class Slime : Enemy
    {
        private readonly Random _random = new();
        private readonly float _speed = 70f;

        // Hướng di chuyển hiện tại
        private float _dirX = 0f;
        private float _dirY = 0f;

        // Thời gian giữ hướng hiện tại
        private float _changeDirectionTimer = 0f;
        private float _changeDirectionInterval = 0.8f;

        // Giới hạn biên di chuyển (để không bay lung tung quá xa)
        private float _minX = 400f;
        private float _maxX = 1200f;
        private float _minY = 80f;
        private float _maxY = 620f;

        public Slime(float x, float y)
            : base(x, y, maxHealth: 30, damage: 8)
        {
            Width = 40;
            Height = 32;

            // Khởi tạo hướng ngẫu nhiên ngay từ đầu
            ChooseNewDirection();
        }

        public override void Update(float deltaTime)
        {
            if (!IsAlive) return;

            _changeDirectionTimer -= deltaTime;

            // Đến lúc đổi hướng
            if (_changeDirectionTimer <= 0)
            {
                ChooseNewDirection();
            }

            // Di chuyển
            X += _dirX * _speed * deltaTime;
            Y += _dirY * _speed * deltaTime;

            // Giữ trong vùng hợp lý (không để nó chạy ra ngoài màn hình quá nhiều)
            if (X < _minX) { X = _minX; _dirX = Math.Abs(_dirX); }
            if (X > _maxX) { X = _maxX; _dirX = -Math.Abs(_dirX); }
            if (Y < _minY) { Y = _minY; _dirY = Math.Abs(_dirY); }
            if (Y > _maxY) { Y = _maxY; _dirY = -Math.Abs(_dirY); }
        }

        private void ChooseNewDirection()
        {
            // Chọn 1 trong 8 hướng 
            int choice = _random.Next(0, 9);

            switch (choice)
            {
                case 0: _dirX = 0; _dirY = -1; break; // lên
                case 1: _dirX = 0; _dirY = 1; break; // xuống
                case 2: _dirX = -1; _dirY = 0; break; // trái
                case 3: _dirX = 1; _dirY = 0; break; // phải
                case 4: _dirX = -1; _dirY = -1; break; // lên-trái
                case 5: _dirX = 1; _dirY = -1; break; // lên-phải
                case 6: _dirX = -1; _dirY = 1; break; // xuống-trái
                case 7: _dirX = 1; _dirY = 1; break; // xuống-phải
                default: _dirX = 0; _dirY = 0; break; // đứng yên một chút
            }

            if (_dirX != 0 && _dirY != 0)
            {
                float length = (float)Math.Sqrt(2);
                _dirX /= length;
                _dirY /= length;
            }

            // Thời gian giữ hướng này (0.6 → 1.4 giây)
            _changeDirectionInterval = 0.6f + (float)_random.NextDouble() * 0.8f;
            _changeDirectionTimer = _changeDirectionInterval;
        }
    }
}