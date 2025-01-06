using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/TileBase")]
    public class TileBaseLootTable : LootTable<TileBase> {
        [SerializeField] private List<WeightedTile> items;
        [SerializeField] private List<TileBaseLootTable> secondaryTables;

        public override TileBase GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            int sumWeights = 0;

            if(items?.Count == 0 && secondaryTables?.Count == 0) return default;

            foreach(WeightedTile wItem in items) {
                if(wItem.Weight < lowerLimit) continue;
                if(wItem.Weight > upperLimit) continue;

                sumWeights += wItem.Weight;
            }

            foreach(TileBaseLootTable lootTable in secondaryTables) {
                if(lootTable.weight < lowerLimit) continue;
                if(lootTable.weight > upperLimit) continue;

                sumWeights += lootTable.weight;
            }

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach(WeightedTile wItem in items) {
                if(wItem.Weight < lowerLimit) continue;
                if(wItem.Weight > upperLimit) continue;

                added += wItem.Weight;
                if(rand < added) {
                    return wItem.Item;
                }
            }

            foreach(TileBaseLootTable lootTable in secondaryTables) {
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