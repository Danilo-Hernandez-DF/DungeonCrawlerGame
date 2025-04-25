using UtilsModule;

namespace Game
{
    public class EnragedVesselDeathState : EnragedVesselBaseState {
        CountdownTimer timer;
        public EnragedVesselDeathState(EnragedVesselEnemy enemy, int animHash = 0) : base(enemy, animHash) { }
        
        public override void OnEnter() {
            base.OnEnter();
            enemy.gameObject.AddComponent<DestroyAfter>().time = 2f;
        }
    }
}