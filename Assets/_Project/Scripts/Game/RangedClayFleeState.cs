using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class RangedClayFleeState : RangedClayBaseState
    {
        NavMeshAgent agent;
        Transform target;

        public RangedClayFleeState(RangedClayEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash) {
            this.agent = agent;
            this.target = target;
        }
        
        public override void Update() {
            if (agent.isActiveAndEnabled) {
                agent.SetDestination(enemy.GetFurthestPoint(target.position, enemy.safeRange + 0.5f)); 
            }
            base.Update();
        }
    }
}