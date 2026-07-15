using StateMachines;

namespace Game {
    public abstract class BaseState : IState {
        protected readonly PlayerController Player;
        protected Animator animator;
        protected int animHash;

        public static readonly int LocomotionHash = Animator.StringToHash("PlayerLocomotion");
        public static readonly int DashHash = Animator.StringToHash("PlayerDash");

        protected BaseState(PlayerController player, int animhash = 0) {
            if (animhash != 0) {
                this.animHash = animhash;
            }
            
            this.Player = player;
            animator = player.GetComponent<Animator>();
        }

        public void Update() {
            
        }

        public virtual void FixedUpdate() {
        
        }

        public virtual void OnEnter() {
            animator.CrossFade(animHash, 0f);
        }

        public virtual void OnExit() {
            
        }
    }
}
