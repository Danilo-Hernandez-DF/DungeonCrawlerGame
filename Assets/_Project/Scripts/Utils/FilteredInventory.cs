using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UtilsModule {
    public class FilteredInventory : Inventory
    {
        private readonly List<SlotFilter> slotFilters;
        private readonly FilteredInventoryEventChannel inventoryChannel;

        public FilteredInventory(List<SlotFilter> slotFilters, FilteredInventoryEventChannel inventoryChannel = null)
        {
            this.slotFilters = slotFilters;
            Size = slotFilters.Count;
            this.inventoryChannel = inventoryChannel;
            PopulateInventory();
        }

        public override int Add(Item item, int count = 1) 
        {
            if (items == null) PopulateInventory();
            if (item.IsEmpty) return 0;

            for (int i = 0; i < slotFilters.Count; i++)
            {
                if (!slotFilters[i].Evaluate(item)) continue;
                if (!items[i].IsEmpty && !items[i].Matches(item)) continue;

                if (items[i].IsEmpty)
                {
                    SetSlot(i, item, Mathf.Max(item.data.maxCount, count));
                    if (item.data.maxCount >= count)
                    {
                        count = 0;
                        break;
                    }
                    count -= item.data.maxCount;
                    continue;
                }

                int remainder = items[i].count + count - item.data.maxCount;
                if (remainder < 0) remainder = 0;
                count -= remainder;
                items[i].count += count;
                count = remainder;

                if (count == 0) break;
            }

            inventoryChannel?.Invoke(this);
            return count;
        }

        public override int AddAt(int slot, Item item, int count = 1)
        {
            if (items == null) PopulateInventory();
            if (!slotFilters[slot].Evaluate(item))
            {
                return count;
            }
            if (slot < 0 || slot >= items.Count) return count;
            if (count > item.data.maxCount) count = item.data.maxCount;

            if (items[slot].IsEmpty)
            {
                SetSlot(slot, item, count);
                return 0;
            }

            if (!items[slot].Matches(item)) return count;
            int remainder = items[slot].count + count - item.data.maxCount;
            if (remainder < 0) remainder = 0;
            count -= remainder;
            items[slot].count += count;
            inventoryChannel?.Invoke(this);
            return remainder;
        }

        public override bool SetSlot(int slot, Item item, int count = 1, bool force = false)
        {
            if (!force) if (!slotFilters[slot].Evaluate(item))
                {
                    return false;
                }
            items[slot] = item.Copy();
            items[slot].count = count;

            inventoryChannel?.Invoke(this);
            return true;
        }

        public override bool TryAdd(Item item, int count = 1)
        {
            if (AvailableCount(item) < count) return false;
            if (!Allowed(item))
            {
                return false;
            }

            return Add(item, count) < 0;
        }

        public override void SetAtRandom(Item item, int count = 1)
        {
            if (items == null) PopulateInventory();
            if (!Allowed(item))
            {
                return;
            }
            if (AvailableCount(item) == 0) return;

            int rand;

            bool found = false;
            int iterations = 0;
            do
            {
                rand = Random.Range(0, Size);

                if ((items[rand].IsEmpty || items[rand].Matches(item)) && slotFilters[rand].Evaluate(item)) found = true;
                iterations++;
            } while (!found && iterations < 100);

            AddAt(rand, item, count);
        }

        private bool Allowed(Item item)
        {
            return slotFilters.Any(filter => filter.Evaluate(item));
        }

        private int NextAllowed(Item item, int start = 0)
        {
            for (int i = start; i < slotFilters.Count; i++)
            {
                if (slotFilters[i].Evaluate(item) && items[i].count < item.data.maxCount) return i;
            }
            return -1;
        }

        public int NextMatch(Item item, int start = 0)
        {
            for (int i = start; i < slotFilters.Count; i++)
            {
                if (slotFilters[i].Evaluate(item)) return i;
            }
            return -1;
        }
    }
}