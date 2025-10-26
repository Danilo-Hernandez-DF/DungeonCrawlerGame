using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsModule;

namespace Game
{
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/PlayerEffectOnHit")]
    public class PlayerEffectOnHit: DungeonModifier
    {
        [SerializeField] private List<StatusEffectData> eligibleStatusSource;
        [SerializeField] private List<EntityData> eligibleEntitySource;
        
        [SerializeField] private StatusEffectData statusEffect;
        [SerializeField] private ModifierEffect modifierEffect;
        
        public override void OnPlayerHit(DamageSource source) {
            if (eligibleEntitySource.Count != 0) {
                if (!eligibleEntitySource.Exists(x => x == source.entity?.entityData)) return;
            }
            
            if (eligibleStatusSource.Count != 0) {
                if (!eligibleStatusSource.Exists(x => x == source.statusEffect)) return;
            } else if (source.statusEffect) {
                return;
            }
            
            Visit(PlayerDetector.GetPlayerComponent());
        }
        
        public void Visit<T>(T visitable) where T : Component, IVisitable {
            if (visitable is not Entity entity) return;
            if(modifierEffect.value > 0) ApplyEffect(entity, modifierEffect);
            if(statusEffect) ApplyStatus(entity, statusEffect.GetStatusEffect(new DamageSource(null)));
        }
        
        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect)
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

        public void ApplyStatus(Entity entity, StatusEffect statusEffect)
        {
            entity.Status.Add(statusEffect, statusEffect.source);
        }
    }
}