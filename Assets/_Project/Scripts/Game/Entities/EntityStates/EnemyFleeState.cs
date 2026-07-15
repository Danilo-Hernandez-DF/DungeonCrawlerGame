namespace Game
{
    public class EnemyFleeState : EnemyBaseState
    {
        Transform target;
        float fleeRange;

        public EnemyFleeState(Enemy enemy, Transform target, float fleeRange, int animHash = 0) : base(enemy, animHash)
        {
            this.target = target;
            this.fleeRange = fleeRange;
        }
        
        public override void Update() {
            enemy.MoveTowards(enemy.GetFurthestPoint(fleeRange + 0.5f, target.position)); 
            
            base.Update();
        }
    }
}