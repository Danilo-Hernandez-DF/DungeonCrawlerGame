using System;
using System.Collections.Generic;
using Localisation;
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
        [SerializeField] private EntityData eligibleEnemy;
        [SerializeField] private TagData eligibleTag;
        [SerializeField] private Color specialTextColor;
        private readonly string noEnemyKey = "tag_enemy";
        
        
        public override string Description() {
            var description = LocalisationSystem.GetLocalisedValue(descriptionKey);
            
            var enemyStr = LocalisationSystem.GetLocalisedValue(noEnemyKey);// an enemy // un enemigo
            if(eligibleEnemy) {
                enemyStr = eligibleEnemy.nameKey;
            }
            else if(eligibleTag) {
                enemyStr = eligibleTag.name + " " + LocalisationSystem.GetLocalisedValue(noEnemyKey);
            }

            string hexColor = ColorUtility.ToHtmlStringRGBA(specialTextColor);
            var text0 = $"<color=#{hexColor}>{enemyStr}</color>";
            
            var statusStr = "";
            if(status) {
                statusStr = status.name;
            }
            else {
                statusStr = effect.ToString();
            }
            
            var text1 = $"<color=#{hexColor}>{statusStr}</color>";
            
            description = description.Replace("{0}", text0);
            description = description.Replace("{1}", text1);
            
            if (chance < 100) {
                description = description.Replace("~", "");
            }
            else {
                description = description.Split("~")[0];
            }
            //Whenever {0} spawns, it gains {1}~, {2} of the times.
            //Cuando {0} aparece, gana {1}~, el {2} de las veces.
            
            return description;
        }

        public override void OnEnemySpawn(Enemy enemy) {
            var matchesEnemy = eligibleEnemy == enemy.entityData;
            var matchesTag = enemy.entityData.tags.Exists(x => x.data == eligibleTag);

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
            entity.Status.Add(statusEffect, statusEffect.source, statusEffect.duration);
        }
    }
}