using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "CollectibleData", menuName = "Data/Collectible Data")]
    public class CollectibleData : EntityData {
        public int score;
    }
}