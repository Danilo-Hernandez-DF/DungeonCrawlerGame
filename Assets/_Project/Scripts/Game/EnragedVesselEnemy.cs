using StateMachines;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class EnragedVesselEnemy : Enemy {
        [Header("Enraged Vessel Settings")]
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] private GameObject aoePrefab;
        
        private Transform lastPlayerPosition;
        public bool readyToAttack = false;
        protected bool hasDied = false;

        protected override void InitStates() {
            var wanderState = new EnragedVesselWanderState(this, agent, wanderRadius);
            var attackState = new EnragedVesselAttackState(this, agent, playerDetector.Player);
            var stunnedState = new EnragedVesselStunnedState(this, agent, playerDetector.Player);
            var focusedState = new EnragedVesselFocusedState(this, agent, playerDetector.Player);
            var chaseState = new EnragedVesselChaseState(this, agent, playerDetector.Player);
            var deathState = new EnragedVesselDeathState(this);

            At(stunnedState, wanderState, new FuncPredicate(() => !wasStunned));
            At(wanderState, focusedState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(chaseState, focusedState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(focusedState, chaseState, new FuncPredicate(() => !playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.RemembersPlayerPosition));
            At(focusedState, attackState, new FuncPredicate(() => readyToAttack));
            At(attackState, wanderState, new FuncPredicate(() => !readyToAttack || !playerDetector.CanAttackPlayer()));

            Any(stunnedState, new FuncPredicate(() => wasStunned));
            Any(deathState, new FuncPredicate(() => hasDied));

            stateMachine.SetState(wanderState);
        }
        
        protected override void OnDeath() {
            base.OnDeath();
            hasDied = true;
            Instantiate(aoePrefab, transform.position, Quaternion.identity);
        }

        protected override void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            base.OnDamage(damage, dmgSource, ignoreKnockback);
            LastDamageSource = dmgSource ? dmgSource : LastDamageSource;
            if(!ignoreKnockback) wasStunned = true;
        }
        
        public override void Attack() {
            if(attackTimer.IsRunning) return;

            attackTimer.Start();
            Debug.Log("Enraged Vessel Attacking!");
        }
    }

    public class EnragedVesselDeathState : EnragedVesselBaseState {
        CountdownTimer timer;
        public EnragedVesselDeathState(EnragedVesselEnemy enemy, int animHash = 0) : base(enemy, animHash) { }
        
        public override void OnEnter() {
            base.OnEnter();
            enemy.gameObject.AddComponent<DestroyAfter>().time = 2f;
        }
    }
}