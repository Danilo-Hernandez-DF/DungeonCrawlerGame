using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnragedVesselChaseState : EnragedVesselBaseState {
        readonly NavMeshAgent agent;
        readonly Transform target;

        public EnragedVesselChaseState(EnragedVesselEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash) {
            this.agent = agent;
            this.target = target;
        }

        public override void Update() {
            if (!agent.isActiveAndEnabled) return;

            if (enemy.CanDetectTarget()) {
                agent.SetDestination(target.position);
                enemy.lastPlayerPosition = target; 
                
                enemy.rememberTimer.Reset();
                enemy.rememberTimer.Start();
            } else if(enemy.lastPlayerPosition) {
                agent.SetDestination(enemy.lastPlayerPosition.position);
            }
            base.Update();
        }
    }
}