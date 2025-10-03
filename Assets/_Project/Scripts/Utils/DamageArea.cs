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
        
        [SerializeField] List<TagData> filter = new List<TagData>();
        [SerializeField] bool ignoreSelf = true;
        [SerializeField] bool isFiltered = false;
        [SerializeField] bool isWhiteList = false;
        [SerializeField] bool ignorePlayer = false;

        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect) {
            foreach(var modifier in hitEffects.Select(hitEffect => hitEffect.operatorType switch {
                         OperatorType.Add => new BasicStatModifier(hitEffect.type, v => v + hitEffect.value, hitEffect.duration),
                         OperatorType.Multiply => new BasicStatModifier(hitEffect.type, v => v, hitEffect.duration, hitEffect.value),
                         _ => throw new ArgumentOutOfRangeException()
                     })) {
                entity.Stats.Mediator.AddModifier(modifier);
            }
        }

        public void ApplyStatus(Entity entity, StatusEffect statusEffect) {
            foreach(StatusEffectData hitEffect in hitStatus) {
                entity.Status.Add(new StatusEffect(hitEffect));
            }
        }

        public void Visit<T>(T visitable) where T : Component, IVisitable {
            if(visitable is not Entity entity) return;
            if(affected.Contains(entity)) return;
            affected.Add(entity);
            if(hitEffects?.Count > 0) ApplyEffect(entity, null);
            if(hitStatus?.Count > 0) ApplyStatus(entity, null);
            
            if(damage <= 0) return;
            entity.TakeDamage(damage, dmgSource: playerDamage ? PlayerDetector.GetPlayer() : gameObject);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if(!other.gameObject.TryGetComponent(out Entity entity)) return;
            if(ignoreSelf && entity.gameObject == gameObject) return;
            if(ignorePlayer && entity.gameObject.CompareTag("Player")) return;
            if (!isFiltered) {
                Visit(entity);
                return;
            }

            foreach(TagData filterTag in filter) {
                if (!entity.entityData.HasTag(filterTag.name)) continue;
                if (!isWhiteList) continue;
                Visit(entity);
                return;
            }

            if (!isWhiteList) { 
                Visit(entity);
            }
        }
    } 
}