using UnityEngine;
using UtilsModule;

namespace Game {
    public abstract class Collectible : Entity, IEffector {
        public void Visit<T> (T visitable) where T : Component, IVisitable {
            if(visitable is not Entity entity) return;
            if(entity.gameObject.CompareTag("Player")) ApplyEffect(entity, null);
        }

        public void OnTriggerEnter2D(Collider2D other) {
           other.GetComponent<IVisitable>()?.Accept(this);
           GameManager.Instance.TrackEntity(entityData, new() {timesKilled = 1});
           Destroy(gameObject);
        }

        public virtual void ApplyEffect(Entity entity, ModifierEffect modifierEffect) { }
        public virtual void ApplyStatus(Entity entity, StatusEffect statusEffect) { }
    }
}