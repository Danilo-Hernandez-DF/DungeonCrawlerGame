using UtilsModule;

namespace Game
{
    public class EnemyDeathState : EnemyBaseState {
        float deathTime;
        public EnemyDeathState(Enemy enemy, float deathTime = 0, int animHash = 0) : base(enemy, animHash)
        { 
            this.deathTime = deathTime;
        }
        
        public override void OnEnter() {
            base.OnEnter();
            enemy.gameObject.AddComponent<DestroyAfter>().time = deathTime;
        }
    }
}