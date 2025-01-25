using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/TileBase")]
    public class TileBaseLootTable : LootTable<TileBase> {
        [SerializeField] private List<WeightedTile> items;
        [SerializeField] private List<TileBaseLootTable> secondaryTables;

        public override TileBase GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            if(items?.Count == 0 && secondaryTables?.Count == 0) return null;

            int sumWeights = (from wItem in items where wItem.Weight >= lowerLimit where wItem.Weight <= upperLimit select wItem.Weight).Sum() + (from lootTable in secondaryTables where lootTable.weight >= lowerLimit where lootTable.weight <= upperLimit select lootTable.weight).Sum();

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach (var wItem in items.Where(wItem => wItem.Weight >= lowerLimit).Where(wItem => wItem.Weight <= upperLimit)) {
                added += wItem.Weight;
                if(rand < added) {
                    return wItem.Item;
                }
            }

            foreach (var lootTable in secondaryTables.Where(lootTable => lootTable.weight >= lowerLimit).Where(lootTable => lootTable.weight <= upperLimit)) {
                added += lootTable.weight;
                if(rand < added) {
                    return lootTable.GetWeightedItem(lowerLimit, upperLimit, seeded);
                }
            }

            return null;
        }
    }
}