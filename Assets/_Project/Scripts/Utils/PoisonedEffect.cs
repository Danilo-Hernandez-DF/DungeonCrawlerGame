using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(menuName = "Data/StatusEffectData/Poisoned")]
    public class PoisonedEffect : StatusEffectData {
        [SerializeField] int damage = 1;
        public override void OnTick(Entity entity, int tickCount, DamageSource source) {
            source.statusEffect = this;
            if(tickCount % 4 == 0) entity.TakeDamage(damage, source, true, ignoreKnockback: true);
        }
    }
}