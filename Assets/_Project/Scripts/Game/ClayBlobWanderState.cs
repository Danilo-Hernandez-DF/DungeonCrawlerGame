using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Game {
    public class ClayBlobWanderState : ClayBlobBaseState {
        private readonly NavMeshAgent agent;
        private readonly Vector3 startPoint;
        private readonly float wanderRadius;

        public ClayBlobWanderState(ClayBlobEnemy enemy, NavMeshAgent agent, float wanderRadius, int animHash = 0) : base(enemy, animHash) {
            this.agent = agent;
            this.startPoint = enemy.transform.position;
            this.wanderRadius = wanderRadius;
        }

        public override void Update() {
            if(!HasReachedDestination()) return;
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPoint;
            NavMesh.SamplePosition(randomDirection, out var hit, wanderRadius, 1);
            var finalPosition = hit.position;

            agent.SetDestination(finalPosition);
            base.Update();
        }

        bool HasReachedDestination() {
            return !agent.pathPending 
            && agent.remainingDistance <= agent.stoppingDistance
            && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
