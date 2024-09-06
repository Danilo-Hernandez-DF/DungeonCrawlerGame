using UnityEngine;

namespace Game {
    public class AttackState : BaseState {
        public AttackState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            //Animator stuff
            player.Attack();
        }

        public override void FixedUpdate() {
            player.HandleMovement();
        }
    }
}
