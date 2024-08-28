using UnityEngine;

namespace UtilsModule {
    public class Item {
        public ItemData data;
        public int count;
    }

    public class ItemData : ScriptableObject {
        public new string name;
        public int maxCount;
        [SerializeField] Sprite sprite;

        public Sprite DisplaySprite {get => sprite;}
    }
}