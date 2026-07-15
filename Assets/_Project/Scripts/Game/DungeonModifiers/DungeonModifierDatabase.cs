using Utils;

namespace Game
{
    public class DungeonModifierDatabase : SODatabase<DungeonModifier> {
        public override DungeonModifier GetData(string name) {
            return Items.Find(item => item.nameKey == name);
        }
    }
}