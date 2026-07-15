namespace Game {
    public class LocomotionState : BaseState {
        public LocomotionState(PlayerController player, int animhash = 0) : base(player, animhash) { }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
