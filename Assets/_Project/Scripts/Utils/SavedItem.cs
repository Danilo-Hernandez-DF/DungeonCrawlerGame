using System;
using System.Collections.Generic;
using Game;

namespace UtilsModule
{
    [Serializable]
    public class SavedItem {
        public string data;
        public int count;
        public List<SavedTag> savedTags;

        public SavedItem(Item toCopy) {
            if(toCopy == null || !toCopy.data) toCopy = GameManager.emptyItem.GetItem(tags: new List<Tag>());
            data = toCopy.data.nameKey;
            count = toCopy.count;
            savedTags = new List<SavedTag>();

            foreach(Tag tag in toCopy.tags) {
                if(!tag.inherent) savedTags.Add(new SavedTag(tag));
            }
        }

        public Item ToItem() {
            List<Tag> tags = new();

            foreach(SavedTag tag in savedTags) {
                tags.Add(tag.ToTag());
            }
            
            return new Item(GameManager.GetItem(data), count, tags);
        }
    }
}