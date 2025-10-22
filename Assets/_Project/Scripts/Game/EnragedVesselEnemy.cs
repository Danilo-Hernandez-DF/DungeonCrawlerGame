using UnityEngine;
using UtilsModule;

namespace Game {
    public class EnragedVesselEnemy : Enemy {
        [Header("Enraged Vessel Settings")]
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] private GameObject aoePrefab;
        [SerializeField] private GameObject projectilePrefab;
        private Vector2 patrolPoint;

        protected override void InitStates()
        {
            if (stateMachine == null) {
                return;
            }
            
            patrolPoint = transform.position;
            SetTarget(PlayerDetector.GetPlayerComponent());

            var wanderState = new EnemyWanderState(this, patrolPoint, wanderRadius);
            var attackState = new EnemyAttackState(this, target.transform);
            var stunnedState = new EnemyStunnedState(this);
            var focusedState = new EnemyChargingState(this);
            var chaseState = new EnemyChaseState(this, target.transform);
            var deathState = new EnemyDeathState(this);
            //var fleeState = new EnragedVesselFleeState(this, agent, playerDetector.Player);

            At(stunnedState, wanderState, new FuncPredicate(() => !wasStunned));

            //At(wanderState, focusedState, new FuncPredicate(() => !attackTimer.IsRunning && GetDistanceToTarget() < Stats.AttackRange));
            //attack timer not running & player in line of sight
            At(wanderState, chaseState, new FuncPredicate(() => !attackTimer.IsRunning && CanDetectTarget(Stats.DetectionRange)));
            //attack timer not running & player within range

            At(chaseState, focusedState, new FuncPredicate(() => !attackTimer.IsRunning && GetDistanceToTarget() < Stats.AttackRange));
            //attack timer not running & player in line of sight
            At(chaseState, wanderState, new FuncPredicate(() => !CanDetectTarget(Stats.DetectionRange) && GetDistanceToTarget(lastTargetPosition) < 0.1f));
            //player position not known

            At(focusedState, chaseState, new FuncPredicate(() => GetDistanceToTarget() > Stats.AttackRange));
            //player not in line of sight
            At(focusedState, attackState, new FuncPredicate(() => charged));
            //ready to attack

            At(attackState, wanderState, new FuncPredicate(() => !charged || attackTimer.IsRunning)); //has attacked

            Any(stunnedState, new FuncPredicate(() => wasStunned));
            Any(deathState, new FuncPredicate(() => hasDied));
            //Any(fleeState, new FuncPredicate(TargetInFleeRange));

            stateMachine.SetState(wanderState);
        }

        /*public bool InFleeRange() {   
            return playerDetector.DistanceToPlayer <= Stats.FleeRange;
        }*/
        
        /*private bool CanAttack() {
            return CanDetectTarget() && playerDetector.CanAttackPlayer() && !attackTimer.IsRunning && 
                   playerDetector.HasLineOfSight(transform.position);
        }*/
        
        protected override void OnDeath() {
            base.OnDeath();
            Instantiate(aoePrefab, transform.position, Quaternion.identity);
        }

        public override void Attack()
        {
            if (attackTimer.IsRunning) return;
            charged = false;

            attackTimer.Start();
            //Debug.Log("Enraged Vessel Attacking!");

            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetTarget(target.transform.position);
            base.Attack();
        }
    }
}