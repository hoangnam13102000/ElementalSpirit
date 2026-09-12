using System;
using System.Drawing;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Presentation.Assets;

namespace ElementalSpirit.Domain.Enemy.NormalEnemy
{
    public class Slime : Enemy
    {
        private readonly Random _random = new();
        private readonly float _speed = 70f;
        private float _dirX = 0f, _dirY = 0f;
        private float _changeDirectionTimer = 0f;
        private float _changeDirectionInterval = 0.8f;
        private float _minX = 400f, _maxX = 1200f, _minY = 460f, _maxY = 550f;

        public Slime(float x, float y) : base(x, y, maxHealth: 30, damage: 8)
        {
            Width = 40; Height = 32;
            Image = AssetLoader.Get("Slime.png");
            ChooseNewDirection();
        }

        public override void Update(float deltaTime, float groundY)
        {
            UpdateEffectTimers(deltaTime);
            if (IsDying) return;

            float speedMultiplier = IsHurt ? 0.6f : 1f;
            _changeDirectionTimer -= deltaTime;
            if (_changeDirectionTimer <= 0) ChooseNewDirection();

            X += _dirX * _speed * speedMultiplier * deltaTime;
            Y += _dirY * _speed * speedMultiplier * deltaTime;

            if (_dirX < -0.1f) Facing = FacingDirection.Left;
            else if (_dirX > 0.1f) Facing = FacingDirection.Right;

            if (X < _minX) { X = _minX; _dirX = Math.Abs(_dirX); }
            if (X > _maxX) { X = _maxX; _dirX = -Math.Abs(_dirX); }
            if (Y < _minY) { Y = _minY; _dirY = Math.Abs(_dirY); }
            if (Y > _maxY) { Y = _maxY; _dirY = -Math.Abs(_dirY); }
        }

        private void ChooseNewDirection()
        {
            int choice = _random.Next(0, 9);
            switch (choice)
            {
                case 0: _dirX = 0; _dirY = -1; break;
                case 1: _dirX = 0; _dirY = 1; break;
                case 2: _dirX = -1; _dirY = 0; break;
                case 3: _dirX = 1; _dirY = 0; break;
                case 4: _dirX = -1; _dirY = -1; break;
                case 5: _dirX = 1; _dirY = -1; break;
                case 6: _dirX = -1; _dirY = 1; break;
                case 7: _dirX = 1; _dirY = 1; break;
                default: _dirX = 0; _dirY = 0; break;
            }
            if (_dirX != 0 && _dirY != 0)
            {
                float length = (float)Math.Sqrt(2);
                _dirX /= length; _dirY /= length;
            }
            _changeDirectionInterval = 0.6f + (float)_random.NextDouble() * 0.8f;
            _changeDirectionTimer = _changeDirectionInterval;
        }
    }
}