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

        public override GameObject GetWeightedItem() {
            if(items?.Count == 0 && secondaryTables?.Count == 0) return null;

            int sumWeights = items.Select(item => item.Weight).Sum() + secondaryTables.Select(table => table.weight).Sum();

            int rand = Random.Range(0, sumWeights);
            int added = 0;
            foreach (var wItem in items) {
                added += wItem.Weight;
                if(rand < added) {
                    return wItem.Item;
                }
            }

            foreach (var lootTable in secondaryTables) {
                added += lootTable.weight;
                if(rand < added) {
                    return lootTable.GetWeightedItem();
                }
            }

            return null;
        }
    }
}