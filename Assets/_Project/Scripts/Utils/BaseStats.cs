using UnityEngine;

namespace UtilsModule {
    public abstract class BaseStats : ScriptableObject {
        public int attack = 2;
        public float speed = 4f;
        public int defense = 0;
        public int health = 10;
        public float DashForce = 10f;
        public float DashDuration = 0.5f;
        public float DashCooldown = 1.5f;
        public float AttackCooldown = 0.6f;
        public float AttackRange = 1f;
        public float AttackDistance = 0.75f;
        public float StunDuration = 0.5f;
    }
}