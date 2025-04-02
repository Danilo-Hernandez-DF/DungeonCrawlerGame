using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class ClayBlobStunnedState : EnemyBaseState<ClayBlobEnemy> {
        readonly NavMeshAgent agent;
        readonly Transform target;

        private CountdownTimer timer;

        public ClayBlobStunnedState(ClayBlobEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash) {
            this.agent = agent;
            this.target = target;
        }

        public override void OnEnter() {
            base.OnEnter();
            agent.speed = 0f;
            timer = new CountdownTimer(enemy.Stats.StunDuration);
            timer.OnTimerStop += () => enemy.wasStunned = false;
            timer.Start();
        }

        public override void OnExit() {
            base.OnExit();
            agent.speed = enemy.Stats.Speed;
        }

        public override void Update() {
            base.Update();
            timer.Tick(Time.deltaTime);
        }
    }
}
