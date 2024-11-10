using StateMachines;
using UnityEngine;

namespace Game
{
    public abstract class EnemyBaseSate : IState {
        protected readonly Enemy enemy;
        protected readonly Animator animator;

        protected static readonly int IdleHsah = Animator.StringToHash("ClayBlob_Idle");

        protected const float crossFadeDuration = 0.1f;

        protected EnemyBaseSate(Enemy enemy, Animator animator) {
            this.enemy = enemy;
            this.animator = animator;
        }

        public virtual void FixedUpdate() {
            //noop
        }

        public virtual void OnEnter() {
            //noop
        }

        public virtual void OnExit() {
            //noop
        }

        public virtual void Update() {
            //noop
        }
    }
}
