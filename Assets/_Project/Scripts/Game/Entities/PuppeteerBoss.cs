using System.Collections;
using Utils;

namespace Game
{
    public class PuppeteerBoss: Enemy
    {
        [SerializeField] private Transform[] projectilePoints;
        [SerializeField] private GameObject projectilePrefab;
        protected override void InitStates()
        {
            if (stateMachine == null) {
                return;
            }
            
            SetTarget(PlayerDetector.GetPlayerComponent());

            var idleState = new PuppeteerIdleState(this);
            var spinWaitState = new WaitState(this, 0.5f);
            var spinAttackState = new PuppeteerSpinAttackState(this);
            
            At(idleState, spinWaitState, new FuncPredicate(() => true));
            At(spinWaitState, spinAttackState, new FuncPredicate(() => waited));
            At(spinAttackState, idleState, new FuncPredicate(() => charged));
            
            stateMachine.SetState(idleState);
        }
        
        public void SpinAttack() {
            charged = false;
            StartCoroutine(SpinAttackCoroutine());
        }
        
        private IEnumerator SpinAttackCoroutine() {
            for (var i = 0; i < 2; i++) {
                foreach (var point in projectilePoints) {
                    var projectile = Instantiate(projectilePrefab, point.position, Quaternion.identity);
                    projectile.GetComponent<Projectile>().SetTarget((point.right * 5f) + transform.position);
                    yield return new WaitForSeconds(0.05f);
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            
            foreach (var point in projectilePoints) {
                var projectile = Instantiate(projectilePrefab, point.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().SetTarget((point.right * 5f) + transform.position);
            }

            charged = true;
        }

        public void SwipeAttack() {
            
        }
        
        public void ChargeAttack() {
            
        }
        
        public void HomingAttack() {
            
        }
        
        public void Chase() {
            
        }
    }
}