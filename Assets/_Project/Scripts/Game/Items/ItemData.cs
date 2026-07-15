using Localisation;

namespace Game {
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item/Item Data")]
    public class ItemData : ScriptableObject {
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        [TextArea] public string description;
        public int maxCount;
        public int rarity;
        [SerializeField] Sprite sprite;
        public List<TagDefault> inherentTags;
        public Sprite DisplaySprite {get => sprite;}

        public Item GetItem(int count = 1, List<Tag> tags = null) {
            return new Item(this, count, tags);
        }

        public bool HasTag(TagData tag, float value = 0, bool accept0 = false) {
            if(!accept0 && value == 0) return inherentTags.Exists(x => x.data == tag);
            return inherentTags.Exists(x => x.data == tag && x.value == value);
        }

        public bool HasTag(string tag, float value = 0, bool accept0 = false) {
            if(!accept0 && value == 0) return inherentTags.Exists(x => x.data.nameKey == tag);
            return inherentTags.Exists(x => x.data.nameKey == tag && x.value == value);
        }
    }
    
    [Serializable]
    public struct TagDefault {
        public TagData data;
        public float value;
    }
}