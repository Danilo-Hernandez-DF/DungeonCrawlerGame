using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Game;

namespace UtilsModule {
    [Serializable]
    public class Item {
        public ItemData data;
        public int count;
        public List<Tag> tags;
        public Sprite DisplaySprite => data.DisplaySprite;

        public Item(ItemData data, int count = 1, List<Tag> tags = null) {
            this.data = data;
            this.count = count;
            
            tags ??= new List<Tag>();
            if (!data) {
                //Debug.Log("ItemData is null");
                return;
            }
            foreach(var tag in data.inherentTags) {
                if(tag.data.type != TagData.TagType.None) {
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
            return tags.Exists(x => x.data.nameKey == tag);
        }
        
        public Tag GetTag(string tag) {
            return tags.Find(x => x.data.nameKey == tag);
        }
        
        public Tag GetTag(TagData tag) {
            return tags.Find(x => x.data == tag);
        }

        public bool Matches(Item item, bool fullMatch = true) {
            if(!fullMatch) return data == item.data;
            
            if(data != item.data) return false;
            return tags.Count == item.tags.Count && tags.All(tag => item.tags.Exists(x => x.Matches(tag)));
        }

        public void AddTag(Tag tag)
        {

            if (!HasTag(tag.data)) tags.Add(tag);
            else {
                GetTag(tag.data).SetValue(tag.GetValue());
            }
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
            if(!HasTag(tag)) return 0;
            Tag foundTag = tags.Find(x => x.data == tag);
            if(foundTag.GetType() != typeof(T)) return 0;
            return ((T)foundTag).GetValue();
        }

        public Item Copy() {
            var copy = new Item(data, count) {
                tags = tags == null ? new List<Tag>() : new List<Tag>(tags)
            };

            return copy;
        }

        public override string ToString() {
            return $"{data.name} x{count}";
        }

        public bool IsEmpty => data == GameManager.emptyItem;
    }
}