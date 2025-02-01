using System;
using System.Collections.Generic;
using Game;

namespace UtilsModule
{
    [Serializable]
    public class SavedItem {
        public SerializableGuid data;
        public int count;
        public List<SavedTag> savedTags;

        public SavedItem(Item toCopy) {
            data = toCopy.data.id;
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
            
            return new Item(GameManager.Instance.ItemDatabase.GetData(data), count, tags);
        }
    }
}