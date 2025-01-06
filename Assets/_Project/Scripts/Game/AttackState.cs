using UnityEngine;

namespace Game {
    public class AttackState : BaseState {
        public AttackState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            //Animator stuff
            Player.Attack();
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
