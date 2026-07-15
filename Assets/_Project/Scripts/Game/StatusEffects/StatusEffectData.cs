using Localisation;
using Utils;

namespace Game {
    [System.Serializable]
    public abstract class StatusEffectData : ScriptableObject, IEffector {
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

        public StatusEffect GetStatusEffect(DamageSource source) => new StatusEffect(this, source, 0);
        
        protected void ApplyEffect(Entity entity, ModifierEffect modifierEffect) => ((IEffector)this).ApplyEffect(entity, modifierEffect);

        public void Visit<T>(T visitable) where T : Component, IVisitable {
            throw new NotImplementedException();
        }
    }
}