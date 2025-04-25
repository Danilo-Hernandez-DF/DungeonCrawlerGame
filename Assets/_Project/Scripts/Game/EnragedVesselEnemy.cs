using NUnit.Framework;
using StateMachines;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class EnragedVesselEnemy : Enemy {
        [Header("Enraged Vessel Settings")]
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] private GameObject aoePrefab;
        [SerializeField] private GameObject projectilePrefab;

        

        public float detectionRadius => playerDetector.detectionRadius;
        public Transform lastPlayerPosition;
        public bool remembersLastPlayerPosition => rememberTimer.IsRunning;
        public bool readyToAttack = false;
        protected bool hasDied = false;
        public CountdownTimer rememberTimer = new CountdownTimer(2f);

        protected override void InitStates() {
            var wanderState = new EnragedVesselWanderState(this, agent, wanderRadius);
            var attackState = new EnragedVesselAttackState(this, agent, playerDetector.Player);
            var stunnedState = new EnragedVesselStunnedState(this, agent, playerDetector.Player);
            var focusedState = new EnragedVesselFocusedState(this, agent, playerDetector.Player);
            var chaseState = new EnragedVesselChaseState(this, agent, playerDetector.Player);
            var deathState = new EnragedVesselDeathState(this);
            //var fleeState = new EnragedVesselFleeState(this, agent, playerDetector.Player);

            At(stunnedState, wanderState, new FuncPredicate(() => !wasStunned));
            
            At(wanderState, focusedState, new FuncPredicate(() => !attackTimer.IsRunning && HasLineOfSight())); 
            //attack timer not running & player in line of sight
            At(wanderState, chaseState, new FuncPredicate(() => !attackTimer.IsRunning && !HasLineOfSight() &&
                                                                KnowsPlayerPosition())); 
            //attack timer not running & player not in line of sight with known position
            
            At(chaseState, focusedState, new FuncPredicate(() => !attackTimer.IsRunning && HasLineOfSight())); 
            //attack timer not running & player in line of sight
            At(chaseState, wanderState, new FuncPredicate(() => !KnowsPlayerPosition())); 
            //player position not known
            
            At(focusedState, chaseState, new FuncPredicate(() => !HasLineOfSight())); 
            //player not in line of sight
            At(focusedState, attackState, new FuncPredicate(() => readyToAttack)); 
            //ready to attack
            
            At(attackState, wanderState, new FuncPredicate(() => !readyToAttack || attackTimer.IsRunning)); //has attacked

            Any(stunnedState, new FuncPredicate(() => wasStunned));
            Any(deathState, new FuncPredicate(() => hasDied));
            //Any(fleeState, new FuncPredicate(TargetInFleeRange));

            stateMachine.SetState(wanderState);
        }

        private bool HasLineOfSight() {
            return playerDetector.HasLineOfSight(transform.position) && CanDetectTarget();
        }
        
        public bool CanDetectTarget() => playerDetector.CanDetectPlayer(FacingDirection);
        
        private bool KnowsPlayerPosition() => rememberTimer.IsRunning || playerDetector.CanDetectPlayer(FacingDirection);

        /*public bool InFleeRange() {   
            return playerDetector.DistanceToPlayer <= Stats.FleeRange;
        }*/
        
        /*private bool CanAttack() {
            return CanDetectTarget() && playerDetector.CanAttackPlayer() && !attackTimer.IsRunning && 
                   playerDetector.HasLineOfSight(transform.position);
        }*/
        
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
            readyToAttack = false;

            attackTimer.Start();
            Debug.Log("Enraged Vessel Attacking!");
            
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetTarget(playerDetector.Player.position);
        }
    }
}