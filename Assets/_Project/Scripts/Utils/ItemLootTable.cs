using System.Collections.Generic;
using System.Linq;
using Game;
using Localisation;
using UnityEditor.Search;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/ItemLootTable")]
    public class ItemLootTable : LootTable<Item> {
        public string nameKey;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);
        [SerializeField] private List<WeightedItem> items;
        [SerializeField] private List<ItemLootTable> secondaryTables;

        public override Item GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue) {
            if(items?.Count == 0 && secondaryTables?.Count == 0) return GameManager.emptyItem.GetItem();

            int sumWeights = (from wItem in items where wItem.Weight >= lowerLimit where wItem.Weight <= upperLimit select wItem.Weight).Sum() + 
                             (from lootTable in secondaryTables where lootTable.weight >= lowerLimit where lootTable.weight <= upperLimit select lootTable.weight).Sum();

            int rand = Random.Range(0, sumWeights);
            int added = 0;
            if(items != null)
                foreach (var wItem in items.Where(wItem => wItem.Weight >= lowerLimit)
                             .Where(wItem => wItem.Weight <= upperLimit)) {
                    added += wItem.Weight;
                    if (rand >= added) continue;
                    int randCount = Random.Range(wItem.countMin, wItem.countMax + 1);
                    
                    return wItem.GetItem(randCount);
                }

            if(secondaryTables == null) return GameManager.emptyItem.GetItem();
            foreach (var lootTable in secondaryTables.Where(lootTable => lootTable.weight >= lowerLimit)
                         .Where(lootTable => lootTable.weight <= upperLimit)) {
                added += lootTable.weight;
                if (rand < added)
                {
                    return lootTable.GetWeightedItem(lowerLimit, upperLimit);
                }
            }

            return GameManager.emptyItem.GetItem();
        }
    }
}