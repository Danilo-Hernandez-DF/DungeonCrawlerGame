using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class TagData<T> : TagData {
        public string valPrefix;
        public string valSuffix;

        public override Tag GetTag(bool inherent = false) {
            return new Tag<T>(this, inherent);
        }

        public Tag GetTag(T value, bool inherent = false) {
            return ((Tag<T>)GetTag(inherent)).SetValue(value);
        }
    }

    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag/Tag Data")]
    public class TagData : ScriptableObject {
        public new string name;
        public virtual Tag GetTag(bool inherent = false) {
            return new Tag(this, inherent);
        }

        public override string ToString() {
            return name;
        }
    }
}