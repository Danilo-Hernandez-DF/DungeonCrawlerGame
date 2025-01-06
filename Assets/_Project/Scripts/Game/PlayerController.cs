using UnityEngine;
using UtilsModule;
using System.Collections.Generic;
using StateMachines;
using Systems.Persistence;
using Unity.Cinemachine;

namespace Game {
    public class PlayerController : Entity, IBind<PlayerData> {
        [Header("References")]
        [SerializeField] Animator animator;
        [SerializeField] CinemachineCamera vCam;
        InputReader Input => GameManager.Instance.input;

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

        private FilteredInventory equipmentInv;

        // Animator Parameters
        // readonly int Speed = Animator.StringToHash("Speed");

        public void Bind(PlayerData data) {
            this.data = data;
            this.data.Id = Id;
            transform.position = data.position;
            transform.rotation = data.rotation;
        }

        new protected void Awake() {
            base.Awake();
            mainCam = Camera.main.transform;
            vCam.Follow = transform;
            vCam.LookAt = transform;
            vCam.OnTargetObjectWarped(transform, transform.position - vCam.transform.position - Vector3.forward);

            Rb.freezeRotation = true;
            Rb.gravityScale = 0f;

            health.Init(Stats.Health);

            SetupTimers();
            SetupStateMachine();
        }

        private void SetupStateMachine() {
            stateMachine = new StateMachine();

            // Declare States
            var locomotionState = new LocomotionState(this, animator);
            var dashState = new DashState(this, animator);
            var attackState = new AttackState(this, animator);

            // Define Transitions
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            At(locomotionState, attackState, new FuncPredicate(() => attackTimer.IsRunning));
            At(attackState, locomotionState, new FuncPredicate(() => !attackTimer.IsRunning));

            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // Set Initial State
            stateMachine.SetState(locomotionState);
        }

        bool ReturnToLocomotionState() {
            return !dashTimer.IsRunning 
                && !attackTimer.IsRunning;
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

        void OnDash(bool performed) {
            if(GameManager.Instance.Paused) return;
            attackStep = 0;
            if(performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning) {
                dashTimer.Reset(DashDuration);
                dashTimer.Start();
            } else if(!performed && dashTimer.IsRunning) {
                dashTimer.Stop();
            }

            Debug.Log(Stats.ToString());
        }

        new void Update() {
            data.position = transform.position;
            data.rotation = transform.rotation;

            base.Update();

            if(!GameManager.Instance.Paused) {
                if(GetComponent<Animator>().speed == 0f) {
                    GetComponent<Animator>().speed = 1f;
                }
                movement = new Vector2(Input.Direction.x, Input.Direction.y);
                stateMachine.Update();
                HandleTimers();
            } else {
                GetComponent<Animator>().speed = 0f;
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
            Item weapon = equipmentInv.GetItem(equipmentInv.NextMatch(GameManager.Instance.ItemDatabase.GetEquipment(Tag.Equipment.Weapon)));
            if(weapon?.IsEmpty ?? true) return;
            if(!attackTimer.IsRunning) {
                attackTimer.Reset(AttackCooldown);
                attackTimer.Start();
            }
        }

        public void Attack() {
            AdditionalDataManager.Instance.TriggerItemBehaviour(equipmentInv, 
                equipmentInv.NextMatch(GameManager.Instance.ItemDatabase.GetEquipment(Tag.Equipment.Weapon)), BehaviourType.OnUse, this, attackStep);
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

        private void HandleHorizontalMovement(Vector2 movement) {
            Vector2 velocity = dashVelocity * MoveSpeed * movement;
            Rb.linearVelocity = new Vector2(velocity.x, velocity.y);
        }

        void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}