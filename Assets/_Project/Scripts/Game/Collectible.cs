using UnityEngine;
using UtilsModule;

namespace Game {
    public abstract class Collectible : Entity, IEffector {
        public void Visit<T> (T visitable) where T : Component, IVisitable {
            if(visitable is Entity entity) {
                if(entity.gameObject.CompareTag("Player")) ApplyEffect(entity);
            }
        }

        public void OnTriggerEnter2D(Collider2D other) {
           other.GetComponent<IVisitable>()?.Accept(this);
           Destroy(gameObject);
        }

        public virtual void ApplyEffect(Entity entity) { }
    }
}