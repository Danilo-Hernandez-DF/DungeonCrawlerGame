using System;
using Game;

namespace UtilsModule
{
    [Serializable]
    public class SavedTag {
        public float value;
        public bool inherent;
        public SerializableGuid data;

        public SavedTag(Tag toCopy)
        {
            value = toCopy.value;
            inherent = toCopy.inherent;
            data = toCopy.data.id;
        }

        public Tag ToTag() {
            return new Tag(GameManager.Instance.TagDatabase.GetData(data), inherent, value);
        }
    }
}