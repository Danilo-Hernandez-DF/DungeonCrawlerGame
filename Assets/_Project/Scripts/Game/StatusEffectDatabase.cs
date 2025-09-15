using UtilsModule;

namespace Game {
    internal class StatusEffectDatabase : SODatabase<StatusEffectData>
    {
        public override StatusEffectData GetData(string name) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }
            return Items.Find(item => item.nameKey == name);
        }
    }
}