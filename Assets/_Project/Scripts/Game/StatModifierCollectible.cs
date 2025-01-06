using System;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class StatModifierCollectible : Collectible {
        [SerializeField] ModifierEffect modEffect;

        public override void ApplyEffect(Entity entity, ModifierEffect modifierEffect) {
            BasicStatModifier modifier = modEffect.operatorType switch {
                OperatorType.Add => new BasicStatModifier(modEffect.type, v => v + modEffect.value, modEffect.duration),
                OperatorType.Multiply => new BasicStatModifier(modEffect.type, v => v, modEffect.duration, modEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier);
        }
    }
}