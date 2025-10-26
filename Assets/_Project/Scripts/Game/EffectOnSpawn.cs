using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsModule;
using Random = UnityEngine.Random;

namespace Game
{
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/EffectOnSpawn")]
    public class EffectOnSpawn: DungeonModifier, IEffector
    {
        [SerializeField] private ModifierEffect effect;
        [SerializeField] private StatusEffectData status;
        [SerializeField] private float chance;
        [SerializeField] private List<EntityData> eligibleEnemies;
        [SerializeField] private List<TagData> eligibleTags;

        public override void OnEnemySpawn(Enemy enemy) {
            var matchesEnemy = eligibleEnemies?.Count == 0 || (eligibleEnemies?.Exists(x => x == enemy.entityData) ?? true);
            var matchesTag = (eligibleTags?.Exists(x => enemy.entityData.tags.Exists(y => y.data == x)) ?? false);

            if (!matchesEnemy && !matchesTag) return;
            if (Random.Range(0f, 100f) > chance) return;
            Visit(enemy);
        }

        public void Visit<T>(T visitable) where T : Component, IVisitable {
            if (visitable is not Entity entity) return;
            if(effect.value > 0) ApplyEffect(entity, effect);
            if(status) ApplyStatus(entity, status.GetStatusEffect(new DamageSource(null)));
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