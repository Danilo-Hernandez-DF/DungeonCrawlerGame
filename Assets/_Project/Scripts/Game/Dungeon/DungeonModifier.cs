using Localisation;
using ProcGen;

namespace Game {
    public class DungeonModifier : ScriptableObject {
        [SerializeField] public Sprite icon;
        [SerializeField] public string nameKey;
        [SerializeField] public string descriptionKey;

        public virtual string Description() {
            return LocalisationSystem.GetLocalisedValue(descriptionKey);
        } 
        public virtual List<ItemData> GetItems() {
            return new List<ItemData>();
        } 
        public virtual List<EntityData> GetEnemies() {
            return new List<EntityData>();
        } 
        public string Name => nameKey;
        public virtual void OnRoomGeneration(RoomController room) { }
        
        public virtual void OnRoomEntered(RoomController room) { }
        
        public virtual void OnRoomStart(RoomController room) { }
        
        public virtual void OnRoomClear(RoomController room) { }
        
        public virtual void OnEnemyHit(Enemy enemy, DamageSource source) { }
        
        public virtual void OnEnemySpawn(Enemy enemy) { }
        
        public virtual void OnEnemyDeath(Enemy enemy, DamageSource source) { }
        
        public virtual void OnPlayerHit(DamageSource source) { }
        
        public virtual void OnLootGenerated(Lootable loot) { }

        public virtual void Reset() { }
    }
}