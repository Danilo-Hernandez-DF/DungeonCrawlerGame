using Utils;

namespace Game
{
    public class TagDatabase : SODatabase<TagData> {
        public override TagData GetData(string name) {
            return Items.Find(item => item.nameKey == name);
        }
    }
}