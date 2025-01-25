using StateMachines;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity {
        [Header("Behaviour Settings")]
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected PlayerDetector playerDetector;
        [SerializeField] protected Animator animator;
        
        protected GameObject LastDamageSource { get;  set; }

        protected float TimeBetweenAttacks => Stats.AttackCooldown;
        protected int AttackDamage => Stats.Attack;
        public bool wasStunned;
        protected Vector3 MovementDirection => agent.velocity.normalized;

        protected StateMachine stateMachine;
        protected CountdownTimer attackTimer;
        
        void Start() {
            attackTimer = new CountdownTimer(TimeBetweenAttacks);
            attackTimer.OnTimerStop += () => attackTimer.Reset(TimeBetweenAttacks);
            stateMachine = new StateMachine();

            InitStates();
        }
        
        virtual protected void InitStates() { }

        protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        new void Update() {
            base.Update();

            agent.speed = Stats.Speed;
            playerDetector.Init(Stats.AttackRange);

            if(!GameManager.Instance.Paused) {
                if(agent.isActiveAndEnabled && agent.isStopped) {
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
            GameManager.Instance.TrackStat(StatisticsTracker.TrackedStat.KilledEnemies, 1);
            base.OnDeath();
        }
        
        public virtual void Attack() { }
    }
}
