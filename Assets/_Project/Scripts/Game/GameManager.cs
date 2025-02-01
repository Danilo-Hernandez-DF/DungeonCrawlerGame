using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UtilsModule;

namespace Game {
    public class GameManager : PersistentSingleton<GameManager> {
        //------------------------------------------------
        //Variables
        //------------------------------------------------

        [FormerlySerializedAs("EmptyItem")] public ItemData emptyItem;
        public EventSystem eventSystem;
        public ItemDatabase ItemDatabase;
        public EntityDatabase EntityDatabase;
        public TagDatabase TagDatabase;
        public GameObject openUI;
        public InputReader input;

        public UnityAction<StatisticsTracker.TrackedStat, int> TrackStat;
        public UnityAction<ItemData, int> TrackItem;
        public UnityAction<EntityData, EntityTrack> TrackEntity;
        
        public bool Paused => openUI != null;

        //------------------------------------------------
        //Methods
        //------------------------------------------------

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
        }

        //------------------------------------------------
        
        void OnApplicationQuit() {
            input.Controls.Player.Disable();
            input.Controls.UI.Disable();
            input.Controls.Debug.Disable();
            
            MonoBehaviour[] scripts = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
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
    }

    public enum OperatorType {Add, Multiply}
}