using _Project.Scripts.Utils;
using UnityEngine;
using UtilsModule;

namespace Game  {
    public class ClayBlobEnemy : Enemy {
        // Animation Hashes
        public static int ChaseHash = Animator.StringToHash("ClayBlobChase");
        public static int WanderHash = Animator.StringToHash("ClayBlobWander");

        //------------------------

        [Header("ClayBlob Settings")]
        [SerializeField] float wanderRadius = 10f;
        private Vector2 patrolPoint;

        protected override void InitStates()
        {
            if (stateMachine == null) {
                return;
            }
            
            patrolPoint = transform.position;
            SetTarget(PlayerDetector.GetPlayerComponent());

            var wanderState = new EnemyWanderState(this, patrolPoint, wanderRadius, WanderHash);
            var chaseState = new EnemyChaseState(this, target.transform, ChaseHash);
            var attackState = new EnemyAttackState(this, target.transform);
            var stunnedState = new EnemyStunnedState(this);
            var deathState = new EnemyDeathState(this);

            At(wanderState, chaseState, new FuncPredicate(() => CanDetectTarget(Stats.DetectionRange)));
            At(chaseState, wanderState, new FuncPredicate(() => !CanDetectTarget(Stats.DetectionRange) && GetDistanceToTarget(lastTargetPosition) < 0.1f));
            At(chaseState, attackState, new FuncPredicate(() => GetDistanceToTarget() < Stats.AttackRange && !attackTimer.IsRunning));
            At(attackState, chaseState, new FuncPredicate(() => attackTimer.IsRunning || GetDistanceToTarget() > Stats.AttackRange));
            At(stunnedState, wanderState, new FuncPredicate(() => !wasStunned));

            Any(stunnedState, new FuncPredicate(() => wasStunned));
            Any(deathState, new FuncPredicate(() => hasDied));

            stateMachine.SetState(wanderState);
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