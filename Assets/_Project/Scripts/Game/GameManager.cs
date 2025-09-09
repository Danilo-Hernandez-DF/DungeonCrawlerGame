using System.Collections.Generic;
using Systems.Persistence;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UtilsModule;

namespace Game {
    public class GameManager : PersistentSingleton<GameManager> {
        //------------------------------------------------
        //Variables
        //------------------------------------------------

        [Header("Dungeon Settings")]
        [SerializeField] public DungeonData dungeonData;
        [SerializeField] private Difficulty difficulty;
        public int currentLevel = 0;

        [Header("Game Settings")]
        public static ItemData emptyItem => Instance.ItemDatabase.GetData("empty");
        public EventSystem eventSystem;
        private ItemDatabase ItemDatabase;
        private EntityDatabase EntityDatabase;
        private TagDatabase TagDatabase;
        private QuestDatabase QuestDatabase;
        public GameObject openUI;
        public InputReader input;

        public UnityAction<StatisticsTracker.TrackedStat, int> TrackStat;
        public UnityAction<ItemData, int> TrackItem;
        public UnityAction<EntityData, EntityTrack> TrackEntity;
        public UnityAction<EntityData, int> TrackLoot;
        public bool Paused => openUI != null;

        //------------------------------------------------
        //Methods
        //------------------------------------------------

        public void StartGame() {
            DungeonController.Instance.StartFloor(dungeonData);
            //GameSettings.Instance.SetDifficulty(difficulty);
        }

        public void FreshGame() {
            currentLevel = 0;
            StartGame();
        }

        public void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        public static ItemData GetItem(string name) {
            return Instance.ItemDatabase.GetData(name);
        }
        
        public static Item GetEquipment(Tag.Equipment type) {
            return Instance.ItemDatabase.GetEquipment(type);
        }
        
        public static EntityData GetEntity(string name) {
            return Instance.EntityDatabase.GetData(name);
        }
        
        public static TagData GetTag(string name) {
            return Instance.TagDatabase.GetData(name);
        }
        
        public static Quest GetQuest(string name) {
            return Instance.QuestDatabase.GetData(name);
        }
        
        protected override void Awake() {
            base.Awake();
            
            ItemDatabase = new ItemDatabase();

            foreach(ItemData data in Resources.LoadAll<ItemData>("ItemData")) {
                ItemDatabase.AddItem(data);
            }

            EntityDatabase = new EntityDatabase();
            
            foreach(EntityData data in Resources.LoadAll<EntityData>("EntityData")) {
                EntityDatabase.AddItem(data);
            }
            
            TagDatabase = new TagDatabase();
            
            foreach(TagData data in Resources.LoadAll<TagData>("TagData")) {
                TagDatabase.AddItem(data);
            }
            
            QuestDatabase = new QuestDatabase();
            
            foreach(Quest data in Resources.LoadAll<Quest>("Quest")) {
                QuestDatabase.AddItem(data);
            }
        }

        //------------------------------------------------
        
        void OnApplicationQuit() {
            input.Controls.Player.Disable();
            input.Controls.UI.Disable();
            input.Controls.Debug.Disable();
            
            MonoBehaviour[] scripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (MonoBehaviour script in scripts) {
                if(script is PersistentSingleton<GameManager>) continue;
                script.enabled = false;
            }
            
            this.enabled = false;
        }

        public void TrackStats(StatisticsTracker.TrackedStat stat, int value) {
            TrackStat?.Invoke(stat, value);
        }
        
        public void TrackEntities(EntityData stat, EntityTrack value) {
            TrackEntity?.Invoke(stat, value);
        }
        
        public void TrackItems(ItemData item, int value) {
            TrackItem?.Invoke(item, value);
        }
        
        public void TrackLoots(EntityData item, int value) {
            TrackLoot?.Invoke(item, value);
        }
    }

    public enum Difficulty {Easy, Normal, Hard, Cataclysm}

    public enum OperatorType {Add, Multiply}
}