using _Project.Scripts.Utils;
using StateMachines;
using UnityEngine;

namespace Game {
    public abstract class EnemyBaseState<T> : IState where T : Enemy {
        protected T enemy;
        protected Animator animator;
        protected int animHash;

        protected EnemyBaseState(T enemy, int animHash = 0) {
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
    }
}
