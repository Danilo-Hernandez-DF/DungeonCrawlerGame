namespace Game
{
    public class PuppeteerSpinAttackState: PuppeteerState
    {
        public PuppeteerSpinAttackState(PuppeteerBoss enemy, int animHash = 0) : base(enemy, animHash) { }
        
        public override void OnEnter() {
            enemy.SpinAttack();
        }
    }
}