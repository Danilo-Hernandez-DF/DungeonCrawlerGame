using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entity Data")]
    public class EntityData : ScriptableObject {
        public string entityName;
        public EntityStats stats;
        public GameObject prefab;
        public List<Tag> tags;
        
        public SerializableGuid id = SerializableGuid.Empty;
        private void OnValidate() {
            if(id == SerializableGuid.Empty) id = SerializableGuid.NewGuid();
            Debug.Log($"Assigned {name} with id: {id.ToString()}");
        }

        public bool HasTag(string tag) {
            foreach(Tag t in tags) {
                if(t.data.name == tag) return true;
            }

            return false;
        }
    }
}