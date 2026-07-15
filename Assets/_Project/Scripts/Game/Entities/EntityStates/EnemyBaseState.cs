using StateMachines;

namespace Game {
    public abstract class EnemyBaseState : IState {
        protected Enemy enemy;
        protected Animator animator;
        protected int animHash;

        protected EnemyBaseState(Enemy enemy, int animHash = 0) {
            this.enemy = enemy;
            this.animHash = animHash;
            animator = enemy.GetComponent<Animator>();
        }

        public virtual void FixedUpdate() {
            //noop
        }

        public virtual void OnEnter() {
            animator.CrossFade(animHash, 0f);
        }

        public virtual void OnExit() {
            
        }

        public virtual void Update() {
            
        }

        public override string ToString() {
            return GetType().Name;
        }
    }
}
