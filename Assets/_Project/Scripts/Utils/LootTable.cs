using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public abstract class LootTable<T> : ScriptableObject {
        [SerializeField] protected int weight;
        public virtual T GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            return default;
        }
    }
}