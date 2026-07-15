namespace Game {
    public class AttackState : BaseState {
        public AttackState(PlayerController player, int animhash = 0) : base(player, animhash) { }

        public override void OnEnter() {
            Player.Attack();
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
