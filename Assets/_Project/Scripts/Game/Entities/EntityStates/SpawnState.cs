using Utils;

namespace Game
{
    public class SpawnState: EnemyBaseState
    {
        private CountdownTimer spawnTimer;

        public SpawnState(Enemy enemy, float spawnTime, int animHash = 0) : base(enemy, animHash) {
            spawnTimer = new CountdownTimer(spawnTime);
        }
        
        public override void OnEnter() {
            base.OnEnter();
            enemy.spawned = false;
            spawnTimer.OnTimerStop += () => enemy.spawned = true;
            spawnTimer.Start();
        }
        
        public override void Update() {
            spawnTimer.Tick(Time.deltaTime);
        }
    }
}