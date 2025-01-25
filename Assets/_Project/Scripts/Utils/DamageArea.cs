using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class DamageArea : MonoBehaviour, IEffector {
        public int damage;
        public List<ModifierEffect> hitEffects;
        public List<StatusEffectData> hitStatus;
        public Transform origin;
        public bool playerDamage = false;
        readonly List<Entity> affected = new List<Entity>();

        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect) {
            foreach(var modifier in hitEffects.Select(hitEffect => hitEffect.operatorType switch {
                         OperatorType.Add => new BasicStatModifier(hitEffect.type, v => v + hitEffect.value, hitEffect.duration),
                         OperatorType.Multiply => new BasicStatModifier(hitEffect.type, v => v, hitEffect.duration, hitEffect.value),
                         _ => throw new ArgumentOutOfRangeException()
                     })) {
                entity.Stats.Mediator.AddModifier(modifier);
            }
        }

        private void ApplyStatus(Entity entity) {
            foreach(StatusEffectData hitEffect in hitStatus) {
                entity.Status.Add(new StatusEffect(hitEffect));
            }
        }

        public void Visit<T>(T visitable) where T : Component, IVisitable {
            if(visitable is not Entity entity) return;
            if(affected.Contains(entity)) return;
            affected.Add(entity);
            if(hitEffects?.Count > 0) ApplyEffect(entity, null);
            if(hitStatus?.Count > 0) ApplyStatus(entity);
            entity.TakeDamage(damage, dmgSource: gameObject);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if(!other.gameObject.TryGetComponent(out Entity entity)) return;
            if(!playerDamage && entity.gameObject.CompareTag("Player")) return;
            Visit(entity);
        }
    } 
}