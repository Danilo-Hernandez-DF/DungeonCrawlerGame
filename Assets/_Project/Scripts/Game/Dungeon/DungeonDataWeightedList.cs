using System.Linq;
using Utils;

namespace Game {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/DungeonData")]
    public class DungeonDataWeightedList : WeightedList<DungeonData> {
        [SerializeField] private List<WeightedDungeonData> items;
        [SerializeField] private List<DungeonDataWeightedList> secondaryTables;

        public List<DungeonData> GetList() {
            var toReturn = items.Select(item => item.Item).ToList();

            foreach(var table in secondaryTables) {
                toReturn.AddRange(table.GetList());
            }

            return toReturn;
        }
        
        public List<ItemData> GetItems() {
            var allItems = new List<ItemData>();
            
            foreach (var data in items) {
                foreach (var item in data.Item.AvailableItems()) {
                    if (allItems.Contains(item)) continue;
                    allItems.Add(item);
                }
            }

            return allItems;
        }

        public override DungeonData GetWeightedItem() {
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