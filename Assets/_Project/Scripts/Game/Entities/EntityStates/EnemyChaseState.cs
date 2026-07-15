using Utils;

namespace Game {
    public class EnemyChaseState : EnemyBaseState
    {
        readonly Transform target;
        Vector2 randomDeviation;
        private CountdownTimer timer = new CountdownTimer(0.5f);

        public EnemyChaseState(Enemy enemy, Transform target, int animHash = 0) : base(enemy, animHash)
        {
            this.target = target;
            enemy.lastTargetPosition = Vector3.zero;
            timer.OnTimerStart += () => randomDeviation = Random.insideUnitCircle * (enemy.Stats.AttackRange * 0.8f);
            timer.OnTimerStop += () => timer.Start();
            timer.Start();
        }

        public override void FixedUpdate()
        {
            if (enemy.HasLineOfSight(target.position))
            {
                enemy.lastTargetPosition = target.position;
                enemy.MoveTowards((Vector2)target.position + randomDeviation, true);
            }
            else if (enemy.lastTargetPosition != Vector3.zero)
            {
                enemy.MoveTowards(enemy.lastTargetPosition, true);
            }
            base.FixedUpdate();
        }

        public override void Update()
        {
            timer.Tick(Time.deltaTime);
            base.Update();
        }
    }
}
