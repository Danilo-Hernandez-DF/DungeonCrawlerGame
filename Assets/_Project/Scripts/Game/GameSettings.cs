using Utils;

namespace Game {
    public class GameSettings : Singleton<GameSettings>, IBind<GameSettingsData> {
        [Header("Binding Data")]
        [SerializeField] GameSettingsData data;
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        
        public StatisticsTracker statisticsTracker;
        public List<StatisticsTracker> statsTrackers = new List<StatisticsTracker>();

        public SaveableInventoryHolder questRewardInv;

        new void Awake() {
            base.Awake();
            statisticsTracker = new StatisticsTracker("Main Tracker");
            questRewardInv = GetComponent<SaveableInventoryHolder>();
        }

        public StatisticsTracker AddQuest(Quest quest, NPC questGiver = null) {
            var tracker = new StatisticsTracker(quest.name, quest, questGiver);
            statsTrackers.Add(tracker);
            return tracker;
        }
        
        void Update() {
            data.mainTracker = new TrackerData() {
                trackerName = statisticsTracker.name,
                statKeyPairs = statisticsTracker.GetStatKeyPairs(),
                itemKeyPairs = statisticsTracker.GetItemKeyPairs(),
                entityKeyPairs = statisticsTracker.GetEntityKeyPairs(),
                lootKeyPairs = statisticsTracker.GetLootKeyPairs()
            };
            if(statisticsTracker.quest) data.mainTracker.questName = statisticsTracker.quest.name;
            else data.mainTracker.questName = null;

            data.trackers = new TrackerData[statsTrackers.Count];
            for (int i = 0; i < statsTrackers.Count; i++) {
                data.trackers[i] = new TrackerData() {
                    trackerName = statsTrackers[i].name,
                    statKeyPairs = statsTrackers[i].GetStatKeyPairs(),
                    itemKeyPairs = statsTrackers[i].GetItemKeyPairs(),
                    entityKeyPairs = statsTrackers[i].GetEntityKeyPairs(),
                    lootKeyPairs = statsTrackers[i].GetLootKeyPairs()
                };
            }
        }
        
        void FixedUpdate() {
            var toRemove = new List<StatisticsTracker>();
                
            foreach(StatisticsTracker tracker in statsTrackers) {
                if(tracker.quest == null) {
                    toRemove.Add(tracker);
                    continue;
                }
                if(!tracker.GoalAchieved()) continue;
                tracker.quest.OnCompletion(tracker.questGiver);
                toRemove.Add(tracker);
            }
            
            foreach(StatisticsTracker tracker in toRemove) {
                statsTrackers.Remove(tracker);
            }
        }
        
        public void Bind(GameSettingsData data) {
            this.data = data;
            this.data.Id = Id;

            if(data.mainTracker.trackerName != "" && data.mainTracker.trackerName != null) {
                statisticsTracker = BindTracker(data.mainTracker);
            } else {
                statisticsTracker = new StatisticsTracker("Main Tracker");
            }

            statsTrackers = new();
            foreach(var keyPair in data.trackers) {
                if(GameManager.GetQuest(keyPair.questName) is DungeonCompletionQuest) {
                    continue;
                }
                statsTrackers.Add(BindTracker(keyPair));
            }
        }

        public StatisticsTracker BindTracker(TrackerData tracker) {
            var newTracker = new StatisticsTracker(tracker.trackerName);
            if(tracker.trackerName != null && tracker.trackerName != "") {
                newTracker.quest = GameManager.GetQuest(tracker.questName);
            }

            foreach(var keyPair in tracker.statKeyPairs) {
                newTracker.TrackStat(keyPair.stat, keyPair.value);
            }
                
            foreach(var keyPair in tracker.itemKeyPairs) {
                newTracker.TrackItem(GameManager.GetItem(keyPair.stat), keyPair.value); 
            }
                
            foreach(var keyPair in tracker.entityKeyPairs) {
                newTracker.TrackEntity(GameManager.GetEntity(keyPair.stat), keyPair.value);
            }
                
            foreach(var keyPair in tracker.lootKeyPairs) {
                newTracker.TrackEntity(GameManager.GetEntity(keyPair.stat));
            }

            return newTracker;
        }

        void OnEnable() {
            GameManager.Instance.TrackStat += TrackStat;
            GameManager.Instance.TrackItem += TrackItem;
            GameManager.Instance.TrackEntity += TrackEntity;
            GameManager.Instance.TrackLoot += TrackLoot;
            GameManager.Instance.input.CheckQuests += OnCheckQuests;
        }
        
        void OnDisable() {
            GameManager.Instance.TrackStat -= TrackStat;
            GameManager.Instance.TrackItem -= TrackItem;
            GameManager.Instance.TrackEntity -= TrackEntity;
            GameManager.Instance.TrackLoot -= TrackLoot;
            GameManager.Instance.input.CheckQuests -= OnCheckQuests;
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
        
        public void TrackEntity(EntityData stat, EntityTrack amount) {
            statisticsTracker.TrackEntity(stat, amount);
            foreach(StatisticsTracker tracker in statsTrackers) {
                tracker.TrackEntity(stat, amount);
            }
        }

        public void TrackLoot(EntityData stat, int amount) {
            statisticsTracker.TrackEntity(stat);
            foreach(StatisticsTracker tracker in statsTrackers) {
                tracker.TrackEntity(stat);
            }
        }

        public void OnCheckQuests() {
            questRewardInv.inventoryChannel?.Invoke(questRewardInv.Inventory);
            questRewardInv.lootUIChannel?.Invoke(new Empty());
        }
    }
}