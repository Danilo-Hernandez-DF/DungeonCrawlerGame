namespace UtilsModule {
    public class ValueTag<T> : Tag {
        private T value;
        public ValueTag(TagData data, T value, bool inherent = false) : base(data, inherent) {
            this.value = value;
        }

        public T GetValue() {
            return value;
        }
    }
}