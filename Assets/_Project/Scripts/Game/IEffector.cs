using Utils;

namespace Game {
    public interface IEffector : IVisitor {
        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect, string id = "") {
            BasicStatModifier modifier = modifierEffect.operatorType switch {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, modifierEffect.duration),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, modifierEffect.duration, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier, id);
            
            if (modifierEffect.type == StatType.Health) {
                entity.health.AddHealth(entity.Stats.Health - entity.health.currentHealth);
            }
        }

        public void ApplyStatus(Entity entity, StatusEffect statusEffect, string id = "") {
            entity.Status.Add(statusEffect, statusEffect.source, statusEffect.duration, id);
        }
    }
}