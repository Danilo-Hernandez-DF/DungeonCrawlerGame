using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class EnemyAttackState : EnemyBaseState {
        readonly Transform target;

        public EnemyAttackState(Enemy enemy, Transform target, int animHash = 0) : base(enemy, animHash) {
            if (animHash != 0) {
                this.animHash = animHash;
            }
            
            this.target = target;
        }

        public override void Update() {
            base.Update();
            enemy.Attack();
        }
    }
}
