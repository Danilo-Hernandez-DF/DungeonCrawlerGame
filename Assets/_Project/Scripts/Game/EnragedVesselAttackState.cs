using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnragedVesselAttackState : EnragedVesselBaseState {
        private NavMeshAgent agent;
        private Transform target;
        
        public EnragedVesselAttackState(EnragedVesselEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(
            enemy, animHash) {
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