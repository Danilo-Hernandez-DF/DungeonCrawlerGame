namespace Game {
    public class EnemyWanderState : EnemyBaseState {
        private Vector3 startPoint;
        private readonly float wanderRadius;
        private readonly Vector2 patrolPoint;
        private Vector2 nextPoint;

        public EnemyWanderState(Enemy enemy, Vector2 patrolPoint, float wanderRadius, int animHash = 0) : base(enemy, animHash)
        {
            this.patrolPoint = patrolPoint;
            startPoint = enemy.transform.position;
            this.wanderRadius = wanderRadius;
            nextPoint = patrolPoint;
        }

        public override void FixedUpdate() {
            if (!HasReachedDestination(nextPoint))
            {
                enemy.MoveTowards(nextPoint);
                return;
            }

            startPoint = enemy.transform.position;

            Vector2 randomDirection;
            do
            {
                randomDirection = Random.insideUnitCircle * wanderRadius;
                randomDirection += patrolPoint;
            } while (!enemy.HasLineOfSight(randomDirection) || !enemy.IsSpaceAvailable(randomDirection));
            
            nextPoint = randomDirection;
            base.FixedUpdate();
        }

        bool HasReachedDestination(Vector2 target) {
            return enemy.GetDistanceToTarget(target) <= 0.1f;
        }
    }
}
