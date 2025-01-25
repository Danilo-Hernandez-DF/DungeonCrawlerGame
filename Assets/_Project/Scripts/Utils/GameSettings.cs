using System.Collections.Generic;
using Game;
using Systems.Persistence;
using Unity.VisualScripting;
using UnityEngine;
using UtilsModule;

namespace _Project.Scripts.Utils {
    public class GameSettings : MonoBehaviour, IBind<GameSettingsData> {
        [Header("Binding Data")]
        [SerializeField] GameSettingsData data;
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        
        public StatisticsTracker statisticsTracker;
        public List<StatisticsTracker> statsTrackers = new List<StatisticsTracker>();
        [SerializeField] Quest[] quests;

        void Awake() {
            statisticsTracker = new StatisticsTracker("Main Tracker");
            
            foreach(Quest q in quests) {
                statsTrackers.Add(new StatisticsTracker(q.name, q));
            }
        }
        
        void Update() {
            data.statisticsTracker = statisticsTracker;
            data.statKeyPairs = statisticsTracker.GetStatKeyPairs();
            data.itemKeyPairs = statisticsTracker.GetItemKeyPairs();
        }
        
        void FixedUpdate() {
            var toRemove = new List<StatisticsTracker>();
                
            foreach(StatisticsTracker tracker in statsTrackers) {
                if(!tracker.GoalAchieved()) continue;
                tracker.quest.OnCompletion();
                toRemove.Add(tracker);
            }
            
            foreach(StatisticsTracker tracker in toRemove) {
                statsTrackers.Remove(tracker);
            }
        }
        
        public void Bind(GameSettingsData data) {
            this.data = data;
            this.data.Id = Id;

            if(data.statisticsTracker != null && data.statisticsTracker.name != "") {
                statisticsTracker = new StatisticsTracker(data.statisticsTracker.name);

                foreach(var keyPair in data.statKeyPairs) {
                    statisticsTracker.TrackStat(keyPair.stat, keyPair.value);
                }
                
                foreach(var keyPair in data.itemKeyPairs) {
                    statisticsTracker.TrackItem(keyPair.stat, keyPair.value);
                }
                
                Debug.Log($"Binding GameManagers data, Id:{Id.ToHexString()}");
            }
        }

        void OnEnable() {
            GameManager.Instance.TrackStat += TrackStat;
            GameManager.Instance.TrackItem += TrackItem;
        }
        
        void OnDisable() {
            GameManager.Instance.TrackStat -= TrackStat;
            GameManager.Instance.TrackItem -= TrackItem;
        }
        
        public void TrackStat(StatisticsTracker.TrackedStat stat, int amount) {
            statisticsTracker.TrackStat(stat, amount);
            foreach(StatisticsTracker tracker in statsTrackers) {
                tracker.TrackStat(stat, amount);
            }
        }
        
        public void TrackItem(ItemData stat, int amount) {
            statisticsTracker.TrackItem(stat, amount);
            foreach(StatisticsTracker tracker in statsTrackers) {
                tracker.TrackItem(stat, amount);
            }
        }
    }
}