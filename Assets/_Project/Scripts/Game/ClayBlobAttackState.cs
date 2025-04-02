using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class ClayBlobAttackState : ClayBlobBaseState {
        readonly NavMeshAgent agent;
        readonly Transform target;

        public ClayBlobAttackState(ClayBlobEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash) {
            if (animHash != 0) {
                this.animHash = animHash;
            }
            this.agent = agent;
            this.target = target;
        }

        public override void Update() {
            base.Update();
            agent.SetDestination(target.position);
            enemy.Attack();
        }
    }
}
