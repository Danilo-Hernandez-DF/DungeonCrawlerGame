using ProcGen;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class DungeonModifier : ScriptableObject {
        [SerializeField] public Sprite icon;
        [SerializeField] public string nameKey;
        public string Name => nameKey;
        public virtual void OnRoomGeneration(RoomController room) { }
        
        public virtual void OnRoomStart(RoomController room) { }
        
        public virtual void OnRoomClear(RoomController room) { }
        
        public virtual void OnEnemyHit(Enemy enemy, DamageSource source) { }
        
        public virtual void OnEnemySpawn(Enemy enemy) { }
        
        public virtual void OnEnemyDeath(Enemy enemy, DamageSource source) { }
        
        public virtual void OnPlayerHit(DamageSource source) { }
        
        public virtual void OnLootGenerated(Lootable loot) { }
    }
}