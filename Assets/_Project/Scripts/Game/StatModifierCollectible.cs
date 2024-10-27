using System;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class StatModifierCollectible : Collectible {
        [SerializeField] ModifierEffect modifierEffect;

        public override void ApplyEffect(Entity entity) {
            StatModifier modifier = modifierEffect.operatorType switch {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, modifierEffect.duration),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, modifierEffect.duration, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier);
        }
    }
}