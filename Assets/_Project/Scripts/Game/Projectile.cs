using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilsModule;

namespace Game
{
    public class Projectile : MonoBehaviour, IEffector {
        [SerializeField] private float speed = 10f;
        [SerializeField] private int piercing = 0;
        private int pierced = 0;
        [SerializeField] private int damage = 1;
        [SerializeField] LayerMask collidable;
        private Vector3 target;
        [SerializeField] private List<ModifierEffect> hitEffects = new List<ModifierEffect>();
        [SerializeField] private List<StatusEffectData> hitStatus = new List<StatusEffectData>();
        
        [Header("Filter Affected")]
        [SerializeField] List<TagData> filterTags = new List<TagData>();
        [SerializeField] private bool isFiltered;
        [SerializeField] private bool isWhiteList;
        
        private List<Entity> affectedEntities = new List<Entity>();
        
        public void SetTarget(Vector3 target) {
            this.target = target;
        }
        
        void Update() {
            if(target == null) return;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if(transform.position == target) Destroy(gameObject);
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if(collidable.value == (collidable.value | 1 << other.gameObject.layer)) {
                Entity entity = other.GetComponent<Entity>();
                if (entity == null) {
                    Destroy(gameObject);
                    return;
                }
                
                if(affectedEntities.Contains(entity)) return;
                
                if(isFiltered) {
                    if(isWhiteList) {
                        if(!filterTags.Any(x => entity.entityData.HasTag(x.name))) return;
                        Visit(entity);
                        pierced++;
                    } else {
                        if(filterTags.Any(x => entity.entityData.HasTag(x.name))) return;
                        Visit(entity);
                        pierced++;
                    }
                } else {
                    Visit(entity);
                    pierced++;
                }
                
                if(pierced >= piercing) {
                    Destroy(gameObject);
                }
            }
        }

        public void Visit<T>(T visitable) where T : Component, IVisitable {
            if(visitable is not Entity entity) return;
            affectedEntities.Add(entity);
            entity.TakeDamage(damage);
            ApplyEffect(entity, null);
            ApplyStatus(entity, null);
        }
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
    }
}