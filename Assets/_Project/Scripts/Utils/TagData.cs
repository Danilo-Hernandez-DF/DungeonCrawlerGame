using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag/Tag Data")]
    public class TagData : ScriptableObject {
        public enum TagType {
            None,
            Int,
            Float,
            Equipment
        }
        public TagType type;
        public string valPrefix;
        public string valSuffix;
        public new string name;
        public virtual Tag GetTag(float value, bool inherent = false) {
            return new Tag(this, inherent, value);
        }

        public override string ToString() {
            return name;
        }
    }
}