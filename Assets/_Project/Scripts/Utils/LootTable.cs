using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot Table")]
    public class LootTable : ScriptableObject {
        [SerializeField] List<WeightedItem> items;
        [SerializeField] List<WeightedItemData> itemDatas;

        public Item GetWeightedItem(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            int sumWeights = 0;

            if(items.Count == 0 && itemDatas.Count == 0) return GameManager.Instance.EmptyItem.GetItem();
            if(items.Count == 0 && itemDatas.Count == 1) return itemDatas[0].data.GetItem();
            if(items.Count == 1 && itemDatas.Count == 0) return items[0].item.GetItem();

            foreach(WeightedItem item in items) {
                if(item.weight < lowerLimit) continue;
                if(item.weight > upperLimit) continue;

                sumWeights += item.weight;
            }

            foreach(WeightedItemData item in itemDatas) {
                if(item.weight < lowerLimit) continue;
                if(item.weight > upperLimit) continue;

                sumWeights += item.weight;
            }

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach(WeightedItem item in items) {
                if(item.weight < lowerLimit) continue;
                if(item.weight > upperLimit) continue;

                added += item.weight;
                if(rand < added) {
                    int randCount = seeded? SeededRandom.GetRange(item.countMin, item.countMax+1): Random.Range(item.countMin, item.countMax+1);
                    var toReturn = item.item.GetItem(randCount);

                    foreach(OverrideTag tag in item.additionalTags) {
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

            foreach(WeightedItemData item in itemDatas) {
                if(item.weight < lowerLimit) continue;
                if(item.weight > upperLimit) continue;

                added += item.weight;
                if(rand < added) {
                    int randCount = seeded? SeededRandom.GetRange(item.countMin, item.countMax+1): Random.Range(item.countMin, item.countMax+1);
                    return item.data.GetItem(randCount);
                }
            }

            return GameManager.Instance.EmptyItem.GetItem();
        }
    }

    [System.Serializable]
    public struct WeightedItem {
        public ItemData item;
        public int countMin;
        public int countMax;
        public int weight;
        public List<OverrideTag> additionalTags;
    }

    [System.Serializable]
    public struct WeightedItemData {
        public ItemData data;
        public int countMin;
        public int countMax;
        public int weight;
    }

    [System.Serializable]
    public struct OverrideTag {
        public Tag tag;
        public float value;
    }
}