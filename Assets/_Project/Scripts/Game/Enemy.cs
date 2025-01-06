using StateMachines;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity {
        [Header("Behaviour Settings")]
        [SerializeField] NavMeshAgent agent;
        [SerializeField] PlayerDetector playerDetector;
        [SerializeField] Animator animator;
        [SerializeField] float wanderRadius = 10f;

        float TimeBetweenAttacks => Stats.AttackCooldown;
        int AttackDamage => Stats.Attack;

        public GameObject LastDamageSource { get; protected set; }

        StateMachine stateMachine;

        CountdownTimer attackTimer;

        public Vector3 MovementDirection => agent.velocity.normalized;

        public bool wasStunned = false;

        void Start() {
            attackTimer = new CountdownTimer(TimeBetweenAttacks);
            attackTimer.OnTimerStop += () => attackTimer.Reset(TimeBetweenAttacks);
            stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            var stunnedState = new EnemyStunnedState(this, animator, agent, playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer(MovementDirection)));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            At(stunnedState, wanderState, new FuncPredicate(() => !wasStunned));

            Any(stunnedState, new FuncPredicate(() => wasStunned));
            

            stateMachine.SetState(wanderState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        new void Update() {
            base.Update();

            agent.speed = Stats.Speed;
            playerDetector.Init(Stats.AttackRange);

            if(!GameManager.Instance.Paused) {
                if(agent.isActiveAndEnabled && agent.isStopped == true) {
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
            FacingDirection = MovementDirection;
        }

        protected override void OnDeath() {
            base.OnDeath();
            Destroy(gameObject);
        }


        protected override void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            LastDamageSource = dmgSource != null ? dmgSource : LastDamageSource;
            if(!ignoreKnockback) wasStunned = true;
        }

        public void Attack() {
            if(attackTimer.IsRunning) return;

            attackTimer.Start();
            playerDetector.PlayerComponent.TakeDamage(AttackDamage, dmgSource: gameObject);
        }
    }
}
