using UnityEngine;

namespace Game {
    public class LocomotionState : BaseState {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            Animator.CrossFade(LocomotionHash, CrossFadeDuration);
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
