using StateMachines;
using UnityEngine;

namespace Game {
    public class DashState : BaseState {
        public DashState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            Animator.CrossFade(DashHash, CrossFadeDuration);
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
