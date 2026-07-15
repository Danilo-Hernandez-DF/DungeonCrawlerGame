using UnityEngine.Serialization;

namespace Game {
    [System.Serializable]
    public class LootSetting {
        public LootType lootType;
        [FormerlySerializedAs("lootTable")] public ItemWeightedList weightedList;
        public int rolls; 
    }
}