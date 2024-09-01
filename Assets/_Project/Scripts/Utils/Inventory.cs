using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class Inventory {
        public List<Item> items;
        public int size;
        public int Size {get => size;}

        public int Add(Item item, int count = 1) {
            if(items == null) items = new List<Item>();

            if(items.Exists(x => x.data == item.data && x.count < item.data.maxCount)) {
                var foundMatch = items.Find(x => x.data == item.data && x.count < item.data.maxCount);
                int remainder = foundMatch.count + count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder; 
                foundMatch.count += count;

                if(remainder > 0) return Add(item, remainder);
                return 0;
            } else if(items.Count < size) {
                int remainder = count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder;
                items.Add(new Item(item.data, count));

                if(remainder > 0) return Add(item, remainder);
                return 0;
            }

            Debug.Log("Inventory is full");
            return count;
        }

        public bool TryAdd(Item item, int count = 1) {
            if(AvailableCount(item) < count) return false;

            Add(item, count);
            return true;
        }

        public bool TryRemove(Item item, int count = 1) {
            if(items == null) return false;
            if(!items.Exists(x => x.data == item.data)) return false;
            if(GetCount(item) <= count) return false;

            List<Item> matchingItems = items.FindAll(x => x.data == item.data);
            matchingItems.Reverse();
            foreach(var i in matchingItems) {
                if(i.count <= count) {
                    items.Remove(i);
                    count -= i.count;
                } else {
                    i.count -= count;
                    count = 0;
                }

                if(count == 0) return true;
            }

            return false;
        }

        public int GetCount(Item item) {
            if(items == null) return 0;
            if(!items.Exists(x => x.data == item.data)) return 0;
            
            int count = 0;
            foreach(var i in items.FindAll(x => x.data == item.data)) count += i.count;
            return count;
        }

        public int AvailableCount(Item item) {
            if(items == null) return item.data.maxCount * size;
            if(!items.Exists(x => x.data == item.data)) return EmptySlots() * item.data.maxCount;
            
            int count = 0;
            foreach(var i in items.FindAll(x => x.data == item.data)) count += item.data.maxCount - i.count;

            count += EmptySlots() * item.data.maxCount;
            return count;
        }

        public int EmptySlots() {
            if(items == null) return size;
            return size - items.Count;
        }
    }
}