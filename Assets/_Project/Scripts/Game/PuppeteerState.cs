using StateMachines;
using UnityEngine;

namespace Game
{
    public class PuppeteerState: IState
    {
        protected PuppeteerBoss enemy;
        protected Animator animator;
        protected int animHash;

        protected PuppeteerState(PuppeteerBoss enemy, int animHash = 0) {
            this.enemy = enemy;
            this.animHash = animHash;
            animator = enemy.GetComponent<Animator>();
        }
        
        public virtual void OnEnter() {
            animator.CrossFade(animHash, 0f);
        }
        public virtual void Update() {
            
        }
        public virtual void FixedUpdate() {
            
        }
        public virtual void OnExit() {
            
        }
    }
}