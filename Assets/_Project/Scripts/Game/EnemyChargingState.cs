using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game
{
    public class EnemyChargingState : EnemyBaseState {
        CountdownTimer timer;

        public EnemyChargingState(Enemy enemy, int animHash = 0) : base(enemy, animHash)
        {

        }
        
        public override void OnEnter() {
            base.OnEnter();
            timer = new CountdownTimer(enemy.Stats.ChargeDuration);
            timer.OnTimerStop += () => enemy.charged = true;
            timer.Start();
        }

        public override void Update() {
            timer.Tick(Time.deltaTime);
        }
    }
}