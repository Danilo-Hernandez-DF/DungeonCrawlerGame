using System;
using UnityEngine;
using UtilsModule;

namespace Game {
    public abstract class Collectible : Entity, IEffector {
        [SerializeField] StatusEffectData statusEffect;
        [SerializeField] ModifierEffect modifierEffect;

        public void Visit<T>(T visitable) where T : Component, IVisitable
        {
            if (visitable is not Entity entity) return;
            if (entity.gameObject.CompareTag("Player"))
            {
                if (statusEffect != null) ApplyStatus(entity, statusEffect.GetStatusEffect());
                if (modifierEffect.value > 0) ApplyEffect(entity, modifierEffect);

                GameManager.Instance.TrackEntity(entityData, new() { timesKilled = 1 });
                Destroy(gameObject);
            }
        }

        public void OnTriggerEnter2D(Collider2D other) {
           other.GetComponent<IVisitable>()?.Accept(this);
        }

        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect) {
            BasicStatModifier modifier = modifierEffect.operatorType switch {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, modifierEffect.duration),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, modifierEffect.duration, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier);
        }

        public void ApplyStatus(Entity entity, StatusEffect statusEffect)
        {
            entity.Status.Add(statusEffect);
        }
    }
}