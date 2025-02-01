using UnityEngine;
using System.Collections.Generic;
using System;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item/Item Data")]
    public class ItemData : ScriptableObject {
        public new string name;
        [TextArea] public string description;
        public int maxCount;
        [SerializeField] Sprite sprite;
        public List<TagDefault> inherentTags;
        public Sprite DisplaySprite {get => sprite;}

        public SerializableGuid id = SerializableGuid.Empty;
        private void OnValidate() {
            if(id == SerializableGuid.Empty) id = SerializableGuid.NewGuid();
            Debug.Log($"Assigned {name} with id: {id.ToString()}");
        }

        public Item GetItem(int count = 1, List<Tag> tags = null) {
            return new Item(this, count, tags);
        }

        public bool HasTag(TagData tag, float value = 0, bool accept0 = false) {
            if(!accept0 && value == 0) return inherentTags.Exists(x => x.data == tag);
            return inherentTags.Exists(x => x.data == tag && x.value == value);
        }

        public bool HasTag(string tag, float value = 0, bool accept0 = false) {
            if(!accept0 && value == 0) return inherentTags.Exists(x => x.data.name == tag);
            return inherentTags.Exists(x => x.data.name == tag && x.value == value);
        }
    }

    [Serializable]
    public struct TagDefault {
        public TagData data;
        public float value;
    }
}