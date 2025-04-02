using ProcGen;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class DungeonModifier : ScriptableObject {
        public virtual void OnRoomGeneration(RoomController room) { }
        
        public virtual void OnRoomStart(RoomController room) { }
        
        public virtual void OnRoomClear(RoomController room) { }
        
        public virtual void OnEnemyHit(Enemy enemy) { }
        
        public virtual void OnEnemySpawn(Enemy enemy) { }
        
        public virtual void OnEnemyDeath(Enemy enemy) { }
        
        public virtual void OnPlayerHit() { }
        
        public virtual void OnLootGenerated(Lootable loot) { }
    }
}