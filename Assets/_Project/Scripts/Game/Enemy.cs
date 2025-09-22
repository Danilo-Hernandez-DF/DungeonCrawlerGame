using AudioSystem;
using StateMachines;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class Enemy : Entity
    {
        [Header("Audio Settings")]
        [SerializeField] protected SoundData hitSound;
        [SerializeField] protected SoundData deathSound;
        [SerializeField] protected SoundData attackSound;
        [SerializeField] protected SoundData idleSound;

        [Header("Debug")]
        [SerializeField] private TMP_Text stateViewer;
        [SerializeField] private bool doDebug = false;
        protected GameObject LastDamageSource { get; set; }

        protected float TimeBetweenAttacks => Stats.AttackCooldown;
        protected int AttackDamage => Stats.Attack;
        public bool wasStunned;
        protected Vector3 MovementDirection;
        protected Entity target;
        public Vector3 lastTargetPosition;
        public bool charged = false;
        protected bool hasDied = false;
        protected float coliderRadius = 0;

        protected StateMachine stateMachine;
        protected CountdownTimer attackTimer;

        void Start()
        {
            attackTimer = new CountdownTimer(TimeBetweenAttacks);
            attackTimer.OnTimerStop += () => attackTimer.Reset(TimeBetweenAttacks);
            stateMachine = new StateMachine();

            if (Collider is CircleCollider2D circle) coliderRadius = circle.radius;

            InitStates();
            DungeonController.Instance.OnEnemySpawn(this);
        }

        virtual protected void InitStates() { }

        protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
        public Vector2 DirectionToTarget(Vector2 target = default)
        {
            if (target == default) target = this.target.transform.position;

            var direction = target - (Vector2)transform.position;
            return direction.normalized;
        }

        public Vector2 GetFurthestPoint(float radius, Vector2 target = default)
        {
            if (target == default) target = this.target.transform.position;

            var furthestPoint = target + (DirectionToTarget(target) * radius);
            return furthestPoint;
        }

        public void MoveTowards(Vector2 target = default, bool dash = false)
        {
            if (target == default) target = this.target.transform.position;

            MovementDirection = DirectionToTarget(target);

            RaycastHit2D hit = Physics2D.Raycast(transform.position + (MovementDirection * coliderRadius * 1.25f), MovementDirection, coliderRadius, LayerMask.GetMask("Entity"));

            if (hit.collider)
            {
                target += Vector2.Perpendicular(MovementDirection) * 2f;
                MovementDirection = DirectionToTarget(target);
            }

            while (!IsSpaceAvailable(target))
            {
                target *= 0.9f;
            }

            float speed = dash ? Stats.Speed * Stats.DashForce : Stats.Speed;

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        public bool HasLineOfSight(Vector2 target = default)
        {
            if (target == default) target = this.target.transform.position;

            var direction = DirectionToTarget(target);
            var distance = GetDistanceToTarget(target);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Wall"));

            return !hit.collider;
        }

        public bool CanDetectTarget(float distance, Vector2 target = default)
        {
            if (target == default) target = this.target.transform.position;
            return GetDistanceToTarget(target) <= distance && HasLineOfSight(target);
        }

        public void SetTarget(Entity target) => this.target = target;

        public float GetDistanceToTarget(Vector2 target = default)
        {
            if (target == default) target = this.target.transform.position;
            return Vector2.Distance(transform.position, target);
        }

        public bool IsSpaceAvailable(Vector2 target = default)
        {
            if (target == default) target = this.target.transform.position;
            return !Physics2D.OverlapCircle(target, coliderRadius, LayerMask.GetMask("Wall"));
        }

        new void Update()
        {
            base.Update();

            //playerDetector.Init(Stats.AttackRange);

            if (!GameManager.Instance.Paused)
            {
                stateMachine.Update();
                attackTimer.Tick(Time.deltaTime);
            }
            else
            {
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

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
            FacingDirection = MovementDirection;
        }

        protected override void OnDeath()
        {
            GameManager.Instance.TrackEntity(entityData, new() { timesKilled = 1 });
            DungeonController.Instance.OnEnemyDeath(this);

            if(deathSound.clip) SoundManager.Instance.CreateSound().WithSoundData(deathSound).Play();

            hasDied = true;
            base.OnDeath();
        }

        protected override void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false)
        {
            base.OnDamage(damage, dmgSource, ignoreKnockback);
            DungeonController.Instance.OnEnemyHit(this);

            if(hitSound.clip) SoundManager.Instance.CreateSound().WithSoundData(hitSound).Play();

            LastDamageSource = dmgSource ? dmgSource : null;
            if (!ignoreKnockback)
            {
                Vector2 knockbackDirection;
                if (LastDamageSource)
                {
                    knockbackDirection = (transform.position - LastDamageSource.transform.position).normalized;
                    ApplyForce(knockbackDirection * Stats.Knockback);
                    wasStunned = true;
                }
            }
        }

        public virtual void Attack()
        {
            if(attackSound.clip) SoundManager.Instance.CreateSound().WithSoundData(attackSound).WithRandomPitch().Play();
        }
    }
}
