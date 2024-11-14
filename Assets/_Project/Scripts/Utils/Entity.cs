using Game;
using UnityEngine;

namespace UtilsModule {
    public abstract class Entity : MonoBehaviour, IVisitable {
        [SerializeField] BaseStats baseStats; 
        public Health health;
        public Rigidbody2D rb {get; private set;}
        public Stats Stats {get; private set;}
        public Status Status {get; private set;}
        public Vector2 facingDirection {get; protected set;}

        protected void Awake() {
            Stats = new Stats(new StatsMediator(), baseStats);
            Status = new Status(this);
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            health?.Init(Stats.Health);
            facingDirection = Vector2.right;
        }

        public void Update() {
            Stats.Mediator.Update(Time.deltaTime);
            Status.Update(Time.deltaTime);
        }

        public void ApplyForce(Vector2 force) {
            if(rb == null) return;
            rb.AddForce(force);
        }

        protected virtual void OnDeath() {
            Status.OnDeath();
            Debug.Log($"{name} has reached 0 Hp");
        }

        protected virtual void OnDamage(int damage, GameObject dmgSource = null) {
            Status.OnDamage(damage);
            Debug.Log($"{name} took {damage} damage");
        } 

        public void TakeDamage(int damage, bool ignoreDefense = false, GameObject dmgSource = null) {
            if(health == null) return;

            bool isDead;

            if(!ignoreDefense) isDead = health.TakeDamage(damage - Stats.Defense);
            else isDead = health.TakeDamage(damage);

            if(isDead) { 
                OnDeath();
            } else {
                OnDamage(damage, dmgSource);
            }
        }

        public void Heal(int amount) {
            if(health == null) return;

            health.Heal(amount);
            Status.OnHeal(amount);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}