using System.Linq;
using Utils;

namespace Game {
    [System.Serializable]
    public class WeightedItem : WeightedObject<ItemData> {
        public int countMin;
        public int countMax;
        public List<OverrideTag> additionalTags;

        public Item GetItem(int count) {
            Item temp = Item.GetItem(count);
            
            foreach (OverrideTag tag in additionalTags)
            {
                switch (tag.tag.data.type)
                {
                    case TagData.TagType.Int:
                    case TagData.TagType.Equipment:
                    {
                        var tagInt = new Tag(tag.tag.data, tag.tag.inherent, Mathf.FloorToInt(tag.value));
                        temp.AddTag(tagInt);
                        continue;
                    }
                    case TagData.TagType.Float:
                    {
                        var tagFloat = new Tag(tag.tag.data, tag.tag.inherent, tag.value);
                        temp.AddTag(tagFloat);
                        continue;
                    }
                    case TagData.TagType.None:
                    default:
                        temp.AddTag(tag.tag);
                        break;
                }
            }

            return temp;
        }
        
        public static WeightedItem GetFromBase(WeightedObject<ItemData> toConvert) {
            return new WeightedItem {Item = toConvert.Item, Weight = toConvert.Weight};
        }

        public static WeightedObject<ItemData> GetBase(WeightedItem toConvert) {
            return new WeightedObject<ItemData> {Item = toConvert.Item, Weight = toConvert.Weight};
        }

        public static List<WeightedItem> GetListFromBase(List<WeightedObject<ItemData>> toConvert) {
            return toConvert.Select(GetFromBase).ToList();
        }
    }
}