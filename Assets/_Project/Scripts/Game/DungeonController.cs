using System;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using ProcGen;
using UtilsModule;

namespace Game {
    public class DungeonController : Singleton<DungeonController> {
        public DungeonGenerator dungeonGenerator;
        public List<DungeonModifier> modifiers;
        public Quest completionQuest;
        private bool exitable;
        
        public void Start() {
            dungeonGenerator.Generate();
            GameSettings.Instance.AddQuest(completionQuest);
        }
        
        void ResetRooms() {
            foreach(RoomController room in dungeonGenerator.rooms) {
                room.Reset();
            }
        }
        
        public void CompleteRequirements() {
            exitable = true;
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