using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game
{
    public class RangedClayChargeState : RangedClayBaseState
    {
        readonly NavMeshAgent agent;
        readonly Transform target;
        private CountdownTimer chargeTimer = new CountdownTimer(0.5f);

        public RangedClayChargeState(RangedClayEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash)
        {
            this.agent = agent;
            this.target = target;
        }

        public override void OnEnter()
        {
            if (agent.isActiveAndEnabled)
            {
                GameObject damageArea = GameObject.Instantiate(enemy.damageArea, enemy.transform.position, Quaternion.identity);
                damageArea.transform.SetParent(enemy.transform);
                damageArea.transform.localPosition = Vector3.zero;
                agent.SetDestination(target.position);
                agent.speed = enemy.Stats.DashForce;
                agent.isStopped = false;
                chargeTimer.Reset();
                chargeTimer.OnTimerStop += () =>
                {
                    enemy.endedCharge = true;
                    GameObject.Destroy(damageArea);
                    enemy.TakeDamage(enemy.Stats.Attack, dmgSource: enemy.gameObject);
                };
                chargeTimer.Start();
            }
            base.OnEnter();
        }
    }
}