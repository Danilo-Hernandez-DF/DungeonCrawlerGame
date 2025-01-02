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
                if(tag.data.type == TagData.TagType.Int) {
                    tags.Add(tag.data.GetTag(tag.value, true));
                    continue;
                }

                if(tag.data.type == TagData.TagType.Float) {
                    tags.Add(tag.data.GetTag(tag.value, true));
                    continue;
                }

                if(tag.data.type == TagData.TagType.Equipment) {
                    tags.Add(tag.data.GetTag(tag.value, true));
                    continue;
                }

                tags.Add(tag.data.GetTag(0, true));
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

        public void AddTag<T>(T tag, float value) where T : Tag {
            if(!tags.Contains(tag)) tags.Add(tag.SetValue(value));
        }

        public void RemoveTag(TagData tag) {
            if(!HasTag(tag)) return;
            Tag foundTag = tags.Find(x => x.data == tag);
            if(!foundTag.inherent) tags.Remove(foundTag);
        }

        public float GetTagValue<T>(TagData tag) where T : Tag {
            if(!HasTag(tag)) return default;
            Tag foundTag = tags.Find(x => x.data == tag);
            if(foundTag.GetType() != typeof(T)) return default;
            return (float)((T)foundTag).GetValue();
        }

        public Item Copy() { 
            var copy = new Item(data, count);
            copy.tags = new List<Tag>(tags);
            return copy;
        }

        public bool IsEmpty => data == GameManager.Instance.EmptyItem;
    }
}