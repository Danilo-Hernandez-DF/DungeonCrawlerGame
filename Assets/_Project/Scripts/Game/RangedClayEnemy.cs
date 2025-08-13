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
        public bool endedCharge = false;

        protected override void InitStates()
        {
            var chaseState = new RangedClayChaseState(this, agent, playerDetector.Player);
            var chargeState = new RangedClayChargeState(this, agent, playerDetector.Player);
            var attackState = new RangedClayAttackState(this, agent, playerDetector.Player);
            var stunnedState = new RangedClayStunnedState(this, agent, playerDetector.Player);
            var FleeState = new RangedClayFleeState(this, agent, playerDetector.Player);
            var deathState = new RangedClayDeathState(this);

            
        }

        protected override void OnDeath()
        {

        }

        protected override void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false)
        {

        }

        public override void Attack()
        {

        }

        private bool HasLineOfSight()
        {
            return playerDetector.HasLineOfSight(transform.position) && CanDetectTarget();
        }

        public bool CanDetectTarget() => playerDetector.CanDetectPlayer(FacingDirection);

        private bool KnowsPlayerPosition() => playerDetector.CanDetectPlayer(FacingDirection);
    }
}