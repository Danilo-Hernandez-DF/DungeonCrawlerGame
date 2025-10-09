using Localisation;
using UnityEngine;

namespace UtilsModule {
    public abstract class StatusEffectData : ScriptableObject {
        public int duration = 0;
        public string nameKey;
        public Color dmgColor;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);

        public virtual void OnTick(Entity entity, int tickCount, DamageSource source) { }
        public virtual void OnApply(Entity entity, DamageSource source) { }
        protected virtual void OnRemove(Entity entity, DamageSource source) { }
        public void OnExpire(Entity entity, DamageSource source) {
            OnRemove(entity, source); 
        }
        public virtual void OnDamage(Entity entity, int damage, DamageSource source) { }
        public virtual void OnHeal(Entity entity, int amount, DamageSource source) { }
        public virtual void OnDeath(Entity entity, DamageSource source) { }

        public StatusEffect GetStatusEffect(DamageSource source) => new StatusEffect(this, source);
    }
}