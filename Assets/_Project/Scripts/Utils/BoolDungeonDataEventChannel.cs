using Game;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(menuName = "Events/Bool-DungeonDataEventChannel")]
    public class BoolDungeonDataEventChannel : EventChannel<bool, DungeonData[]> { }
}