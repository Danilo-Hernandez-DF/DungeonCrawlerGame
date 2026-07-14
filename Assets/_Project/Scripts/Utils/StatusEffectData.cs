using System;
using Game;
using Localisation;
using UnityEngine;

namespace UtilsModule {
    public abstract class StatusEffectData : ScriptableObject {
        public string nameKey;
        public Color dmgColor;
        public Sprite icon;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);

        public virtual void OnTick(Entity entity, int tickCount, DamageSource source) { }
        public virtual void OnApply(Entity entity, DamageSource source, int duration) { }
        protected virtual void OnRemove(Entity entity, DamageSource source) { }
        public void OnExpire(Entity entity, DamageSource source) {
            OnRemove(entity, source); 
        }
        public virtual void OnDamage(Entity entity, int damage, DamageSource source) { }
        public virtual void OnHeal(Entity entity, int amount, DamageSource source) { }
        public virtual void OnDeath(Entity entity, DamageSource source) { }

        public StatusEffect GetStatusEffect(DamageSource source) => new StatusEffect(this, source);
        
        protected void ApplyEffect(Entity entity, ModifierEffect modifierEffect)
        {
            var modifier = modifierEffect.operatorType switch
            {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, modifierEffect.duration),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, modifierEffect.duration, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier);

            if (modifierEffect.type == StatType.Health) {
                entity.health.AddHealth(entity.Stats.Health - entity.health.currentHealth);
            }
        }
    }
}