namespace Game {
    public class DashState : BaseState {
        public DashState(PlayerController player, int animHash = 0) : base(player, animHash) { }
        
        public override void OnEnter() {
            base.OnEnter();
            Player.invulnerable = true;
            Player.GetComponent<Collider2D>().enabled = false;
        }
        
        public override void OnExit() {
            base.OnExit();
            Player.invulnerable = false;
            Player.GetComponent<Collider2D>().enabled = true;
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}
