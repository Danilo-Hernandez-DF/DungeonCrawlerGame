using UnityEngine;
using UnityEngine.Serialization;

namespace UtilsModule {
    public abstract class BaseStats : ScriptableObject {
        public int attack = 2;
        public float speed = 4f;
        public int defense = 0;
        public int health = 10;
        [FormerlySerializedAs("DashForce")] public float dashForce = 10f;
        [FormerlySerializedAs("DashDuration")] public float dashDuration = 0.5f;
        [FormerlySerializedAs("DashCooldown")] public float dashCooldown = 1.5f;
        [FormerlySerializedAs("AttackCooldown")] public float attackCooldown = 0.6f;
        [FormerlySerializedAs("AttackRange")] public float attackRange = 1f;
        [FormerlySerializedAs("AttackDistance")] public float attackDistance = 0.75f;
        [FormerlySerializedAs("StunDuration")] public float stunDuration = 0.5f;
    }
}