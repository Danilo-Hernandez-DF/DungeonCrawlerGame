using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class Inventory {
        public List<Item> items;
        InventoryEventChannel inventoryChannel;
        int size = 10;
        public int Size {get => size;}

        public Inventory(int size = 10, InventoryEventChannel inventoryChannel = null) {
            this.inventoryChannel = inventoryChannel;
            this.size = size;
            PopulateInventory();
        }

        public int Add(Item item, int count = 1) {
            if(items == null) PopulateInventory();

            var foundMatch = items.Find(x => x.Matches(item) && x.count < item.data.maxCount);
            if(foundMatch != null) {
                int remainder = foundMatch.count + count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder; 
                foundMatch.count += count;

                if(remainder > 0) return Add(item, remainder);
                inventoryChannel?.Invoke(this);
                return 0;
            } else if(EmptySlots() > 0) {
                int remainder = count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder;
                SetSlot(NextEmpty(), item, count);

                if(remainder > 0) return Add(item, remainder);
                inventoryChannel?.Invoke(this);
                return 0;
            }

            Debug.Log("Inventory is full");
            inventoryChannel?.Invoke(this);
            return count;
        }

        public int AddAt(int slot, Item item, int count = 1) {
            if(items == null) PopulateInventory();
            if(slot < 0 || slot >= items.Count) return count;
            if(count > item.data.maxCount) count = item.data.maxCount;

            if(items[slot].IsEmpty) {
                SetSlot(slot, item, count);
                return 0;
            }

            if(items[slot].Matches(item)) {
                int remainder = items[slot].count + count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder;
                items[slot].count += count;
                inventoryChannel?.Invoke(this);
                return remainder;
            }

            Debug.Log("Slot is occupied");
            return count;
        }

        public void SetSlot(int slot, Item item, int count = 1) {
            items[slot] = item.Copy();
            items[slot].count = count;

            inventoryChannel?.Invoke(this);
        }

        int NextEmpty() {
            for(int i = 0; i < items.Count; i++) {
                if(items[i].data == GameManager.Instance.EmptyItem) return i;
            }
            return items.Count;
        }

        void PopulateInventory() {
            items = new List<Item>(size);

            for(int i = 0; i < size; i++) {
                items.Add(new Item(GameManager.Instance.EmptyItem));
            }
        }

        public bool TryAdd(Item item, int count = 1) {
            if(AvailableCount(item) < count) return false;

            Add(item, count);
            return true;
        }

        public void SetAtRandom(Item item, int count = 1, bool isSeeded = false) {
            if(items == null) PopulateInventory();
            if(AvailableCount(item) == 0) return;

            int rand;

            bool found = false;
            do {
                rand = isSeeded? SeededRandom.GetRange(0, Size): Random.Range(0, Size);

                if(items[rand].IsEmpty || items[rand].Matches(item)) found = true;
            } while(!found);

            AddAt(rand, item, count);
        }

        public bool TryRemove(Item item, int count = 1) {
            if(items == null) return false;
            if(!items.Exists(x => x.Matches(item))) return false;
            if(GetCount(item) <= count) return false;

            List<Item> matchingItems = items.FindAll(x => x.Matches(item));
            matchingItems.Reverse();
            for(int i = 0; i < matchingItems.Count; i++) {
                if(matchingItems[i].count <= count) {
                    ResetSlot(i);
                    count -= matchingItems[i].count;
                } else {
                    matchingItems[i].count -= count;
                    count = 0;
                }

                if(count == 0) {
                    inventoryChannel?.Invoke(this);
                    return true;
                }
            }

            return false;
        }

        void ResetSlot(int index) {
            items[index] = new Item(GameManager.Instance.EmptyItem, 0);
            inventoryChannel?.Invoke(this);
        }

        public int GetCount(Item item) {
            if(items == null) return 0;
            if(!items.Exists(x => x.Matches(item))) return 0;
            
            int count = 0;
            foreach(var i in items.FindAll(x => x.Matches(item))) count += i.count;
            return count;
        }

        public int AvailableCount(Item item) {
            if(items == null) return item.data.maxCount * size;
            if(!items.Exists(x => x.Matches(item))) return EmptySlots() * item.data.maxCount;
            
            int count = 0;
            foreach(var i in items.FindAll(x => x.Matches(item))) count += item.data.maxCount - i.count;

            count += EmptySlots() * item.data.maxCount;
            return count;
        }

        public int EmptySlots() {
            int emptySlots = 0;

            foreach(var i in items) {
                if(i.data == GameManager.Instance.EmptyItem) emptySlots++;
            }

            return emptySlots;
        }
    }
}