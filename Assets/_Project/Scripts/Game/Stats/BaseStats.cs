namespace Game {
    public abstract class BaseStats : ScriptableObject
    {
        [Header("Universal Stats")]
        public int attack = 2;
        public float speed = 4f;
        public int defense = 0;
        public int health = 10;
        public float attackRange = 1f;

        [Header("Enemy Stats")]
        public float stunDuration = 0.5f;
        public float chargeDuration = 0.75f;
        public float fleeRange = 0;
        public float detectionRange = 3f;
        public float knockback = 1f;

        [Header("Specific Stats")]
        public float dashForce = 10f;
        public float dashDuration = 0.5f;
        public float dashCooldown = 1.5f;
        public float attackCooldown = 0.6f;
        public float attackDistance = 0.75f;
    }
}