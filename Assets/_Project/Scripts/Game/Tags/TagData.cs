using Localisation;

namespace Game {
    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag/Tag Data")]
    public class TagData : ScriptableObject {
        public enum TagType {
            None,
            Int,
            Float,
            Equipment
        }

        public bool hidden;
        public TagType type;
        public string valPrefix;
        public string valSuffix;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        
        public Tag GetTag(float value, bool inherent = false)
        {
            return new Tag(this, inherent, value);
        }

        public override string ToString() {
            return name;
        }
    }
}