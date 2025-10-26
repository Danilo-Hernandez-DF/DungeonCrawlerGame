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

        public int GetRarity() {
            return data.rarity;
        }

        public Item(ItemData data, int count = 1, List<Tag> addTags = null) {
            this.data = data;
            this.count = count;
            
            if (!data) {
                this.data = GameManager.emptyItem;
                this.count = 0;
                tags = new List<Tag>();
                return;
            }
            
            tags = new List<Tag>();
            
            foreach(var tag in data.inherentTags) {
                if(tag.data.type != TagData.TagType.None) {
                    tags.Add(tag.data.GetTag(tag.value, true));
                }
                else {
                    tags.Add(tag.data.GetTag(0, true));
                }
            }
            
            addTags ??= new List<Tag>();
            foreach(var tag in addTags) {
                if (!tags.Exists(x => x.data == tag.data)) {
                    tags.Add(tag);
                } else {
                    var found = tags.Find(x => x.data == tag.data);
                    if (found.data.type != TagData.TagType.None) {
                        found.value = tag.value;
                    }
                }
            }
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

        public void RemoveTag(TagData tag) {
            if(!HasTag(tag)) return;
            Tag foundTag = tags.Find(x => x.data == tag);
            if(!foundTag.inherent) tags.Remove(foundTag);
        }

        public Item Copy() {
            var copyTags = new List<Tag>();
            foreach(var tag in tags) {
                copyTags.Add(tag.Copy());
            }
            
            var copy = new Item(data, count, copyTags);

            return copy;
        }

        public override string ToString() {
            return $"{data.name} x{count}";
        }

        public bool IsEmpty => data == GameManager.emptyItem;
    }
}