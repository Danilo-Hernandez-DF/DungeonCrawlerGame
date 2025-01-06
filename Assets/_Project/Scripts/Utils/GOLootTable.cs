using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/GameObject")]
    public class GOLootTable : LootTable<GameObject> {
        [SerializeField] private List<WeightedGameObject> items;
        [SerializeField] private List<GOLootTable> secondaryTables;

        public List<GameObject> GetList() {
            var toReturn = new List<GameObject>();

            foreach(var item in items) {
                toReturn.Add(item.Item);
            }

            foreach(var table in secondaryTables) {
                toReturn.AddRange(table.GetList());
            }

            return toReturn;
        }

        public override GameObject GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            int sumWeights = 0;

            if(items?.Count == 0 && secondaryTables?.Count == 0) return default;

            foreach(WeightedGameObject wItem in items) {
                if(wItem.Weight < lowerLimit) continue;
                if(wItem.Weight > upperLimit) continue;

                sumWeights += wItem.Weight;
            }

            foreach(GOLootTable lootTable in secondaryTables) {
                if(lootTable.weight < lowerLimit) continue;
                if(lootTable.weight > upperLimit) continue;

                sumWeights += lootTable.weight;
            }

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach(WeightedGameObject wItem in items) {
                if(wItem.Weight < lowerLimit) continue;
                if(wItem.Weight > upperLimit) continue;

                added += wItem.Weight;
                if(rand < added) {
                    return wItem.Item;
                }
            }

            foreach(GOLootTable lootTable in secondaryTables) {
                if(lootTable.weight < lowerLimit) continue;
                if(lootTable.weight > upperLimit) continue;

                added += lootTable.weight;
                if(rand < added) {
                    return lootTable.GetWeightedItem(lowerLimit, upperLimit, seeded);
                }
            }

            return default;
        }
    }
}