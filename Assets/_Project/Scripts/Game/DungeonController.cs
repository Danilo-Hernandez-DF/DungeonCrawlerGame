using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Utils;
using ProcGen;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class DungeonController : Singleton<DungeonController> {
        public DungeonGenerator dungeonGenerator;
        public List<DungeonModifier> modifiers;
        public Quest completionQuest;
        private RoomController startRoom;
        [SerializeField] GameObject floorExit;
        private int currentLevel => GameManager.Instance.currentLevel;
        LootSetting[] lootSettings;
        
        public void StartFloor(DungeonData dungeonData) {
            modifiers = dungeonData.modifiers?.ToList() ?? new List<DungeonModifier>();
            lootSettings = dungeonData.lootSettings;
            completionQuest = dungeonData.completionQuest;
            dungeonGenerator.Configure(dungeonData);
            dungeonGenerator.Generate(out startRoom);
            GameSettings.Instance.AddQuest(completionQuest);
            GameManager.Instance.currentLevel++;
        }

        public LootSetting GetLootTable(LootType lootType) {
            LootSetting currentSetting = null;
            if(lootSettings == null || lootSettings.Length == 0) {
                return null;
            }

            foreach(LootSetting lootSetting in lootSettings) {
                if(lootSetting.startLevel <= currentLevel && lootSetting.lootType == lootType) {
                    if(currentSetting == null || lootSetting.startLevel > currentSetting.startLevel) {
                        currentSetting = lootSetting;
                    }
                }
            }

            if(currentSetting != null) {
                return currentSetting;
            }
           
            return null;
        }
        
        void ResetRooms() {
            foreach(RoomController room in dungeonGenerator.rooms) {
                room.Reset();
            }
        }
        
        public void CompleteRequirements() {
            var exit = Instantiate(floorExit);
            exit.transform.SetParent(startRoom.transform);
            exit.transform.localPosition = Vector2.zero;
        }
        
        // Modifier methods
        //#################

        public virtual void OnRoomGeneration(RoomController room) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnRoomGeneration(room);
            }
        }

        public virtual void OnRoomStart(RoomController room) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnRoomStart(room);
            }
        }

        public virtual void OnRoomClear(RoomController room) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnRoomClear(room);
            }
        }

        public virtual void OnEnemyHit(Enemy enemy) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnEnemyHit(enemy);
            }
        }

        public virtual void OnEnemySpawn(Enemy enemy) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnEnemySpawn(enemy);
            }
        }

        public virtual void OnEnemyDeath(Enemy enemy) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnEnemyDeath(enemy);
            }
        }

        public virtual void OnPlayerHit() {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnPlayerHit();
            }
        }

        public virtual void OnLootGenerated(Lootable loot) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnLootGenerated(loot);
            }
        }
    }
}