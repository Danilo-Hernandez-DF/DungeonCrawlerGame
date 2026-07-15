using Utils;

namespace Game {
    public abstract class Collectible : Entity, IEffector {
        [SerializeField] StatusEffectData statusEffect;
        [SerializeField] ModifierEffect modifierEffect;

        public void Visit<T>(T visitable) where T : Component, IVisitable
        {
            if (visitable is not Entity entity) return;
            if (entity.gameObject.CompareTag("Player"))
            {
                if (statusEffect != null) ApplyStatus(entity, statusEffect.GetStatusEffect(new DamageSource(gameObject)));
                if (modifierEffect.value > 0) ApplyEffect(entity, modifierEffect);

                GameManager.Instance.TrackEntity(entityData, new() { timesKilled = 1 });
                Destroy(gameObject);
            }
        }

        public void OnTriggerEnter2D(Collider2D other) {
           other.GetComponent<IVisitable>()?.Accept(this);
        }

        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect) => ((IEffector)this).ApplyEffect(entity, modifierEffect);

        public void ApplyStatus(Entity entity, StatusEffect statusEffect) => ((IEffector)this).ApplyStatus(entity, statusEffect);
    }
}