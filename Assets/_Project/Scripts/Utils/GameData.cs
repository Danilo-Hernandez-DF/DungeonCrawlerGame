using System;
using System.Collections.Generic;
using Game;
using UtilsModule;

namespace Systems.Persistence {
    [Serializable] public class GameData {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
        public List<InventoryData> inventoryData;
        public List<InventoryData> filteredInventoryData;
    }
}