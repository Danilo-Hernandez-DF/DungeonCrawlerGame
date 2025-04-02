using Game;

namespace UtilsModule
{
    public class TagDatabase : SODatabase<TagData> {
        public override TagData GetData(string name) {
            return Items.Find(item => item.name == name);
        }
    }
}