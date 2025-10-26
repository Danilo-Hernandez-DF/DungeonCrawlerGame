using UnityEngine;
using UtilsModule;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using StateMachines;
using Systems.Persistence;
using Unity.Cinemachine;

namespace Game {
    public class PlayerController : Entity, IBind<PlayerData> {
        [Header("References")]
        [SerializeField] CinemachineCamera vCam;
        static InputReader Input => GameManager.Instance.input;

        [Header("Settings")]
        [SerializeField] float smoothTime = .2f;
        float MoveSpeed => Stats.Speed;

        [Header("Binding Data")]
        [SerializeField] PlayerData data;
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        float DashCooldown => Stats.DashCooldown;
        float DashForce => Stats.DashForce;
        float DashDuration => Stats.DashDuration;

        float AttackCooldown => Stats.AttackCooldown;
        float AttackRange => Stats.AttackRange;
        float AttackDistance => Stats.AttackDistance;
        int AttackDamage => Stats.Attack;

        const float ZeroF = 0f;
        private int attackStep = 0;

        Transform mainCam;

        float currentSpeed;
        float velocity;
        float dashVelocity = 1f;

        Vector2 movement;

        StateMachine stateMachine;

        List<Timer> timers = new();
        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;
        CountdownTimer attackTimer;
        

        public FilteredInventory equipmentInv { get; private set; }
        public Inventory playerInv { get; private set; }

        // Animator Parameters
        // readonly int Speed = Animator.StringToHash("Speed");

        public void Bind(PlayerData data) {
            this.data = data;
            this.data.Id = Id;
        }

        protected void Awake() {
            Init();
            mainCam = CameraManager.Instance.camera.transform;
            vCam.Follow = transform;
            vCam.LookAt = transform;
            vCam.OnTargetObjectWarped(transform, transform.position - vCam.transform.position - Vector3.forward);

            Rb.freezeRotation = true;
            Rb.gravityScale = 0f;

            health.Init(Stats.Health);

            SetupTimers();
            SetupStateMachine();
        }
        
        public void GiveItem(Item item, int amount) => playerInv.TryAdd(item, amount);

        private void SetupStateMachine() {
            stateMachine = new StateMachine();
            
            // Declare States
            var locomotionState = new LocomotionState(this, BaseState.LocomotionHash);
            var dashState = new DashState(this, BaseState.DashHash);
            var attackState = new AttackState(this);

            // Define Transitions
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            At(locomotionState, attackState, new FuncPredicate(() => attackTimer.IsRunning));

            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // Set Initial State
            stateMachine.SetState(locomotionState);
        }

        bool ReturnToLocomotionState() {
            return !dashTimer.IsRunning 
                && !attackTimer.IsRunning;
        }
        
        public void UpdateInventory(Inventory inventory) {
            playerInv = inventory;
        }

        public void UpdateEquipment(FilteredInventory inventory) {
            equipmentInv = inventory;
            attackStep = 0;
        }

        private void SetupTimers() {
            dashTimer = new CountdownTimer(DashDuration);

            dashCooldownTimer = new CountdownTimer(DashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = DashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Reset(DashCooldown);
                dashCooldownTimer.Start();
            };

            attackTimer = new CountdownTimer(AttackCooldown);

            timers = new(3) { dashTimer, dashCooldownTimer, attackTimer };
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void OnDash() {
            if(GameManager.Instance.Paused) return;
            if(dashTimer.IsRunning) return;
            attackStep = 0;
            
            dashTimer.Reset(DashDuration);
            dashTimer.Start();
        }

        new void Update() {
            base.Update();

            if(!GameManager.Instance.Paused) {
                if(anim.speed == 0f) {
                    anim.speed = 1f;
                }
                movement = new Vector2(Input.Direction.x, Input.Direction.y);
                stateMachine.Update();
                HandleTimers();
            } else {
                anim.speed = 0f;
            }
        }

        void FixedUpdate() {
            stateMachine.FixedUpdate();
        }

        void OnEnable() {
            Input.Dash += OnDash;
            Input.Aim += OnAim;
            Input.Attack += OnAttack;
        }

        void OnDisable() {
            Input.Dash -= OnDash;
            Input.Aim -= OnAim;
            Input.Attack -= OnAttack;
        }

        void OnAim(Vector2 position, bool isDeviceMouse) { 
            if(isDeviceMouse) {
                var pos = CameraManager.Instance.camera.ScreenToWorldPoint(position);
                FacingDirection = (pos - transform.position).normalized;
            } else {
                FacingDirection = position.normalized;
                //Debug.Log("using controller");
            }
        }

        void OnAttack() {
            if(GameManager.Instance.Paused) return;
            //Debug.Log("not paused");
            Item weapon = equipmentInv.GetItem(equipmentInv.NextMatch(GameManager.GetEquipment(Tag.Equipment.Weapon)));
            if(weapon?.IsEmpty ?? true) return;
            //Debug.Log($"weapon exists: {weapon.data.name}");
            if(attackTimer.IsRunning) return;
            //Debug.Log("timer not running");
            attackTimer.Reset(AttackCooldown);
            attackTimer.Start();
        }

        public void Attack() {
            AdditionalDataManager.Instance.TriggerItemBehaviour(equipmentInv, 
                equipmentInv.NextMatch(GameManager.GetEquipment(Tag.Equipment.Weapon)), BehaviourType.OnUse, this, attackStep);
            if(++attackStep > 2) attackStep = 0;
        }

        void HandleTimers() {
            foreach(var timer in timers) {
                timer.Tick(Time.deltaTime);
            }
        }

        public void HandleMovement() {
            if(GameManager.Instance.Paused) {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            if(movement.magnitude > ZeroF) {
                HandleHorizontalMovement(movement);
                SmoothSpeed(movement.magnitude);
            } else {
                SmoothSpeed(ZeroF);
                Rb.linearVelocity = new Vector2(ZeroF, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector2 mvmnt) {
            Vector2 vel = dashVelocity * MoveSpeed * mvmnt;
            Rb.linearVelocity = new Vector2(vel.x, vel.y);
        }

        void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

        protected override void OnDamage(int damage, DamageSource dmgSource, bool ignoreKnockback = false) {
            Status.OnDamage(damage);

            if (RendererComponent) {
                Vector2 hitDirection = Vector2.zero;
                if (dmgSource.source)
                {
                    hitDirection = (transform.position - dmgSource.source.transform.position).normalized * 2f;
                }

                RendererComponent.sharedMaterial = GameManager.Instance.entityHitmaterial;
                RendererComponent.sharedMaterial.SetVector("_HitDirection", hitDirection);
                RendererComponent.sharedMaterial.SetTexture("_Texture2D", RendererComponent.sprite.texture);
                RendererComponent.sharedMaterial.SetColor("_Color", Color.white);

                Invoke(nameof(ResetMaterial), 0.1f);
            }

            DungeonController.Instance.OnPlayerHit(dmgSource);
            GameManager.Instance.TrackStat(StatisticsTracker.TrackedStat.DamageTaken, damage);
            Entity sourceEntity = dmgSource.entity;
            if(sourceEntity) {
                GameManager.Instance.TrackEntity(sourceEntity.entityData, new EntityTrack() {damageDealt = damage});
            }
        }
    }
}