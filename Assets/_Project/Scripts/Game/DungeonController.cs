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
        [SerializeField] DungeonData dungeonData;
        public List<DungeonModifier> modifiers;
        public Quest completionQuest;
        private RoomController startRoom;
        [SerializeField] GameObject floorExit;
        private int currentLevel => GameManager.Instance.currentLevel;
        LootSetting[] lootSettings;

        private new void Awake() {
            base.Awake();
            modifiers = new List<DungeonModifier>();
        }
        
        public float objectivePercentage => completionQuest?.GetPercentage() ?? 0;

        public void StartFloor(DungeonData dungeonData, List<DungeonModifier> mods = null)
        {
            this.dungeonData = dungeonData;
            modifiers = mods ?? new List<DungeonModifier>();
            lootSettings = dungeonData.lootSettings;
            completionQuest = dungeonData.completionQuest;
            dungeonGenerator.Configure(dungeonData);
            
            dungeonGenerator.Generate(out startRoom);

            if (completionQuest == null)
            {
                CompleteRequirements();
            }
            else
            {
                completionQuest.tracker = GameSettings.Instance.AddQuest(completionQuest);
            }
            
            PlayerHUD.Instance.Init();
        }

        public LootSetting GetLootTable(LootType lootType) {
            LootSetting currentSetting = null;
            if(lootSettings == null || lootSettings.Length == 0) {
                return null;
            }

            foreach(LootSetting lootSetting in lootSettings) {
                if(lootSetting.lootType == lootType) {
                    currentSetting = lootSetting;
                    break;
                }
            }

            return currentSetting;
        }
        
        void ResetRooms() {
            foreach(RoomController room in dungeonGenerator.rooms) {
                room.Reset();
            }
        }
        
        public void CompleteRequirements() {
            if(floorExit == null) return;
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

        public virtual void OnEnemyHit(Enemy enemy, DamageSource source) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnEnemyHit(enemy, source);
            }
        }

        public virtual void OnEnemySpawn(Enemy enemy) {
            enemy.Init();
            
            foreach(DungeonModifier mod in modifiers) {
                mod.OnEnemySpawn(enemy);
            }
        }

        public virtual void OnEnemyDeath(Enemy enemy, DamageSource source) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnEnemyDeath(enemy, source);
            }
        }

        public virtual void OnPlayerHit(DamageSource source) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnPlayerHit(source);
            }
        }

        public virtual void OnLootGenerated(Lootable loot) {
            foreach(DungeonModifier mod in modifiers) {
                mod.OnLootGenerated(loot);
            }
        }
    }
}