using System;
using Game;

namespace UtilsModule
{
    [Serializable]
    public class SavedTag {
        public float value;
        public bool inherent;
        public string data;

        public SavedTag(Tag toCopy)
        {
            value = toCopy.value;
            inherent = toCopy.inherent;
            data = toCopy.data.nameKey;
        }

        public Tag ToTag() {
            return new Tag(GameManager.GetTag(data), inherent, value);
        }
    }
}