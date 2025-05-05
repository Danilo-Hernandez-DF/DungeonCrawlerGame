using Game;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(menuName = "Events/DungeonDataEventChannel")]
    public class DungeonDataEventChannel : EventChannel<DungeonData> { }
}