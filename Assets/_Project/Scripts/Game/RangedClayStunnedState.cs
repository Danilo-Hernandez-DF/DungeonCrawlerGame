using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game
{
    public class RangedClayStunnedState : RangedClayBaseState {
        readonly NavMeshAgent agent;
        private CountdownTimer timer;

        public RangedClayStunnedState(RangedClayEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash) {
            this.agent = agent;
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