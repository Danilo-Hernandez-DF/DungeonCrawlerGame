using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UtilsModule {
    [Serializable]
    public class Inventory {
        public List<Item> items;
        protected InventoryEventChannel InventoryChannel;
        public Entity targetEntity;
        public bool IsEquipment => targetEntity != null;
        public int Size { get; protected set; }

        public Inventory(int size = 10, InventoryEventChannel inventoryChannel = null) {
            this.InventoryChannel = inventoryChannel;
            this.Size = size;
            PopulateInventory();
        }

        public Inventory Copy() {
            Inventory inv = new Inventory(Size, InventoryChannel);
            inv.targetEntity = targetEntity;
            inv.items = new List<Item>(items);

            return inv;
        }

        public virtual int Add(Item item, int count = 1) {
            if(items == null) PopulateInventory();
            if (item.IsEmpty) return 0;

            var foundMatch = items.Find(x => x.Matches(item) && x.count < item.data.maxCount);
            if(foundMatch != null) {
                int remainder = foundMatch.count + count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder; 
                foundMatch.count += count;

                if(remainder > 0) return Add(item, remainder);
                InventoryChannel?.Invoke(this);
                return 0;
            } 
            
            if(EmptySlots() > 0) {
                int remainder = count - item.data.maxCount;
                if(remainder < 0) remainder = 0;
                count -= remainder;
                SetSlot(NextEmpty(), item, count);

                if(remainder > 0) return Add(item, remainder);
                InventoryChannel?.Invoke(this);
                return 0;
            }

            //Debug.Log("Inventory is full");
            InventoryChannel?.Invoke(this);
            return count;
        }

        public virtual int AddAt(int slot, Item item, int count = 1) {
            if(items == null) PopulateInventory();
            if(slot < 0 || slot >= items.Count) return count;
            if(count > item.data.maxCount) count = item.data.maxCount;

            if(items[slot].IsEmpty) {
                SetSlot(slot, item, count);
                return 0;
            }

            if(!items[slot].Matches(item)) return count;
            int remainder = items[slot].count + count - item.data.maxCount;
            if(remainder < 0) remainder = 0;
            count -= remainder;
            items[slot].count += count;
            InventoryChannel?.Invoke(this);
            return remainder;

            //Debug.Log("Slot is occupied");
        }

        public Item GetItem(int slot) {
            if(items == null) PopulateInventory();
            if(slot < 0 || slot >= items.Count) return null;
            return items[slot];
        }

        public virtual bool SetSlot(int slot, Item item, int count = 1, bool force = false) {
            items[slot] = item.Copy();
            items[slot].count = count;

            InventoryChannel?.Invoke(this);
            return true;
        }

        public int NextEmpty(int start = 0) {
            for(int i = start; i < items.Count; i++) {
                if(items[i].data == GameManager.emptyItem) return i;
            }
            return items.Count;
        }

        protected void PopulateInventory() {
            items = new List<Item>(Size);

            for(int i = 0; i < Size; i++) {
                items.Add(new Item(GameManager.emptyItem));
            }
        }

        public virtual bool TryAdd(Item item, int count = 1) {
            if(AvailableCount(item) < count) return false;

            Add(item, count);
            return true;
        }
        
        public virtual bool TryAddBulk(Item[] items) {
            Inventory temp = Copy();

            foreach(Item item in items) {
                if(!temp.TryAdd(item, item.count)) return false;
            }

            foreach(Item item in items) {
                Add(item, item.count);
            }

            return true;
        }

        public virtual void SetAtRandom(Item item, int count = 1, bool isSeeded = false) {
            if(items == null) PopulateInventory();
            if(AvailableCount(item) == 0) return;

            int rand;

            bool found = false;
            int iterations = 0;
            do {
                rand = isSeeded? SeededRandom.GetRange(0, Size): Random.Range(0, Size);

                if(items[rand].IsEmpty || items[rand].Matches(item)) found = true;
                iterations++;
            } while(!found && iterations < 100);

            AddAt(rand, item, count);
        }

        public bool TryRemove(Item item, int count = 1, bool fullMatch = true) {
            if(items == null) return false;
            if(!items.Exists(x => x.Matches(item, fullMatch))) return false;
            //Debug.Log("Item exists");
            if(GetCount(item, fullMatch) < count) return false;
            //Debug.Log("Count is enough");

            for(int j = items.Count - 1; j >= 0; j--) {
                if(items[j].IsEmpty) continue;
                if(!items[j].Matches(item, fullMatch)) continue;
                if(items[j].count <= count) {
                    count -= items[j].count;
                    ResetSlot(j);
                } else {
                    items[j].count -= count;
                    count = 0;
                }

                if(count != 0) continue;
                InventoryChannel?.Invoke(this);
                return true;
            }

            return false;
        }

        public bool RemoveAt(int index, int count = 1) {
            if(items == null) return false;
            if(items[index].count <= count) return false;

            items[index].count -= count;
            InventoryChannel?.Invoke(this);
            return true;
        }

        public void ResetSlot(int index) {
            items[index] = new Item(GameManager.emptyItem, 0);
            InventoryChannel?.Invoke(this);
        }

        public int GetCount(Item item, bool fullmatch = true) {
            if(items == null) return 0;
            return !items.Exists(x => x.Matches(item, fullMatch:fullmatch)) ? 0 : items.FindAll(x => x.Matches(item, fullMatch:fullmatch)).Sum(i => i.count);
        }

        public int AvailableCount(Item item, bool fullmatch = true) {
            if(items == null) return item.data.maxCount * Size;
            if(!items.Exists(x => x.Matches(item, fullMatch:fullmatch))) return EmptySlots() * item.data.maxCount;
            
            int count = items.FindAll(x => x.Matches(item, fullMatch:fullmatch)).Sum(i => item.data.maxCount - i.count);

            count += EmptySlots() * item.data.maxCount;
            return count;
        }

        public int EmptySlots() {
            return items.Count(i => i.IsEmpty);
        }

        public void Clear() {
            for(int i = 0; i < items.Count; i++) {
                ResetSlot(i);
            }
        }
    }
}