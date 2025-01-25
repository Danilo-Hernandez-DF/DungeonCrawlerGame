using System;
using System.Collections.Generic;
using Unity.Cinemachine;
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
            DamageDealt
        }

        public Dictionary<TrackedStat, int> trackedStats;
        public Dictionary<ItemData, int> trackedItems;
        public string name;
        public Quest quest;
        
        public StatisticsTracker Copy() {
            var tracker = new StatisticsTracker(name) {
                trackedStats = trackedStats,
                trackedItems = trackedItems
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
        }

        public int GetTracked(ItemData stat) {
            return trackedItems.GetValueOrDefault(stat, 0);
        }
        
        public int GetTracked(TrackedStat stat) {
            return trackedStats.GetValueOrDefault(stat, 0);
        }
        
        public StatisticsTracker(string name, Quest quest = null) {
            this.name = name;
            Reset();
            
            this.quest = quest;
        }

        public void Reset() {
            trackedStats = new Dictionary<TrackedStat, int>();
            trackedItems = new Dictionary<ItemData, int>();
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
                    stat = pair.Key, 
                    value = pair.Value
                });
            }

            return itemKeyPairs.ToArray();
        }
    }
}