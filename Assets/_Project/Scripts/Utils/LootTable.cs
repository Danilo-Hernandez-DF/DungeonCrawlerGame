using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public abstract class LootTable<T> : ScriptableObject {
        [SerializeField] protected int weight;
        public virtual T GetWeightedItem() {
            return default;
        }
        
        public virtual List<T> GetAllItems() {
            return new List<T>();
        }
    }
}