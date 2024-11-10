using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using StateMachines;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity {
        [Header("Behaviour Settings")]
        [SerializeField, Self] NavMeshAgent agent;
        [SerializeField, Self] PlayerDetector playerDetector;
        [SerializeField, Child] Animator animator;
        [SerializeField] float wanderRadius = 10f;

        float timeBetweenAttacks => Stats.AttackCooldown;
        int attackDamage => Stats.Attack;

        public GameObject lastDamageSource { get; protected set; }

        StateMachine stateMachine;

        CountdownTimer attackTimer;

        public Vector3 MovementDirection => agent.velocity.normalized;

        public bool tookDamage = false;

        void OnValidate() => this.ValidateRefs();

        new protected void Awake() {
            base.Awake();
            agent.speed = Stats.Speed;
        }

        void Start() {
            playerDetector.Init(Stats.AttackRange);

            attackTimer = new CountdownTimer(timeBetweenAttacks);
            stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            var damagedState = new EnemyDamagedState(this, animator, agent, playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            At(damagedState, wanderState, new FuncPredicate(() => !tookDamage));

            Any(damagedState, new FuncPredicate(() => tookDamage));
            

            stateMachine.SetState(wanderState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        new void Update() {
            base.Update();

            if(!GameManager.Instance.Paused) {
                if(agent.isStopped) {
                    agent.isStopped = false;
                    animator.speed = 1f;
                }
                stateMachine.Update();
                attackTimer.Tick(Time.deltaTime);
            } else {
                agent.isStopped = true;
                animator.speed = 0f;
            }
        }

        void FixedUpdate() {
            stateMachine.FixedUpdate();
            facingDirection = MovementDirection;
        }

        protected override void OnDeath() {
            base.OnDeath();
            Destroy(gameObject);
        }


        protected override void OnDamage(int damage, GameObject dmgSource = null) {
            lastDamageSource = dmgSource != null ? dmgSource : lastDamageSource;
            tookDamage = true;
        }

        public void Attack() {
            if(attackTimer.IsRunning) return;

            attackTimer.Start();
            playerDetector.PlayerComponent.TakeDamage(attackDamage, dmgSource: gameObject);
        }
    }
}
