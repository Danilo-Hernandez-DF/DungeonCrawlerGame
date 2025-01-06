using System;
using System.Collections.Generic;
using Game;
using UnityEngine.Serialization;
using UtilsModule;

namespace Systems.Persistence {
    [Serializable] public class GameData {
        [FormerlySerializedAs("Name")] public string name;
        [FormerlySerializedAs("CurrentLevelName")] public string currentLevelName;
        public PlayerData playerData;
        public List<InventoryData> inventoryData;
        public List<InventoryData> filteredInventoryData;
    }
}