using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UtilsModule {
    public class FilteredInventory : Inventory {
        private List<SlotFilter> slotFilters;
        private new FilteredInventoryEventChannel inventoryChannel;

        public FilteredInventory(List<SlotFilter> slotFilters, FilteredInventoryEventChannel inventoryChannel = null) {
            this.slotFilters = slotFilters;
            Size = slotFilters.Count;
            this.inventoryChannel = inventoryChannel;
            PopulateInventory();
        }

        public override int Add(Item item, int count = 1) {
            if(items == null) PopulateInventory();
            if(!Allowed(item)) {
                //Debug.Log("Item not allowed");
                return count;
            }

            var foundMatch = items[NextAllowed(item)];
            
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

            //Debug.Log("Inventory is full");
            inventoryChannel?.Invoke(this);
            return count;
        }

        public override int AddAt(int slot, Item item, int count = 1) {
            if(items == null) PopulateInventory();
            if(!slotFilters[slot].Evaluate(item)) {
                //Debug.Log("Item not allowed");
                return count;
            }
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

            //Debug.Log("Slot is occupied");
            return count;
        }

        public override bool SetSlot(int slot, Item item, int count = 1, bool force = false) {
            if(!force) if(!slotFilters[slot].Evaluate(item)) {
                //Debug.Log("Item not allowed");
                return false;
            }
            items[slot] = item.Copy();
            items[slot].count = count;

            inventoryChannel?.Invoke(this);
            return true;
        }

        public override bool TryAdd(Item item, int count = 1) {
            if(AvailableCount(item) < count) return false;
            if(!Allowed(item)) {
                //Debug.Log("Item not allowed");
                return false;
            }

            return Add(item, count) < 0;
        }

        public override void SetAtRandom(Item item, int count = 1, bool isSeeded = false) {
            if(items == null) PopulateInventory();
            if(!Allowed(item)) {
                //Debug.Log("Item not allowed");
                return;
            }
            if(AvailableCount(item) == 0) return;

            int rand;

            bool found = false;
            int iterations = 0;
            do {
                rand = isSeeded? SeededRandom.GetRange(0, Size): Random.Range(0, Size);

                if((items[rand].IsEmpty || items[rand].Matches(item)) && slotFilters[rand].Evaluate(item)) found = true;
                iterations++;
            } while(!found && iterations < 100);

            AddAt(rand, item, count);
        }

        private bool Allowed(Item item) {
            foreach(var filter in slotFilters) {
                if(!filter.Evaluate(item)) return false;
            }
            return true;
        }

        public int NextAllowed(Item item, int start = 0) {
            for(int i = start; i < slotFilters.Count; i++) {
                if(slotFilters[i].Evaluate(item) && items[i].count < item.data.maxCount) return i;
            }
            return -1;
        }

        public int NextMatch(Item item, int start = 0) {
            for(int i = start; i < slotFilters.Count; i++) {
                if(slotFilters[i].Evaluate(item)) return i;
            }
            return -1;
        }
    }
}