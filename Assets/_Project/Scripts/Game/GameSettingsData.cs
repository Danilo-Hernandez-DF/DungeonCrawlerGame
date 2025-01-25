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
    }

    [Serializable]
    public struct ItemKeyPair {
        public ItemData stat;
        public int value;
    }
    
    [Serializable]
    public struct StatKeyPair {
        public StatisticsTracker.TrackedStat stat;
        public int value;
    }
}