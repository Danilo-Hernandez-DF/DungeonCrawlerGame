using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEditor.Search;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "LootTable/ItemLootTable")]
    public class ItemLootTable : LootTable<Item> {
        [SerializeField] private List<WeightedItem> items;
        [SerializeField] private List<ItemLootTable> secondaryTables;

        public override Item GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            if(items?.Count == 0 && secondaryTables?.Count == 0) return GameManager.Instance.emptyItem.GetItem();

            int sumWeights = (from wItem in items where wItem.Weight >= lowerLimit where wItem.Weight <= upperLimit select wItem.Weight).Sum() + 
                             (from lootTable in secondaryTables where lootTable.weight >= lowerLimit where lootTable.weight <= upperLimit select lootTable.weight).Sum();

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            if(items != null)
                foreach (var wItem in items.Where(wItem => wItem.Weight >= lowerLimit)
                             .Where(wItem => wItem.Weight <= upperLimit)) {
                    added += wItem.Weight;
                    if (rand >= added) continue;
                    int randCount = seeded
                        ? SeededRandom.GetRange(wItem.countMin, wItem.countMax + 1)
                        : Random.Range(wItem.countMin, wItem.countMax + 1);
                    var toReturn = wItem.Item.GetItem(randCount);

                    foreach (OverrideTag tag in wItem.additionalTags)
                    {
                        switch (tag.tag.data.type)
                        {
                            case TagData.TagType.Int:
                            case TagData.TagType.Equipment:
                            {
                                var tagInt = new Tag(tag.tag.data, tag.tag.inherent, Mathf.FloorToInt(tag.value));
                                toReturn.AddTag(tagInt);
                                continue;
                            }
                            case TagData.TagType.Float:
                            {
                                var tagFloat = new Tag(tag.tag.data, tag.tag.inherent, tag.value);
                                toReturn.AddTag(tagFloat);
                                continue;
                            }
                            case TagData.TagType.None:
                            default:
                                toReturn.AddTag(tag.tag);
                                break;
                        }
                    }

                    return toReturn;
                }

            if(secondaryTables == null) return GameManager.Instance.emptyItem.GetItem();
            foreach (var lootTable in secondaryTables.Where(lootTable => lootTable.weight >= lowerLimit)
                         .Where(lootTable => lootTable.weight <= upperLimit)) {
                added += lootTable.weight;
                if (rand < added)
                {
                    return lootTable.GetWeightedItem(lowerLimit, upperLimit, seeded);
                }
            }

            return GameManager.Instance.emptyItem.GetItem();
        }
    }
}