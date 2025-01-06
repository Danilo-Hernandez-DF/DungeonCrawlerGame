using StateMachines;
using UnityEngine;

namespace Game
{
    public abstract class EnemyBaseSate : IState {
        protected readonly Enemy Enemy;
        protected readonly Animator Animator;

        protected static readonly int IdleHsah = Animator.StringToHash("ClayBlob_Idle");

        protected const float CrossFadeDuration = 0.1f;

        protected EnemyBaseSate(Enemy enemy, Animator animator) {
            this.Enemy = enemy;
            this.Animator = animator;
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
