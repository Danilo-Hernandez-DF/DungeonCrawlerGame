using UnityEngine;
using System.Collections.Generic;
using System;
using Game;

namespace UtilsModule {
    [Serializable]
    public class Item {
        public ItemData data;
        public int count;
        public List<Tag> tags;
        public Sprite DisplaySprite => data.DisplaySprite;

        public Item(ItemData data, int count = 1, List<Tag> tags = default) {
            this.data = data;
            this.count = count;
            
            tags ??= new List<Tag>();
            foreach(var tag in data.inherentTags) {
                if(tag.data is TagData<int>) {
                    tags.Add(((TagData<int>)tag.data).GetTag(Mathf.FloorToInt(tag.value), true));
                    continue;
                }

                if(tag.data is TagData<float>) {
                    tags.Add(((TagData<float>)tag.data).GetTag(tag.value, true));
                    continue;
                }

                if(tag.data is TagData<EquipmentSlot>) {
                    var tValue = (EquipmentSlot)Mathf.FloorToInt(tag.value);
                    tags.Add(((TagData<EquipmentSlot>)tag.data).GetTag(tValue, true));
                    continue;
                }

                tags.Add(tag.data.GetTag(true));
            }
            this.tags = tags;
        }

        public bool HasTag(TagData tag) {
            return tags.Contains(tags.Find(x => x.data == tag));
        }

        public bool HasTag(string tag) {
            return tags.Exists(x => x.data.name == tag);
        }

        public bool Matches(Item item) {
            if(data != item.data) return false;

            if(tags.Count != item.tags.Count) return false;

            foreach(var tag in tags) { 
                if(!item.tags.Exists(x => x.Matches(tag))) return false;
            }

            return true;
        }

        public void AddTag(Tag tag) {
            if(!tags.Contains(tag)) tags.Add(tag);
        }

        public void AddTag<T>(Tag<T> tag, T value) {
            if(!tags.Contains(tag)) tags.Add(tag.SetValue(value));
        }

        public void RemoveTag(TagData tag) {
            if(!HasTag(tag)) return;
            Tag foundTag = tags.Find(x => x.data == tag);
            if(!foundTag.inherent) tags.Remove(foundTag);
        }

        public T GetTagValue<T>(TagData tag) {
            if(!HasTag(tag)) return default;
            Tag foundTag = tags.Find(x => x.data == tag);
            if(foundTag.GetType() != typeof(Tag<T>)) return default;
            return ((Tag<T>)foundTag).GetValue();
        }

        public Item Copy() { 
            var copy = new Item(data, count);
            copy.tags = new List<Tag>(tags);
            return copy;
        }

        public bool IsEmpty => data == GameManager.Instance.EmptyItem;
    }
}