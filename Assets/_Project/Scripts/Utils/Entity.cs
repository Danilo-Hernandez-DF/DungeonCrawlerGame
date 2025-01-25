using Game;
using UnityEngine;

namespace UtilsModule {
    public abstract class Entity : MonoBehaviour, IVisitable {
        [SerializeField] BaseStats baseStats; 
        public Health health;
        protected Rigidbody2D Rb {get; private set;}
        public Stats Stats {get; private set;}
        public Status Status {get; private set;}
        public Vector2 FacingDirection {get; protected set;}

        protected void Awake() {
            Stats = new Stats(new StatsMediator(), baseStats);
            Status = new Status(this);
            Rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            health?.Init(Stats.Health);
            FacingDirection = Vector2.right;
        }

        public void Update() {
            Stats.Mediator.Update(Time.deltaTime);
            Status.Update(Time.deltaTime);
        }

        public void ApplyForce(Vector2 force) {
            if(Rb == null) return;
            Rb.AddForce(force);
        }

        protected virtual void OnDeath() {
            Status.OnDeath();
            //Debug.Log($"{name} has reached 0 Hp");
        }

        protected virtual void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            Status.OnDamage(damage);
            GameManager.Instance.TrackStat(StatisticsTracker.TrackedStat.DamageDealt, damage);
            //Debug.Log($"{name} took {damage} damage");
        } 

        public void TakeDamage(int damage, bool ignoreDefense = false, GameObject dmgSource = null, bool ignoreKnockback = false) {
            if(!health) return;

            var isDead = !ignoreDefense ? health.TakeDamage(damage - Stats.Defense) : health.TakeDamage(damage);
            
            OnDamage(damage, dmgSource);
            if(isDead) OnDeath();
        }

        public void Heal(int amount) {
            if(health == null) return;

            health.Heal(amount);
            Status.OnHeal(amount);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}