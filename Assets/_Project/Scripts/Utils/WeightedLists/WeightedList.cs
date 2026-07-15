namespace Utils {
    public abstract class WeightedList<T> : ScriptableObject {
        [SerializeField] protected int weight;
        public virtual T GetWeightedItem() {
            return default;
        }
        
        public virtual List<T> GetAllItems() {
            return new List<T>();
        }
    }
}