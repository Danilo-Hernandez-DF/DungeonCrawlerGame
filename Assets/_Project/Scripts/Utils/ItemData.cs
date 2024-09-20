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

        public virtual Item GetItem(int count = 1, List<Tag> tags = default) {
            return new Item(this, count, tags);
        }
    }

    [Serializable]
    public struct TagDefault {
        public TagData data;
        public float value;
    }
}