using Game;

namespace UtilsModule {
    public class QuestDatabase : SODatabase<Quest> {
        public override Quest GetData(string name) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }
            return Items.Find(item => item.nameKey == name);
        }
    }
}