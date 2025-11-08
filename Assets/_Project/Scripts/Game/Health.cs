using UnityEngine;
using UtilsModule;

namespace Game {
    public class Health : MonoBehaviour {

        public int maxHealth { get; private set;}
        [SerializeField] FloatEventChannel healthChannel;

        public int currentHealth { get; private set; }

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

        public void Heal(int amount) { 
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            PublishHealthPercentage();
        }

        public void Init(int health) {
            maxHealth = health;
            currentHealth = health;
            PublishHealthPercentage();
        }
        
        public void AddHealth(int health) {
            maxHealth += health;
            currentHealth += health;
            PublishHealthPercentage();
        }
        
        public void RemoveHealth(int health) {
            maxHealth -= health;
            if(currentHealth > maxHealth) currentHealth = maxHealth;
            PublishHealthPercentage();
        }

        void PublishHealthPercentage() {
            healthChannel?.Invoke(currentHealth / (float)maxHealth);
        }
    }
}