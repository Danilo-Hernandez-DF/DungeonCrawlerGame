using System;

namespace UtilsModule {
    [Serializable]
    public class Tag {
        public TagData.TagType Type => data.type;
        public float value;
        public bool inherent;
        public TagData data;

        public Tag(TagData data, bool inherent = false, float value = 0) { 
            this.inherent = inherent;
            this.data = data;
            this.value = value;
        }

        public virtual bool Matches(Tag tag) {
            if(tag.data != data) return false;
            return inherent == tag.inherent;
        }

        public Tag SetValue(float value) {
            this.value = value;
            return this;
        }

        public virtual float GetValue() {
            return value;
        }

        public override string ToString() {
            return Type switch {
                TagData.TagType.None => data.name,
                TagData.TagType.Equipment => data.name + $": {(Equipment)value}",
                _ => data.name + $": {data.valPrefix}{value}{data.valSuffix}"
            };
        }

        public enum Equipment {
            Weapon, // 0
            Chest, // 1
            Legs, // 2
            Head, // 3
            Charm, // 4
            Pendant // 5
        }
    }
}