using System.Linq;

namespace Utils {
    [Serializable]
    public class WeightedGameObject : WeightedObject<GameObject> { 
        public static WeightedGameObject GetFromBase(WeightedObject<GameObject> toConvert) {
            return new WeightedGameObject {Item = toConvert.Item, Weight = toConvert.Weight};
        }

        public static WeightedObject<GameObject> GetBase(WeightedGameObject toConvert) {
            return new WeightedObject<GameObject> {Item = toConvert.Item, Weight = toConvert.Weight};
        }

        public static List<WeightedGameObject> GetListFromBase(List<WeightedObject<GameObject>> toConvert) {
            return toConvert.Select(GetFromBase).ToList();
        }

        public static List<WeightedObject<GameObject>> GetListOfBase(List<WeightedGameObject> toConvert) {
            return toConvert.Select(GetBase).ToList();
        }
    }
}