using System;

namespace UtilsModule {
    [Serializable]
    public class Tag {
        public TagData data;
        public bool inherent;

        public Tag(TagData data, bool inherent = false) {
            this.data = data;
            this.inherent = inherent;
        }

        public bool Matches(Tag tag) {
            if(data != tag.data) return false;

            return inherent == tag.inherent;
        }
    }
}