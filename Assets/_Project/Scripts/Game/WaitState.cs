using UnityEngine;
using UtilsModule;

namespace Game
{
    public class WaitState: EnemyBaseState
    {
        private CountdownTimer waitTimer;

        public WaitState(Enemy enemy, float waitTime, int animHash = 0) : base(enemy, animHash) {
            waitTimer = new CountdownTimer(waitTime);
        }
        
        public override void OnEnter() {
            base.OnEnter();
            enemy.waited = false;
            waitTimer.OnTimerStop += () => enemy.waited = true;
            waitTimer.Start();
        }
        
        public override void Update() {
            waitTimer.Tick(Time.deltaTime);
        }
    }
}