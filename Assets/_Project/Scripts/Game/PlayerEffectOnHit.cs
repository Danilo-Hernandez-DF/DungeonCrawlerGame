using System;
using System.Collections.Generic;
using Localisation;
using UnityEngine;
using UtilsModule;
using Random = UnityEngine.Random;

namespace Game
{
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/PlayerEffectOnHit")]
    public class PlayerEffectOnHit: DungeonModifier
    {
        [SerializeField] private StatusEffectData eligibleStatusSource;
        [SerializeField] private EntityData eligibleEntitySource;
        [SerializeField] private TagData eligibleSourceTag;
        
        [SerializeField] private StatusEffectData statusEffect;
        [SerializeField] private ModifierEffect modifierEffect;
        [SerializeField] private float chance;
        
        [SerializeField] private Color specialTextColor;
        private readonly string noEnemyKey = "no_enemy";
        private readonly string anything = "anything";
        
        public override string Description() {
            var descriptionKey = "description_player_effect_on_hit";//add
            var description = LocalisationSystem.GetLocalisedValue(descriptionKey);
            
            var enemyStr = LocalisationSystem.GetLocalisedValue(noEnemyKey);// an enemy // un enemigo
            if(eligibleEntitySource) {
                enemyStr = eligibleEntitySource.nameKey;
            }
            else if(eligibleSourceTag) {
                enemyStr = enemyStr.Replace("{}", eligibleSourceTag.name);
                enemyStr = enemyStr.TrimEnd(' ');
            }
            else if (eligibleStatusSource) {
                enemyStr = eligibleStatusSource.name;
            } else {
                enemyStr = enemyStr.Replace("{}", "");
                enemyStr = enemyStr.TrimEnd(' ');
            }
            //{} enemy
            //un enemigo {}

            string hexColor = ColorUtility.ToHtmlStringRGBA(specialTextColor);
            var text0 = $"<color=#{hexColor}>{enemyStr}</color>";
            
            var statusStr = "";
            if(statusEffect) {
                statusStr = statusEffect.name;
            }
            else {
                statusStr = modifierEffect.ToString();
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
            //Whenever Player is damaged by {0}, Player gains {1}~, sometimes.
            //Cuando Jugador es dañado por {0}, Jugador gana {1}~, algunas veces.
            
            return description;
        }
        
        public override void OnPlayerHit(DamageSource source) {
            if (eligibleEntitySource && eligibleEntitySource != source.entity?.entityData) return;
            if (eligibleStatusSource && eligibleStatusSource != source.statusEffect) return;
            if (eligibleSourceTag && !source.entity.entityData.tags.Exists(x => x.data == eligibleSourceTag)) return;
            
            if (Random.Range(0f, 100f) > chance) return;
            
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
            entity.Status.Add(statusEffect, statusEffect.source, statusEffect.duration);
        }
    }
}