using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class EnemyStunnedState : EnemyBaseSate {
        readonly NavMeshAgent agent;
        readonly Transform player;

        private CountdownTimer timer;

        public EnemyStunnedState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator) {
            this.agent = agent;
            this.player = player;
        }

        public override void OnEnter() {
            agent.speed = 0f;
            timer = new CountdownTimer(enemy.Stats.StunDuration);
            timer.OnTimerStop += () => enemy.wasStunned = false;
            timer.Start();
        }

        public override void OnExit() {
            agent.speed = enemy.Stats.Speed;
        }

        public override void Update() {
            timer.Tick(Time.deltaTime);
        }
    }
}
