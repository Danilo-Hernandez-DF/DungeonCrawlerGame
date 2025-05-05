using UnityEngine;
using UtilsModule;
using System;
using Systems.Persistence;

namespace Game {
    [Serializable]
    public class PlayerData : ISaveable {
        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}