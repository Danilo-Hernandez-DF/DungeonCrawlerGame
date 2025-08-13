using StateMachines;
using TMPro;
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
        
        [Header("Debug")]
        [SerializeField] private TMP_Text stateViewer;
        [SerializeField] private bool doDebug = false;
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
            DungeonController.Instance.OnEnemySpawn(this);
        }
        
        virtual protected void InitStates() { }

        protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
        protected Vector2 DirectionToTarget(Vector2 target)
        {
            var direction = target - (Vector2)transform.position;
            return direction.normalized;
        }
        public Vector2 GetFurthestPoint(Vector2 target, float radius)
        {
            var furthestPoint = target + (Vector2)(DirectionToTarget(target) * radius);
            return furthestPoint;
        }

        new void Update()
        {
            base.Update();

            agent.speed = Stats.Speed;
            playerDetector.Init(Stats.AttackRange);

            if (!GameManager.Instance.Paused)
            {
                if (agent.isActiveAndEnabled && agent.isStopped)
                {
                    agent.isStopped = false;
                    animationSpeed = 1f;
                }
                stateMachine.Update();
                attackTimer.Tick(Time.deltaTime);
            }
            else
            {
                agent.isStopped = true;
                animationSpeed = 0f;
            }

#if UNITY_EDITOR
            if (!stateViewer) return;

            if (!doDebug)
            {
                stateViewer.gameObject.SetActive(false);
                return;
            }

            stateViewer.gameObject.SetActive(true);
            stateViewer.text = stateMachine.GetState().ToString();
#endif
        }

        void FixedUpdate() {
            stateMachine.FixedUpdate();
            FacingDirection = MovementDirection;
        }
        
        protected override void OnDeath() {
            GameManager.Instance.TrackEntity(entityData, new() {timesKilled = 1});
            DungeonController.Instance.OnEnemyDeath(this);
            base.OnDeath();
        }
        
        protected override void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            base.OnDamage(damage, dmgSource, ignoreKnockback);
            DungeonController.Instance.OnEnemyHit(this);
        }
        
        public virtual void Attack() { }
    }
}
