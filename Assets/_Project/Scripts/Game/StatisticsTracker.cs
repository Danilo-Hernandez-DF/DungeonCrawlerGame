using System;
using System.Collections.Generic;
using UnityEngine;
using UtilsModule;

namespace Game {
    [Serializable]
    public class StatisticsTracker {
        public enum TrackedStat {
            KilledEnemies,
            LootablesOpened,
            CollectiblesCollected,
            RoomsCleared,
            DamageTaken,
            DamageDealt,
            ItemsFound
        }

        public Dictionary<TrackedStat, int> trackedStats;
        public Dictionary<ItemData, int> trackedItems;
        public Dictionary<EntityData, EntityTrack> trackedEntities;
        public Dictionary<string, int> trackedLoot;
        public string name;
        public Quest quest;
        public NPC questGiver;
        
        public StatisticsTracker Copy() {
            var tracker = new StatisticsTracker(name) {
                trackedStats = trackedStats,
                trackedItems = trackedItems,
                trackedEntities = trackedEntities,
                trackedLoot = trackedLoot
            };

            return tracker;
        }
        
        public bool GoalAchieved() {
            if(!quest || quest.objectives.Length == 0) return false;
            foreach(Objective objective in quest.objectives) {
                if(objective.GetProgress(this) > 0) return false;
            }

            return true;
        }
        
        public void TrackStat(TrackedStat stat, int value) {
            if(trackedStats.TryGetValue(stat, out int currentValue)) {
                trackedStats[stat] = currentValue + value;
            } else {
                trackedStats.Add(stat, value);
            }
        }
        
        public void TrackItem(ItemData stat, int value) {
            if(trackedItems.TryGetValue(stat, out int currentValue)) {
                trackedItems[stat] = currentValue + value;
            } else {
                trackedItems.Add(stat, value);
            }
            TrackStat(TrackedStat.ItemsFound, value);
        }
        
        public void TrackLoot(string stat, int value = 1) {
            if(trackedLoot.TryGetValue(stat, out int currentValue)) {
                trackedLoot[stat] = currentValue + value;
            } else {
                trackedLoot.Add(stat, value);
            }
            TrackStat(TrackedStat.LootablesOpened, value);
        }
        
        public void TrackEntity(EntityData stat, EntityTrack value = default, GameObject source = null) {
            if(stat.HasTag("tag_lootable")) {
                TrackLoot(stat.nameKey);
                return;
            }
            
            if(trackedEntities.TryGetValue(stat, out EntityTrack currentValue)) {
                trackedEntities[stat] = currentValue + value;
            } else {
                trackedEntities.Add(stat, value);
            }

            if(stat.HasTag("tag_enemy")) {
                TrackStat(TrackedStat.KilledEnemies, value.timesKilled);
            } else if(stat.HasTag("tag_collectible")) {
                TrackStat(TrackedStat.CollectiblesCollected, value.timesKilled);
            }
        }

        public int GetTracked(ItemData stat) {
            return trackedItems.GetValueOrDefault(stat, 0);
        }
        
        public int GetTracked(TrackedStat stat) {
            return trackedStats.GetValueOrDefault(stat, 0);
        }

        public EntityTrack GetTracked(EntityData stat) {
            return trackedEntities.GetValueOrDefault(stat, new());
        }
        
        public StatisticsTracker(string name, Quest quest = null, NPC questGiver = null) {
            this.name = name;
            Reset();
            
            this.quest = quest;
            this.questGiver = questGiver;
        }

        public void Reset() {
            trackedStats = new Dictionary<TrackedStat, int>();
            trackedItems = new Dictionary<ItemData, int>();
            trackedEntities = new Dictionary<EntityData, EntityTrack>();
            trackedLoot = new Dictionary<string, int>();
            foreach(TrackedStat stat in Enum.GetValues(typeof(TrackedStat))) {
                trackedStats.Add(stat, 0);
            }
        }
        
        public StatKeyPair[] GetStatKeyPairs() {
            var statKeyPairs = new List<StatKeyPair>();
            foreach(KeyValuePair<TrackedStat, int> pair in trackedStats) {
                statKeyPairs.Add(new StatKeyPair {
                    stat = pair.Key, 
                    value = pair.Value
                });
            }

            return statKeyPairs.ToArray();
        }
        
        public ItemKeyPair[] GetItemKeyPairs() {
            var itemKeyPairs = new List<ItemKeyPair>();
            foreach(KeyValuePair<ItemData, int> pair in trackedItems) {
                itemKeyPairs.Add(new ItemKeyPair {
                    stat = pair.Key.nameKey, 
                    value = pair.Value
                });
            }

            return itemKeyPairs.ToArray();
        }
        
        public EntityKeyPair[] GetEntityKeyPairs() {
            var entityKeyPairs = new List<EntityKeyPair>();
            foreach(KeyValuePair<EntityData, EntityTrack> pair in trackedEntities) {
                entityKeyPairs.Add(new EntityKeyPair {
                    stat = pair.Key.nameKey, 
                    value = pair.Value
                });
            }

            return entityKeyPairs.ToArray();
        }

        public LootKeyPair[] GetLootKeyPairs() {
            var lootKeyPairs = new List<LootKeyPair>();
            foreach(KeyValuePair<string, int> pair in trackedLoot) {
                lootKeyPairs.Add(new LootKeyPair {
                    stat = pair.Key, 
                    value = pair.Value
                });
            }

            return lootKeyPairs.ToArray();
        }
    }
    
    [Serializable]
    public struct EntityTrack {
        public int damageTaken;
        public int damageDealt;
        public int timesKilled;

        public static implicit operator int(EntityTrack arg) {
            return arg.damageDealt + arg.damageTaken + arg.timesKilled;
        }

        public static EntityTrack operator +(EntityTrack arg1, EntityTrack arg2) {
            return new EntityTrack() {
                damageDealt = arg1.damageDealt + arg2.damageDealt,
                damageTaken = arg1.damageTaken + arg2.damageTaken,
                timesKilled = arg1.timesKilled + arg2.timesKilled
            };
        }
        
        public static EntityTrack operator -(EntityTrack arg1, EntityTrack arg2) {
           return new EntityTrack() {
               damageDealt = Mathf.Clamp(arg1.damageDealt - arg2.damageDealt, 0, Int32.MaxValue),
               damageTaken = Mathf.Clamp(arg1.damageTaken - arg2.damageTaken, 0, Int32.MaxValue),
               timesKilled = Mathf.Clamp(arg1.timesKilled - arg2.timesKilled, 0, Int32.MaxValue)
            };
        }
    }
}