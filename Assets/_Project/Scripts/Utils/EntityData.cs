using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entity Data")]
    public class EntityData : ScriptableObject {
        public GameObject prefab;
    }
}