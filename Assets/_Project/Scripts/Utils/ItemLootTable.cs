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
        
        public List<ItemLootTable> GetTables() {
            return secondaryTables;
        }

        public Item GetBiasedItem(ItemData item, out int remainingRolls, int rolls = 10) {
            int attempts = 0;
            while (attempts < rolls) {
                var result = GetWeightedItem();
                if (result.data == item) {
                    //Debug.Log("found biased item");
                    remainingRolls = rolls - attempts;
                    return result;
                }
                attempts++;
            }
            
            //Debug.Log("did not find biased item");
            remainingRolls = 0;
            return GetWeightedItem();
        }
        
        public Item GetBiasedItem(TagData tag, out int remainingRolls, int rolls = 10) {
            int attempts = 0;
            while (attempts < rolls) {
                var result = GetWeightedItem();
                if (result.HasTag(tag)) {
                    //Debug.Log("found biased item");
                    remainingRolls = rolls - attempts;
                    return result;
                }
                attempts++;
            }
            
            //Debug.Log("did not find biased item");
            remainingRolls = 0;
            return GetWeightedItem();
        }
        
        public Item GetBiasedItem(int rarity, out int remainingRolls, int rolls = 10) {
            int attempts = 0;
            while (attempts < rolls) {
                var result = GetWeightedItem();
                if (result.data.rarity >= rarity) {
                    //Debug.Log("found biased item");
                    remainingRolls = rolls - attempts;
                    return result;
                }
                attempts++;
            }
            
            //Debug.Log("did not find biased item");
            remainingRolls = 0;
            return GetWeightedItem();
        }

        public override Item GetWeightedItem() {
            if(items?.Count == 0 && secondaryTables?.Count == 0) return GameManager.emptyItem.GetItem();

            int sumWeights = items.Select(x => x.Weight).Sum() + secondaryTables.Select(x => x.weight).Sum();

            int rand = Random.Range(0, sumWeights);
            int added = 0;
            if(items != null)
                foreach (var wItem in items) {
                    added += wItem.Weight;
                    if (rand >= added) continue;
                    int randCount = Random.Range(wItem.countMin, wItem.countMax + 1);
                    
                    return wItem.GetItem(randCount);
                }

            if(secondaryTables == null) return GameManager.emptyItem.GetItem();
            foreach (var lootTable in secondaryTables) {
                added += lootTable.weight;
                if (rand < added)
                {
                    return lootTable.GetWeightedItem();
                }
            }

            return GameManager.emptyItem.GetItem();
        }
        
        public override List<Item> GetAllItems() {
            List<Item> allItems = new List<Item>();
            foreach(var item in items) {
                allItems.Add(item.GetItem(1));
            }

            foreach (var table in secondaryTables) {
                allItems.AddRange(table.GetAllItems());
            }
            
            return allItems;
        }
    }
}