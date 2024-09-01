using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag Data")]
    public class TagData : ScriptableObject {
        public new string name;

        public Tag GetTag(bool inherent = false) {
            return new Tag(this, inherent);
        }
    }
}