using System;
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
        [SerializeField, Anywhere] CinemachineVirtualCamera vCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Settings")]
        [SerializeField] float moveSpeed = 4f;
        [SerializeField] float smoothTime = .2f;

        [Header("Dash Settings")]
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;

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

        // Animator Parameters
        // readonly int Speed = Animator.StringToHash("Speed");

        protected override void Awake() {
            mainCam = Camera.main.transform;
            vCam.Follow = transform;
            vCam.LookAt = transform;
            vCam.OnTargetObjectWarped(transform, transform.position - vCam.transform.position- Vector3.forward);

            rb.freezeRotation = true;
            rb.gravityScale = 0f;

            // Setup Timers
            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () => { 
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };

            timers = new(2) {dashTimer, dashCooldownTimer};

            // State Machine
            stateMachine = new StateMachine();

            // Declare States
            var locomotionState = new LocomotionState(this, animator);
            var dashState = new DashState(this, animator);

            // Define Transitions
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(() => !dashTimer.IsRunning));

            // Set Initial State
            stateMachine.SetState(locomotionState);

            base.Awake();
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void OnDash(bool performed) {
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
        }

        void OnDisable() {
            input.Dash -= OnDash;
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