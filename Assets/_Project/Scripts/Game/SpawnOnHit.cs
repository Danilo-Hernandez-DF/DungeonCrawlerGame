using System.Collections.Generic;
using Localisation;
using UnityEngine;
using UtilsModule;

namespace Game
{
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/SpawnOnHit")]
    public class SpawnOnHit : DungeonModifier
    {
        [SerializeField] private EntityData spawnEntity;
        [SerializeField] private float radius;
        [SerializeField] private float innerRadius;
        
        [SerializeField] private float chance;
        [SerializeField] private TagData eligibleTag;
        [SerializeField] private StatusEffectData eligibleStatusSource;
        [SerializeField] private EntityData eligibleEntitySource;
        
        [SerializeField] private Color specialTextColor;
        
        public override void OnPlayerHit(DamageSource source) {
            var matchesSource = eligibleEntitySource == source.entity?.entityData;
            var matchesStatusSource = eligibleStatusSource == source.statusEffect;
            var matchesTagSource = source.entity?.entityData.tags.Exists(x => x.data == eligibleTag) ?? true;
            
            if (!matchesSource && !matchesStatusSource && !matchesTagSource) return;
            
            if (Random.Range(0f, 100f) > chance) return;
            
            Vector2 spawnPosition = PlayerDetector.GetPlayerComponent().transform.position;
            spawnPosition += Random.insideUnitCircle * Random.Range(innerRadius, radius);
            
            var spawn = Instantiate(spawnEntity.prefab, spawnPosition, Quaternion.identity);
        }
        
        public override string Description() {
            var descriptionKey = "spawn_on_hit_description";
            var enemyKey = "tag_enemy";
            var anySourceKey = "any_source";
            var description = LocalisationSystem.GetLocalisedValue(descriptionKey);

            var enemyStr = "";
            if(eligibleEntitySource) {
                enemyStr = eligibleEntitySource.nameKey;
            }
            else if(eligibleTag) {
                enemyStr = eligibleTag.name + " " + LocalisationSystem.GetLocalisedValue(enemyKey);
            }
            else {
                enemyStr = LocalisationSystem.GetLocalisedValue(anySourceKey);
            }

            string hexColor = ColorUtility.ToHtmlStringRGBA(specialTextColor);
            var text0 = $"<color=#{hexColor}>{enemyStr}</color>";
            
            description = description.Replace("{0}", text0);
            
            if (chance < 100) {
                description = description.Replace("~", "");
            }
            else {
                description = description.Split("~")[0];
            }
            
            description = description.Replace("{1}", $"<color=#{hexColor}>{spawnEntity.name}</color>");
            
            //Whenever Player takes damage from {0}, {1}~ sometimes spawn around Player.
            //Cuando Jugador toma daño por {0}, {1}~ aparece alrededor del Jugador algunas veces.
            
            return description;
        }

        public override List<EntityData> GetEnemies() {
            var enemies = new List<EntityData>();
            if(spawnEntity) {
                enemies.Add(spawnEntity);
            }
            return enemies;
        }
    }
}