using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/GameObject")]
    public class GOLootTable : LootTable<GameObject> {
        [SerializeField] private List<WeightedGameObject> items;
        [SerializeField] private List<GOLootTable> secondaryTables;

        public List<GameObject> GetList() {
            var toReturn = items.Select(item => item.Item).ToList();

            foreach(var table in secondaryTables) {
                toReturn.AddRange(table.GetList());
            }

            return toReturn;
        }

        public override GameObject GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue) {
            if(items?.Count == 0 && secondaryTables?.Count == 0) return null;

            int sumWeights = (from wItem in items where wItem.Weight >= lowerLimit where wItem.Weight <= 
                upperLimit select wItem.Weight).Sum() + (from lootTable in secondaryTables where lootTable.weight >= lowerLimit where lootTable.weight 
                <= upperLimit select lootTable.weight).Sum();

            int rand = Random.Range(0, sumWeights);
            int added = 0;
            foreach (var wItem in items.Where(wItem => wItem.Weight >= lowerLimit).Where(wItem => 
                         wItem.Weight <= upperLimit)) {
                added += wItem.Weight;
                if(rand < added) {
                    return wItem.Item;
                }
            }

            foreach (var lootTable in secondaryTables.Where(lootTable => lootTable.weight >= lowerLimit).Where(lootTable => lootTable.weight <= upperLimit)) {
                added += lootTable.weight;
                if(rand < added) {
                    return lootTable.GetWeightedItem(lowerLimit, upperLimit);
                }
            }

            return null;
        }
    }
}