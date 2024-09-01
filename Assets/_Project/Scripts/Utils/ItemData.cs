using UnityEngine;
using System.Collections.Generic;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item Data")]
    public class ItemData : ScriptableObject {
        public new string name;
        public int maxCount;
        [SerializeField] Sprite sprite;
        public List<TagData> inherentTags;
        public Sprite DisplaySprite {get => sprite;}
    }
}