using Utils;

namespace Game
{
    public class EntityDatabase : SODatabase<EntityData> {
        public override EntityData GetData(string name) {
            return Items.Find(item => item.nameKey == name);
        }
    }
}