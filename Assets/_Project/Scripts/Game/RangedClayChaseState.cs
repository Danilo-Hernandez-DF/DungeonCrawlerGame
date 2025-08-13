using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class RangedClayChaseState : RangedClayBaseState {
        readonly NavMeshAgent agent;
        readonly Transform target;

        public RangedClayChaseState(RangedClayEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash) {
            this.agent = agent;
            this.target = target;
        }

        public override void Update() {
            if(agent.isActiveAndEnabled) agent.SetDestination(target.position);
            base.Update();
        }
    }
}