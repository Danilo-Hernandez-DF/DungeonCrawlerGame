using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    internal class RangedClayAttackState : RangedClayBaseState
    {
        public RangedClayAttackState(RangedClayEnemy enemy, NavMeshAgent agent, Transform target, int animHash = 0) : base(enemy, animHash)
        {
            
        }
    }
}