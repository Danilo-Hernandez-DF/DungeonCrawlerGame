using Localisation;
using UnityEngine;

namespace UtilsModule {
    public abstract class StatusEffectData : ScriptableObject {
        public int duration = 0;
        public string nameKey;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);

        public virtual void OnTick(Entity entity, int tickCount) { }
        public virtual void OnApply(Entity entity) { }
        protected virtual void OnRemove(Entity entity) { }
        public void OnExpire(Entity entity) {
            OnRemove(entity); 
        }
        public virtual void OnDamage(Entity entity, int damage) { }
        public virtual void OnHeal(Entity entity, int amount) { }
        public virtual void OnDeath(Entity entity) { }

        public StatusEffect GetStatusEffect() => new StatusEffect(this);
    }
}