using Localisation;

namespace Game {
    [CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entity Data")]
    public class EntityData : ScriptableObject {
        public string entityName => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        public EntityStats stats;
        public Sprite sprite;
        public GameObject prefab;
        public List<Tag> tags;

        public bool HasTag(string tag) {
            foreach(Tag t in tags) {
                if(t.data.nameKey == tag) return true;
            }

            return false;
        }
    }
}