using UtilsModule;

namespace Game
{
    public class EntityDatabase : SODatabase<EntityData> {
        public override EntityData GetData(string name) {
            return Items.Find(item => item.name == name);
        }
        
        public override EntityData GetData(SerializableGuid id) {
            return Items.Find(item => item.id == id);
        }
    }
}