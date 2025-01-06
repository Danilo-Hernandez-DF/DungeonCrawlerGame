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

            if(items?.Count == 0 && secondaryTables?.Count == 0) return GameManager.Instance.emptyItem.GetItem();

            foreach(WeightedItem wItem in items) {
                if(wItem.Weight < lowerLimit) continue;
                if(wItem.Weight > upperLimit) continue;

                sumWeights += wItem.Weight;
            }

            foreach(ItemLootTable lootTable in secondaryTables) {
                if(lootTable.weight < lowerLimit) continue;
                if(lootTable.weight > upperLimit) continue;

                sumWeights += lootTable.weight;
            }

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach(WeightedItem wItem in items) {
                if(wItem.Weight < lowerLimit) continue;
                if(wItem.Weight > upperLimit) continue;

                added += wItem.Weight;
                if(rand < added) {
                    int randCount = seeded? SeededRandom.GetRange(wItem.countMin, wItem.countMax+1): Random.Range(wItem.countMin, wItem.countMax+1);
                    var toReturn = wItem.Item.GetItem(randCount);

                    foreach(OverrideTag tag in wItem.additionalTags) {
                        if(tag.tag.data.type == TagData.TagType.Int || tag.tag.data.type == TagData.TagType.Equipment) {
                            var tagInt = new Tag(tag.tag.data, tag.tag.inherent, Mathf.FloorToInt(tag.value));
                            toReturn.AddTag(tagInt);
                            continue;
                        }

                        if(tag.tag.data.type == TagData.TagType.Float) {
                            var tagFloat = new Tag(tag.tag.data, tag.tag.inherent, tag.value);
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

            return GameManager.Instance.emptyItem.GetItem();
        }
    }
}