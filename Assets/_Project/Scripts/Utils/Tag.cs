using System;

namespace UtilsModule {
    [Serializable]
    public class Tag {
        public TagData.TagType type => data.type;
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
            if(inherent != tag.inherent) return false;
            
            return true;
        }

        public Tag SetValue(float value) {
            this.value = value;
            return this;
        }

        public virtual float GetValue() {
            return value;
        }

        public override string ToString() {
            if(type == TagData.TagType.None) return data.name;
            if(type == TagData.TagType.Equipment) return data.name + $": {(Equipment)value}";
            return data.name + $": {data.valPrefix}{value}{data.valSuffix}";
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