using System.Collections.Generic;
using System.Linq;

namespace UtilsModule {
    [System.Serializable]
    public class WeightedItem : WeightedObject<ItemData> {
        public int countMin;
        public int countMax;
        public List<OverrideTag> additionalTags;

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