using System.Collections.Generic;
using UtilsModule;

namespace Game {
    public class SODatabase<T> {
        protected readonly List<T> Items = new();

        public virtual T GetData(string name) {
            return default;
        }
        
        public virtual T GetData(SerializableGuid id) {
            return default;
        }
        
        public T[] GetAll() {
            return Items.ToArray();
        }

        public void AddItem(T item) {
            Items.Add(item);
        }
    }
}