using UnityEngine;
using UnityEngine.AI;

namespace Game {
    public class EnemyChaseState : EnemyBaseSate {
        readonly NavMeshAgent agent;
        readonly Transform player;

        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator) {
            this.agent = agent;
            this.player = player;
        }

        public override void OnEnter() {
            Debug.Log("Chase");
            //animator stuff
        }

        public override void Update() {
            agent.SetDestination(player.position);
        }
    }
}
