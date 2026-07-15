namespace Game
{
    [Serializable]
    public class SavedItem {
        public string data;
        public int count;
        public List<SavedTag> savedTags;

        public SavedItem(Item toCopy) {
            if(toCopy == null || !toCopy.data) toCopy = GameManager.emptyItem.GetItem();
            
            data = toCopy.data.nameKey;
            count = toCopy.count;
            savedTags = new List<SavedTag>();

            foreach(Tag tag in toCopy.tags) {
                savedTags.Add(new SavedTag(tag));
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