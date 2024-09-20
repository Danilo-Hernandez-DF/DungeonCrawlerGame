using System;
using UnityEngine;

namespace UtilsModule {
    public class Tag<T> : Tag {
        [SerializeField] private T value;

        public Tag(TagData<T> data, bool inherent = false, T value = default) : base(data, inherent) {
            this.value = value;
        }

        public T GetValue() {
            return value;
        }

        public Tag SetValue(T value) {
            this.value = value;
            return this;
        }

        public override bool Matches(Tag tag) {
            if(tag.data != data) return false;
            if(inherent != tag.inherent) return false;
            if(!GetValue().Equals(((Tag<T>)tag).GetValue())) return false;
            
            return true;
        }

        TagData<T> TData() {
            return (TagData<T>)data;
        }

        public override string ToString() {
            return data.name + $": {TData().valPrefix}{value}{TData().valSuffix}";
        }
    }

    [Serializable]
    public class Tag {
        public bool inherent;
        public TagData data;

        public Tag(TagData data, bool inherent = false) { 
            this.inherent = inherent;
            this.data = data;
        }

        public virtual bool Matches(Tag tag) {
            if(tag.data != data) return false;
            if(inherent != tag.inherent) return false;
            
            return true;
        }

        public override string ToString() {
            return data.name;
        }
    }
}