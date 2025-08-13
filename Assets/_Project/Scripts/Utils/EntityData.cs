using System.Collections.Generic;
using Localisation;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entity Data")]
    public class EntityData : ScriptableObject {
        public string entityName => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        public EntityStats stats;
        public GameObject prefab;
        public List<Tag> tags;

        public bool HasTag(string tag) {
            foreach(Tag t in tags) {
                if(t.data.name == tag) return true;
            }

            return false;
        }
    }
}