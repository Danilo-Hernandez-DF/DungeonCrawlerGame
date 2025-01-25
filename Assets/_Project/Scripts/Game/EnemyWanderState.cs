using UnityEngine;
using UnityEngine.AI;

namespace Game {
    public class EnemyWanderState : EnemyBaseSate {
        private readonly NavMeshAgent agent;
        private readonly Vector3 startPoint;
        private readonly float wanderRadius;

        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator) {
            this.agent = agent;
            this.startPoint = enemy.transform.position;
            this.wanderRadius = wanderRadius;
        }

        void Start() {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        public override void OnEnter() {
            //Debug.Log("Wander");
            //animator stuff
        }

        public override void Update()
        {
            if(!HasReachedDestination()) return;
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPoint;
            NavMesh.SamplePosition(randomDirection, out var hit, wanderRadius, 1);
            var finalPosition = hit.position;

            agent.SetDestination(finalPosition);
        }

        bool HasReachedDestination() {
            return !agent.pathPending 
            && agent.remainingDistance <= agent.stoppingDistance
            && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
