using System;

namespace ElementalSpirit.Domain.Enemy.AI
{
    public sealed class SimpleEnemyBehavior : IEnemyBehavior
    {
        private readonly Random _random = new();
        private readonly float _detectionRange;
        private readonly float _attackRange;
        private float _patrolDirection;
        private float _patrolTimer;

        public SimpleEnemyBehavior(float detectionRange, float attackRange)
        {
            _detectionRange = detectionRange;
            _attackRange = attackRange;
            ChoosePatrolDirection();
        }

        public EnemyBehaviorDecision Update(
            float deltaTime,
            float enemyCenterX,
            float enemyCenterY,
            IEnemyTarget? target)
        {
            if (target == null || !target.IsAlive)
            {
                UpdatePatrol(deltaTime);
                return new EnemyBehaviorDecision(
                    Math.Abs(_patrolDirection) < 0.05f
                        ? EnemyBehaviorState.Idle
                        : EnemyBehaviorState.Patrol,
                    _patrolDirection);
            }

            float targetCenterX = target.X + target.Width / 2f;
            float targetCenterY = target.Y + target.Height / 2f;
            float deltaX = targetCenterX - enemyCenterX;
            float deltaY = targetCenterY - enemyCenterY;
            float distanceSquared = deltaX * deltaX + deltaY * deltaY;

            if (distanceSquared <= _attackRange * _attackRange)
                return new EnemyBehaviorDecision(EnemyBehaviorState.Attack);

            if (distanceSquared <= _detectionRange * _detectionRange)
            {
                float direction = Math.Sign(deltaX);
                return new EnemyBehaviorDecision(EnemyBehaviorState.Chase, direction);
            }

            UpdatePatrol(deltaTime);
            return new EnemyBehaviorDecision(
                Math.Abs(_patrolDirection) < 0.05f
                    ? EnemyBehaviorState.Idle
                    : EnemyBehaviorState.Patrol,
                _patrolDirection);
        }

        private void UpdatePatrol(float deltaTime)
        {
            _patrolTimer -= deltaTime;
            if (_patrolTimer <= 0f)
                ChoosePatrolDirection();
        }

        private void ChoosePatrolDirection()
        {
            int choice = _random.Next(0, 5);
            _patrolDirection = choice switch
            {
                0 or 1 => -1f,
                2 or 3 => 1f,
                _ => 0f
            };
            _patrolTimer = 0.6f + (float)_random.NextDouble() * 0.8f;
        }
    }
}
