using System;
using Systems.Persistence;
using UnityEngine;
using UtilsModule;

namespace Game {
    [Serializable]
    public class GameSettingsData : ISaveable {
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public TrackerData mainTracker = new TrackerData() {
            statKeyPairs = Array.Empty<StatKeyPair>(),
            itemKeyPairs = Array.Empty<ItemKeyPair>(),
            entityKeyPairs = Array.Empty<EntityKeyPair>(),
            lootKeyPairs = Array.Empty<LootKeyPair>()
        };

        public TrackerData[] trackers = Array.Empty<TrackerData>();
    }

    [Serializable]
    public struct ItemKeyPair {
        public string stat;
        public int value;
    }
    
    [Serializable]
    public struct StatKeyPair {
        public StatisticsTracker.TrackedStat stat;
        public int value;
    }
    
    [Serializable]
    public struct EntityKeyPair {
        public string stat;
        public EntityTrack value;
    }
    
    [Serializable]
    public struct LootKeyPair {
        public string stat;
        public int value;
    }

    [Serializable]
    public struct TrackerData {
        public string trackerName;
        public string questName;
        public StatKeyPair[] statKeyPairs;
        public ItemKeyPair[] itemKeyPairs;
        public EntityKeyPair[] entityKeyPairs;
        public LootKeyPair[] lootKeyPairs;
    }
}