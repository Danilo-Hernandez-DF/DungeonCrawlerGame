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
        
        public SerializableGuid id = SerializableGuid.Empty;
        private void OnValidate() {
            if(id == SerializableGuid.Empty) id = SerializableGuid.NewGuid();
            Debug.Log($"Assigned {name} with id: {id.ToString()}");
        }

        public bool hidden;
        public TagType type;
        public string valPrefix;
        public string valSuffix;
        public new string name;
        
        public Tag GetTag(float value, bool inherent = false) {
            return new Tag(this, inherent, value);
        }

        public override string ToString() {
            return name;
        }
    }
}