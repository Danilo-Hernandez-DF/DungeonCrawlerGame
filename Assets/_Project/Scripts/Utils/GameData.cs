using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Serialization;
using UtilsModule;

namespace Systems.Persistence {
    [Serializable] public class GameData {
        [FormerlySerializedAs("Name")] public string name;
        [FormerlySerializedAs("CurrentLevelName")] public string currentLevelName;
        public Vector3 playerPosition;
        public List<InventoryData> inventoryData;
        public List<InventoryData> filteredInventoryData;
        public List<NPCData> NpcDatas;
        public GameSettingsData gameSettingsData;

        public ISaveable MatchID(SerializableGuid id) {
            //if(playerData.Id.Equals(id)) return playerData;
            if(gameSettingsData.Id.Equals(id)) return gameSettingsData;

            for(int i = 0; i < inventoryData.Count; i++) {
                if(inventoryData[i].Id.Equals(id)) return inventoryData[i];
            }
            
            for(int i = 0; i < filteredInventoryData.Count; i++) {
                if(filteredInventoryData[i].Id.Equals(id)) return filteredInventoryData[i];
            }
            
            for(int i = 0; i < NpcDatas.Count; i++) {
                if(NpcDatas[i].Id.Equals(id)) return NpcDatas[i];
            }

            return null;
        }

        public void MatchIDAndUpdate(SerializableGuid id, ISaveable newData) {
            if(gameSettingsData.Id.Equals(id)) {
                if(newData is GameSettingsData data) gameSettingsData = data;
            }

            for(int i = 0; i < inventoryData.Count; i++) {
                if(!inventoryData[i].Id.Equals(id)) continue;
                if(newData is InventoryData data) inventoryData[i] = data;
            }
            
            for(int i = 0; i < filteredInventoryData.Count; i++) {
                if(!inventoryData[i].Id.Equals(id)) continue;
                if(newData is InventoryData data) inventoryData[i] = data;
            }
            
            for(int i = 0; i < NpcDatas.Count; i++) {
                if(!NpcDatas[i].Id.Equals(id)) continue;
                if(newData is NPCData data) NpcDatas[i] = data;
            }
            
            Debug.Log("No data found to update!");
        }
    }
}