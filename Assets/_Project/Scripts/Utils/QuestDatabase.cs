using Game;

namespace UtilsModule {
    public class QuestDatabase : SODatabase<Quest> {
        public override Quest GetData(string name) {
            return Items.Find(item => item.name == name);
        }
    }
}