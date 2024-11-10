using UnityEngine;
using UtilsModule;

namespace Game {
    public class Health : MonoBehaviour {

        [SerializeField] int maxHealth = 100;
        [SerializeField] FloatEventChannel healthChannel;

        int currentHealth;

        public bool IsDead => currentHealth <= 0;

        void Awake() {
            currentHealth = maxHealth;
        }

        void Start() {
            PublishHealthPercentage();
        }

        public bool TakeDamage(int damage) { 
            currentHealth -= damage;

            if(currentHealth <= 0) currentHealth = 0;

            PublishHealthPercentage();

            return currentHealth == 0;
        }

        public void Init(int health) {
            maxHealth = health;
            currentHealth = health;
            PublishHealthPercentage();
        }

        void PublishHealthPercentage() {
            if(healthChannel != null) {
                healthChannel.Invoke(currentHealth / (float)maxHealth);
            }
        }
    }
}