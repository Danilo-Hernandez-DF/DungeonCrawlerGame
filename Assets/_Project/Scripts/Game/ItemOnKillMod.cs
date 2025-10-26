using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/ItemOnKill")]
    public class ItemOnKillMod : DungeonModifier {
        [SerializeField] private Item item;
        [SerializeField] private float chance;
        [SerializeField] private int minCount;
        [SerializeField] private int maxCount;
        [SerializeField] private List<EntityData> eligibleEnemies;
        [SerializeField] private List<TagData> eligibleTags;
        
        [SerializeField] private List<StatusEffectData> eligibleStatusSource;
        [SerializeField] private List<EntityData> eligibleEntitySource;
        
        public override void OnEnemyDeath(Enemy enemy, DamageSource source) {
            if (eligibleEntitySource.Count != 0) {
                if (!eligibleEntitySource.Exists(x => x == source.entity?.entityData)) return;
            }
            
            if (eligibleStatusSource.Count != 0) {
                if (!eligibleStatusSource.Exists(x => x == source.statusEffect)) return;
            }

            if (eligibleEnemies.Count != 0) {
                if (!eligibleEnemies.Exists(x => x == enemy.entityData)) return;
            }

            if (eligibleTags.Count != 0) {
                if (!eligibleTags.Exists(x => enemy.entityData.tags.Exists(y => y.data == x))) return;
            }
            
            if (Random.Range(0f, 100f) > chance) return;
            
            PlayerDetector.GetPlayerComponent().GiveItem(new Item(item.data, addTags: item.tags),
                Random.Range(minCount, maxCount + 1));
        }
    }
}