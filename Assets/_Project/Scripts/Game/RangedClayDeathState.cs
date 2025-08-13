using UtilsModule;

namespace Game
{
    public class RangedClayDeathState : RangedClayBaseState
    {
        public RangedClayDeathState(RangedClayEnemy enemy, int animHash = 0) : base(enemy, animHash) { }
        
        public override void OnEnter() {
            base.OnEnter();
            enemy.gameObject.AddComponent<DestroyAfter>().time = 0.2f;
        }
    }
}