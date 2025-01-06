using System.Collections.Generic;

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
            List<WeightedItem> toReturn = new List<WeightedItem> ();

            foreach(WeightedObject<ItemData> item in toConvert) {
                toReturn.Add(GetFromBase(item));
            }

            return toReturn;
        }
    }
}