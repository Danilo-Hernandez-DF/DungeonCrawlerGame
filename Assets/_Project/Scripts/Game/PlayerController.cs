using KBCore.Refs;
using UnityEngine;
using UtilsModule;
using Cinemachine;
using System.Collections.Generic;
using StateMachines;

namespace Game {
    public class PlayerController : ValidatedSingleton<PlayerController> {
        [Header("References")]
        [SerializeField, Self] Rigidbody2D rb;
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] Health health;
        [SerializeField, Anywhere] CinemachineVirtualCamera vCam;
        InputReader input => GameManager.Instance.input;

        [Header("Settings")]
        [SerializeField] float moveSpeed = 4f;
        [SerializeField] float smoothTime = .2f;

        [Header("Dash Settings")]
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;

        [Header("Attack Settings")]
        [SerializeField] float atatckCooldown = 0.5f;
        [SerializeField] float attackRange = 1f;
        [SerializeField] float attackDistance = 1f;
        [SerializeField] int attackDamage = 10;

        const float ZeroF = 0f;
        Vector2 aimDirection = Vector2.zero;

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

        // Animator Parameters
        // readonly int Speed = Animator.StringToHash("Speed");

        protected override void Awake() {
            mainCam = Camera.main.transform;
            vCam.Follow = transform;
            vCam.LookAt = transform;
            vCam.OnTargetObjectWarped(transform, transform.position - vCam.transform.position - Vector3.forward);

            rb.freezeRotation = true;
            rb.gravityScale = 0f;

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

        void Update() {
            movement = new Vector2(input.Direction.x, input.Direction.y);
            stateMachine.Update();
            
            HandleTimers();
            UpdateAnimator();
        }

        void FixedUpdate() {
            stateMachine.FixedUpdate();
        }

        void OnEnable() {
            input.Dash += OnDash;
            input.Attack += OnAttack;
            input.Aim += OnAim;
        }

        void OnDisable() {
            input.Dash -= OnDash;
            input.Attack -= OnAttack;
            input.Aim -= OnAim;
        }

        void OnAim(Vector2 position, bool isDeviceMouse) { 
            if(isDeviceMouse) {
                aimDirection = (position - (Vector2)transform.position).normalized;
            }
        }

        void OnAttack() {
            if(GameManager.Instance.Paused) return;
            if(!attackTimer.IsRunning) {
                attackTimer.Start();
            }
        }

        public void Attack() {
            Vector2 attackPos = transform.position + ((Vector3)aimDirection * attackDistance);
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRange);
            Debug.DrawLine(attackPos, attackPos + (aimDirection * attackRange), Color.red, 1f);

            foreach(var hit in hits) {
                Debug.Log(hit.name);

                if(hit.CompareTag("Enemy")) {
                    hit.GetComponent<Health>().TakeDamage(attackDamage);
                }
            }
        }

        private void UpdateAnimator() {
            // animator.SetFloat(Speed, currentSpeed);
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
            Vector2 velocity = movement * moveSpeed * dashVelocity * Time.fixedDeltaTime;
            rb.velocity = new Vector2(velocity.x, velocity.y);
        }

        void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}