using System.Collections.Generic;
using Game;
using UnityEditor.Search;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/ItemLootTable")]
    public class ItemLootTable : LootTable<Item> {
        [SerializeField] private List<WeightedItem> items;
        [SerializeField] private List<ItemLootTable> secondaryTables;

        public override Item GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            int sumWeights = 0;

            if(items?.Count == 0 && secondaryTables?.Count == 0) return GameManager.Instance.EmptyItem.GetItem();

            foreach(WeightedItem wItem in items) {
                if(wItem.weight < lowerLimit) continue;
                if(wItem.weight > upperLimit) continue;

                sumWeights += wItem.weight;
            }

            foreach(ItemLootTable lootTable in secondaryTables) {
                if(lootTable.weight < lowerLimit) continue;
                if(lootTable.weight > upperLimit) continue;

                sumWeights += lootTable.weight;
            }

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach(WeightedItem wItem in items) {
                if(wItem.weight < lowerLimit) continue;
                if(wItem.weight > upperLimit) continue;

                added += wItem.weight;
                if(rand < added) {
                    int randCount = seeded? SeededRandom.GetRange(wItem.countMin, wItem.countMax+1): Random.Range(wItem.countMin, wItem.countMax+1);
                    var toReturn = wItem.item.GetItem(randCount);

                    foreach(OverrideTag tag in wItem.additionalTags) {
                        if(tag.tag.data is TagData<int>) {
                            var tagInt = new Tag<int>((TagData<int>)tag.tag.data, tag.tag.inherent, Mathf.FloorToInt(tag.value));
                            toReturn.AddTag(tagInt);
                            continue;
                        }

                        if(tag.tag.data is TagData<float>) {
                            var tagFloat = new Tag<float>((TagData<float>)tag.tag.data, tag.tag.inherent, tag.value);
                            toReturn.AddTag(tagFloat);
                            continue;
                        }

                        toReturn.AddTag(tag.tag);
                    }


                    return toReturn;
                }
            }

            foreach(ItemLootTable lootTable in secondaryTables) {
                if(lootTable.weight < lowerLimit) continue;
                if(lootTable.weight > upperLimit) continue;

                added += lootTable.weight;
                if(rand < added) {
                    return lootTable.GetWeightedItem(lowerLimit, upperLimit, seeded);
                }
            }

            return GameManager.Instance.EmptyItem.GetItem();
        }
    }
}