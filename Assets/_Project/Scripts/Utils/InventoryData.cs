using System;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.Serialization;

namespace UtilsModule {
    [Serializable]
    public class InventoryData : ISaveable{
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public SavedItem[] items = Array.Empty<SavedItem>();
    }
}