using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

namespace UtilsModule {
    public class Item {
        public ItemData data;
        public int count;
        public List<Tag> tags;
        public Sprite DisplaySprite => data.DisplaySprite;

        public Item(ItemData data, int count = 1, List<Tag> tags = default) {
            this.data = data;
            this.count = count;
            
            if(tags == null) tags = new List<Tag>();
            foreach(var tag in data.inherentTags) {
                tags.Add(tag.GetTag(true));
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

            foreach(var tag in tags) { 
                if(!item.HasTag(tag.data)) return false;
                if(!tag.Matches(item.tags.Find(x => x.data == tag.data))) return false;
            }

            return true;
        }

        public void AddTag(Tag tag) {
            if(!tags.Contains(tag)) tags.Add(tag);
        }

        public void RemoveTag(TagData tag) {
            if(!HasTag(tag)) return;
            if(!tags.Find(x => x.data == tag).inherent) tags.Remove(tags.Find(x => x.data == tag));
        }

        public T GetTagValue<T>(TagData tag) {
            if(!HasTag(tag)) return default;
            if(tags.Find(x => x.data == tag).GetType() != typeof(ValueTag<T>)) return default;
            return ((ValueTag<T>)tags.Find(x => x.data == tag)).GetValue();
        }
    }
}