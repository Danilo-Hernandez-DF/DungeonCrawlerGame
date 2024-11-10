using StateMachines;
using UnityEngine;

namespace Game {
    public abstract class BaseState : IState {
        protected readonly Animator animator;
        protected readonly float crossFadeDuration = 0f;
        protected readonly PlayerController player;

        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int DashHash = Animator.StringToHash("Dash");

        protected BaseState(PlayerController player, Animator animator) {
            this.player = player;
            this.animator = animator;
        }

        public void Update() {
            
        }

        public virtual void FixedUpdate() {
        
        }

        public virtual void OnEnter() {

        }

        public virtual void OnExit() {

        }
    }
}
