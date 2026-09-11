using System.Drawing;

namespace ElementalSpirit.Domain.Player
{
    public class Player
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Speed { get; private set; } = 220f;
        public int Width { get; } = 32;
        public int Height { get; } = 32;
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        public int BaseMaxHp { get; private set; } = 100;
        public int MaxHp { get; private set; } = 100;
        public int CurrentHp { get; private set; } = 100;
        public int CoreDamage { get; private set; } = 10;
        public int BaseDamage { get; private set; } = 10;
        public int Damage { get; private set; } = 10;
        public int Defense { get; private set; } = 0;

        private float _damageMultiplier = 1f;

        public bool IsInvulnerable { get; set; }
        public bool ActiveShield { get; set; }
        public bool ActiveFireBoost { get; set; }
        public bool ActiveHealEffect { get; set; }
        public bool ActiveWindBarrage { get; set; }
        public int ExtraProjectiles { get; set; }

        public Player(float startX, float startY)
        {
            X = startX;
            Y = startY;
        }

        public void Move(float dirX, float dirY, float deltaTime)
        {
            X += dirX * Speed * deltaTime;
            Y += dirY * Speed * deltaTime;
        }

        public void ClampToBounds(float minX, float minY, float maxX, float maxY)
        {
            if (X < minX) X = minX;
            if (Y < minY) Y = minY;
            if (X + Width > maxX) X = maxX - Width;
            if (Y + Height > maxY) Y = maxY - Height;
        }

        public void TakeDamage(int amount)
        {
            if (IsInvulnerable || ActiveShield) return;
            int reduced = Math.Max(1, amount - Defense);
            CurrentHp = Math.Max(0, CurrentHp - reduced);
        }

        public void Heal(int amount)
        {
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
        }

        public void ApplyEquipmentBonuses(int bonusDamage, int bonusMaxHp, int bonusDefense)
        {
            float hpRatio = MaxHp > 0 ? (float)CurrentHp / MaxHp : 1f;
            BaseDamage = CoreDamage + bonusDamage;
            MaxHp = BaseMaxHp + bonusMaxHp;
            Defense = bonusDefense;
            CurrentHp = Math.Clamp((int)(MaxHp * hpRatio + 0.5f), 0, MaxHp);
            Damage = Math.Max(1, (int)(BaseDamage * _damageMultiplier));
        }

        public void ApplyDamageMultiplier(float multiplier)
        {
            _damageMultiplier = multiplier;
            Damage = Math.Max(1, (int)(BaseDamage * _damageMultiplier));
        }

        public void ResetDamageMultiplier()
        {
            _damageMultiplier = 1f;
            Damage = BaseDamage;
        }

        public float AttackCooldown { get; private set; }
        public float AttackInterval { get; private set; } = 0.25f;

        public void Update(float deltaTime)
        {
            if (AttackCooldown > 0) AttackCooldown -= deltaTime;
        }

        public bool CanAttack() => AttackCooldown <= 0;
        public void ResetAttackCooldown() => AttackCooldown = AttackInterval;
        public bool IsAlive => CurrentHp > 0;
    }
}