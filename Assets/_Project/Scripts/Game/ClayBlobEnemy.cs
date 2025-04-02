using _Project.Scripts.Utils;
using UnityEngine;
using UtilsModule;

namespace Game  {
    public class ClayBlobEnemy : Enemy {
        [Header("ClayBlob Settings")]
        [SerializeField] float wanderRadius = 10f;

        protected override void InitStates() {
            var wanderState = new ClayBlobWanderState(this, agent, wanderRadius, ClayBlobBaseState.ClayBlobWanderHash);
            var chaseState = new ClayBlobChaseState(this, agent, playerDetector.Player, ClayBlobBaseState.ClayBlobChaseHash);
            var attackState = new ClayBlobAttackState(this, agent, playerDetector.Player);
            var stunnedState = new ClayBlobStunnedState(this, agent, playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            At(stunnedState, wanderState, new FuncPredicate(() => !wasStunned));

            Any(stunnedState, new FuncPredicate(() => wasStunned));

            stateMachine.SetState(wanderState);
        }
        
        protected override void OnDeath() {
            base.OnDeath();
            Destroy(gameObject);
        }

        protected override void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            base.OnDamage(damage, dmgSource, ignoreKnockback);
            LastDamageSource = dmgSource ? dmgSource : LastDamageSource;
            if(!ignoreKnockback) wasStunned = true;
        }
        
        public override void Attack() {
            if(attackTimer.IsRunning) return;

            attackTimer.Start();
            playerDetector.PlayerComponent.TakeDamage(AttackDamage, dmgSource: gameObject);
        }
    }
}