namespace Game
{
    public class RangedClayBaseState : EnemyBaseState<RangedClayEnemy> {
        // Animation hash constants
        public RangedClayBaseState(RangedClayEnemy enemy, int animHash = 0) : base(enemy, animHash) { }
    }
}