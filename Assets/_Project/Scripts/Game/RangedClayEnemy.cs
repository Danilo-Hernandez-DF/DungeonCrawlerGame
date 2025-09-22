using System;
using StateMachines;
using UnityEngine;

namespace Game
{
    public class RangedClayEnemy : Enemy
    {
        [Header("RangedClayEnemy Settings")]
        [SerializeField] GameObject projectilePrefab;
        public GameObject damageArea;
        public float safeRange = 4.5f;

        protected override void InitStates()
        {
            var chaseState = new EnemyChaseState(this, target.transform);
            var chargeState = new EnemyChargingState(this);
            var attackState = new EnemyAttackState(this, target.transform);
            var stunnedState = new EnemyStunnedState(this);
            var FleeState = new EnemyFleeState(this, target.transform, safeRange);
            var deathState = new EnemyDeathState(this);

            
        }

        public override void Attack()
        {
            base.Attack();
        }
    }
}