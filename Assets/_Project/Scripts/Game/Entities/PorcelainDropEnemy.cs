using Utils;

namespace Game
{
    public class PorcelainDropEnemy : Enemy
    {
        public static int ChaseHash = Animator.StringToHash("PorcelainDropChase");
        
        protected override void InitStates()
        {
            if (stateMachine == null) {
                return;
            }
            
            SetTarget(PlayerDetector.GetPlayerComponent());

            var spawnState = new SpawnState(this, 0f);
            var waitState = new WaitState(this, 1f);
            var chaseState = new EnemyChaseState(this, target.transform, ChaseHash);
            var attackState = new EnemyAttackState(this, target.transform);
            var stunnedState = new EnemyStunnedState(this);
            var deathState = new EnemyDeathState(this, .5f);
    
            At(spawnState, chaseState, new FuncPredicate(() => spawned));
            At(chaseState, waitState, new FuncPredicate(() => !CanDetectTarget(Stats.DetectionRange) && GetDistanceToTarget(lastTargetPosition) < 0.1f));
            At(waitState, chaseState, new FuncPredicate(() => CanDetectTarget(Stats.DetectionRange) || GetDistanceToTarget(lastTargetPosition) >= 0.1f));
            At(chaseState, attackState, new FuncPredicate(() => GetDistanceToTarget() < Stats.AttackRange && !attackTimer.IsRunning));
            At(attackState, chaseState, new FuncPredicate(() => attackTimer.IsRunning || GetDistanceToTarget() > Stats.AttackRange));
            At(stunnedState, chaseState, new FuncPredicate(() => !wasStunned));
            At(waitState, deathState, new FuncPredicate(() => waited));

            Any(stunnedState, new FuncPredicate(() => wasStunned));
            Any(deathState, new FuncPredicate(() => hasDied));

            stateMachine.SetState(spawnState);
        }
        
        public override void Attack()
        {
            if (attackTimer.IsRunning) return;

            attackTimer.Start();
            target.TakeDamage(AttackDamage, new DamageSource(gameObject));
            base.Attack();
        }
    }
}