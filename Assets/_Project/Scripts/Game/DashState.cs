using _Project.Scripts.Utils;
using UnityEngine;

namespace Game {
    public class DashState : BaseState {
        public DashState(PlayerController player, int animHash = 0) : base(player, animHash) { }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
