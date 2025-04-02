using UnityEngine;
using UnityEngine.Serialization;

namespace UtilsModule {
    public abstract class BaseStats : ScriptableObject {
        public int attack = 2;
        public float speed = 4f;
        public int defense = 0;
        public int health = 10;
        public float dashForce = 10f;
        public float dashDuration = 0.5f;
        public float dashCooldown = 1.5f;
        public float attackCooldown = 0.6f;
        public float attackRange = 1f;
        public float attackDistance = 0.75f;
        public float stunDuration = 0.5f;
        public float chargeDuration = 0.75f;
    }
}