using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game
{
    public class EnragedVesselFocusedState : EnragedVesselBaseState {
        NavMeshAgent agent;
        Transform target;
        
        CountdownTimer timer;
        
        public EnragedVesselFocusedState(EnragedVesselEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(
            enemy, animHash) {
            this.agent = agent;
            this.target = target;
        }
        
        public override void OnEnter() {
            base.OnEnter();
            agent.speed = 0f;
            timer = new CountdownTimer(enemy.Stats.ChargeDuration);
            timer.OnTimerStop += () => enemy.readyToAttack = true;
            timer.Start();
        }
    }
}