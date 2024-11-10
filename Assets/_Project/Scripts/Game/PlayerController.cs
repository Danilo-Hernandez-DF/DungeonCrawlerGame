using KBCore.Refs;
using UnityEngine;
using UtilsModule;
using Cinemachine;
using System.Collections.Generic;
using StateMachines;

namespace Game {
    public class PlayerController : Entity {
        [Header("References")]
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineVirtualCamera vCam;
        InputReader input => GameManager.Instance.input;

        [Header("Settings")]
        [SerializeField] float smoothTime = .2f;
        float moveSpeed => Stats.Speed;

        float dashCooldown => Stats.DashCooldown;
        float dashForce => Stats.DashForce;
        float dashDuration => Stats.DashDuration;

        float atatckCooldown => Stats.AttackCooldown;
        float attackRange => Stats.AttackRange;
        float attackDistance => Stats.AttackDistance;
        int attackDamage => Stats.Attack;

        const float ZeroF = 0f;

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

        private Inventory equipmentInv;

        // Animator Parameters
        // readonly int Speed = Animator.StringToHash("Speed");

        new protected void Awake() {
            base.Awake();
            mainCam = Camera.main.transform;
            vCam.Follow = transform;
            vCam.LookAt = transform;
            vCam.OnTargetObjectWarped(transform, transform.position - vCam.transform.position - Vector3.forward);

            rb.freezeRotation = true;
            rb.gravityScale = 0f;

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

        public void UpdateEquipment(Inventory inventory) {
            equipmentInv = inventory;
        }

        private void SetupTimers() {
            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };

            attackTimer = new CountdownTimer(atatckCooldown);

            timers = new(3) { dashTimer, dashCooldownTimer, attackTimer };
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void OnDash(bool performed) {
            if(GameManager.Instance.Paused) return;
            if(performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning) {
                dashTimer.Start();
            } else if(!performed && dashTimer.IsRunning) {
                dashTimer.Stop();
            }
        }

        new void Update() {
            base.Update();

            if(!GameManager.Instance.Paused) {
                if(GetComponent<Animator>().speed == 0f) {
                    GetComponent<Animator>().speed = 1f;
                }
                movement = new Vector2(input.Direction.x, input.Direction.y);
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
            input.Dash += OnDash;
            input.Aim += OnAim;
            input.Attack += OnAttack;
        }

        void OnDisable() {
            input.Dash -= OnDash;
            input.Aim -= OnAim;
            input.Attack -= OnAttack;
        }

        void OnAim(Vector2 position, bool isDeviceMouse) { 
            if(isDeviceMouse) {
                var pos = CameraManager.Instance.camera.ScreenToWorldPoint(position);
                facingDirection = (pos - transform.position).normalized;
            } else {
                facingDirection = position.normalized;
                Debug.Log("using controller");
            }
        }

        void OnAttack() {
            if(GameManager.Instance.Paused) return;
            var weapon = equipmentInv.GetItem(5);
            if(weapon?.IsEmpty ?? true) return;
            if(!attackTimer.IsRunning) attackTimer.Start();
        }

        public void Attack() {
            EquipmentManager.Instance.TriggerItemBehaviour(equipmentInv, 5, BehaviourType.OnUse, this);
        }

        void HandleTimers() {
            foreach(var timer in timers) {
                timer.Tick(Time.deltaTime);
            }
        }

        public void HandleMovement() {
            if(GameManager.Instance.Paused) {
                rb.velocity = Vector2.zero;
                return;
            }
            if(movement.magnitude > ZeroF) {
                HandleHorizontalMovement(movement);
                SmoothSpeed(movement.magnitude);
            } else {
                SmoothSpeed(ZeroF);
                rb.velocity = new Vector2(ZeroF, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector2 movement) {
            Vector2 velocity = dashVelocity * moveSpeed * movement;
            rb.velocity = new Vector2(velocity.x, velocity.y);
        }

        void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}