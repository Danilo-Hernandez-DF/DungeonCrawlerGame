using Utils;

namespace Game {
    [CreateAssetMenu(menuName = "Events/Bool-DungeonDataEventChannel")]
    public class BoolDungeonDataEventChannel : EventChannel<bool, DungeonData[]> { }
}