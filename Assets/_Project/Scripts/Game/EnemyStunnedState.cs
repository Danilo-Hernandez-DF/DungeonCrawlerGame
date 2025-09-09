using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class EnemyStunnedState : EnemyBaseState {
        private CountdownTimer timer;

        public EnemyStunnedState(Enemy enemy, int animHash = 0) : base(enemy, animHash)
        {
           
        }

        public override void OnEnter() {
            base.OnEnter();

            timer = new CountdownTimer(enemy.Stats.StunDuration);
            timer.OnTimerStop += () => enemy.wasStunned = false;
            timer.Start();
        }

        public override void OnExit() {
            base.OnExit();
            
        }

        public override void Update() {
            base.Update();
            timer.Tick(Time.deltaTime);
        }
    }
}
