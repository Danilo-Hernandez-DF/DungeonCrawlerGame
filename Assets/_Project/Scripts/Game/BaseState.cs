using StateMachines;
using UnityEngine;

namespace Game {
    public abstract class BaseState : IState {
        protected readonly Animator Animator;
        protected readonly float CrossFadeDuration = 0f;
        protected readonly PlayerController Player;

        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int DashHash = Animator.StringToHash("Dash");

        protected BaseState(PlayerController player, Animator animator) {
            this.Player = player;
            this.Animator = animator;
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
