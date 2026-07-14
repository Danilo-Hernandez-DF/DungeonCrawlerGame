using Game;
using UnityEngine;

namespace UtilsModule
{
    [CreateAssetMenu(menuName = "Data/StatusEffectData/Burned")]
    public class BurnedEffect : StatusEffectData {
        [SerializeField] int damage = 1;
        [SerializeField] float speedDecreaseMult = -10;
        
        public override void OnApply(Entity entity, DamageSource source, int duration) {
            ApplyEffect(entity, new ModifierEffect(OperatorType.Multiply, StatType.Speed, speedDecreaseMult, duration));
        }
        
        public override void OnTick(Entity entity, int tickCount, DamageSource source) {
            source.statusEffect = this;
            if(tickCount % 6 == 0) entity.TakeDamage(damage, source, true, ignoreKnockback: true);
        }
    }
}