using System;
using Systems.Persistence;
using UnityEngine;
using UtilsModule;

namespace Game {
    [Serializable]
    public class GameSettingsData : ISaveable {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public StatisticsTracker statisticsTracker;
        public StatKeyPair[] statKeyPairs = Array.Empty<StatKeyPair>();
        public ItemKeyPair[] itemKeyPairs = Array.Empty<ItemKeyPair>();
        public EntityKeyPair[] entityKeyPairs = Array.Empty<EntityKeyPair>();
    }

    [Serializable]
    public struct ItemKeyPair {
        public SerializableGuid stat;
        public int value;
    }
    
    [Serializable]
    public struct StatKeyPair {
        public StatisticsTracker.TrackedStat stat;
        public int value;
    }
    
    [Serializable]
    public struct EntityKeyPair {
        public SerializableGuid stat;
        public EntityTrack value;
    }
}