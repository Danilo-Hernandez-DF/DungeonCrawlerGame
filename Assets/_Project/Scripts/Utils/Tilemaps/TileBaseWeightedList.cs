using System.Linq;
using UnityEngine.Tilemaps;

namespace Utils {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/TileBase")]
    public class TileBaseWeightedList : WeightedList<TileBase> {
        [SerializeField] private List<WeightedTile> items;
        [SerializeField] private List<TileBaseWeightedList> secondaryTables;

        public override TileBase GetWeightedItem() {
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

            return items[0].Item;
        }
    }
}