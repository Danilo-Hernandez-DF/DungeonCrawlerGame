using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Game {
    public class EnemyChaseState : EnemyBaseState {
        readonly Transform target;

        public EnemyChaseState(Enemy enemy, Transform target, int animHash = 0) : base(enemy, animHash)
        {
            this.target = target;
            enemy.lastTargetPosition = Vector3.zero;
        }

        public override void FixedUpdate() {
            if (enemy.HasLineOfSight(target.position)) {
                enemy.lastTargetPosition = target.position;
                enemy.MoveTowards(target.position, true);
            } else if (enemy.lastTargetPosition != Vector3.zero) {
                enemy.MoveTowards(enemy.lastTargetPosition, true);
            }
            base.FixedUpdate();
        }
    }
}
