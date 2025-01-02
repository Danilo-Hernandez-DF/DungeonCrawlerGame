using System;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    [Serializable]
    public class InventoryData : ISaveable{
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public Item[] Items = new Item[0];
    }
}