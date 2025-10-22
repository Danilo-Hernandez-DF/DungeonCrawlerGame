using System.Collections;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Localisation.LocalisationSystem;
using UtilsModule;

namespace Game
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        //------------------------------------------------
        //Variables
        //------------------------------------------------

        public GlobalSettings globalSettings;

        [Header("Dungeon Settings")]
        [SerializeField] public DungeonData dungeonData;
        [SerializeField] private Difficulty difficulty;
        public int currentLevel = 0;

        [Header("Game Settings")]
        public static ItemData emptyItem => Instance.ItemDatabase.GetData("empty");
        public static TagData enemyTag => Instance.TagDatabase.GetData("tag_enemy");
        public static TagData collectibleTag => Instance.TagDatabase.GetData("tag_collectible");
        public static TagData lootableTag => Instance.TagDatabase.GetData("tag_lootable");
        public static TagData spawnerTag => Instance.TagDatabase.GetData("tag_spawner");
        public EventSystem eventSystem;
        private ItemDatabase ItemDatabase;
        private EntityDatabase EntityDatabase;
        private TagDatabase TagDatabase;
        private QuestDatabase QuestDatabase;
        private StatusEffectDatabase StatusEffectDatabase;
        private ItemLootTableDatabase ItemLootTableDatabase;
        public GameObject openUI;
        public InputReader input;
        [Header("Material Settings")]
        public Material entityHitmaterial;

        [Header("Effect Settings")]
        public GameObject hitEffectPrefab;

        public Sprite partcleHitTexture;

        public UnityAction<StatisticsTracker.TrackedStat, int> TrackStat;
        public UnityAction<ItemData, int> TrackItem;
        public UnityAction<EntityData, EntityTrack> TrackEntity;
        public UnityAction<EntityData, int> TrackLoot;
        public bool Paused => openUI != null;
        

        //------------------------------------------------
        //Methods
        //------------------------------------------------

        public IEnumerator StartGame(DungeonType dungeonType)
        {
            yield return null;
            DungeonController.Instance.StartFloor(dungeonType.variants.GetWeightedItem());
        }

        public IEnumerator FreshGame(DungeonType dungeonType)
        {
            yield return null;
            currentLevel = 0;
            StartCoroutine(StartGame(dungeonType));
        }

        public void LoadScene(string sceneName,DungeonType dungeonType, bool resetLevel)
        {
            SceneManager.LoadScene(sceneName);
            if (resetLevel) StartCoroutine(FreshGame(dungeonType));
            else StartCoroutine(StartGame(dungeonType));
        }

        public static ItemData GetItem(string name)
        {
            return Instance.ItemDatabase.GetData(name);
        }

        public static Item GetEquipment(Tag.Equipment type)
        {
            return Instance.ItemDatabase.GetEquipment(type);
        }

        public static EntityData GetEntity(string name)
        {
            return Instance.EntityDatabase.GetData(name);
        }

        public static TagData GetTag(string name)
        {
            return Instance.TagDatabase.GetData(name);
        }

        public static Quest GetQuest(string name)
        {
            return Instance.QuestDatabase.GetData(name);
        }

        public static StatusEffectData GetStatusEffect(string name)
        {
            return Instance.StatusEffectDatabase.GetData(name);
        }

        public static ItemLootTable GetItemLootTable(string name)
        {
            return Instance.ItemLootTableDatabase.GetData(name);
        }

        protected override void Awake()
        {
            base.Awake();

            ItemDatabase = new ItemDatabase();

            foreach (ItemData data in Resources.LoadAll<ItemData>("ItemData"))
            {
                ItemDatabase.AddItem(data);
            }

            EntityDatabase = new EntityDatabase();

            foreach (EntityData data in Resources.LoadAll<EntityData>("EntityData"))
            {
                EntityDatabase.AddItem(data);
            }

            TagDatabase = new TagDatabase();

            foreach (TagData data in Resources.LoadAll<TagData>("TagData"))
            {
                TagDatabase.AddItem(data);
            }

            QuestDatabase = new QuestDatabase();

            foreach (Quest data in Resources.LoadAll<Quest>("Quest"))
            {
                QuestDatabase.AddItem(data);
            }

            StatusEffectDatabase = new StatusEffectDatabase();

            foreach (StatusEffectData data in Resources.LoadAll<StatusEffectData>("StatusEffectData"))
            {
                StatusEffectDatabase.AddItem(data);
            }

            ItemLootTableDatabase = new ItemLootTableDatabase();

            foreach (ItemLootTable data in Resources.LoadAll<ItemLootTable>("LootTables/ItemTables"))
            {
                ItemLootTableDatabase.AddItem(data);
            }
        }

        void Start()
        {
            SaveLoadSystem.Instance.LoadGlobalSettings();
            SetLanguage(globalSettings.localisedLanguage);
        }

        //------------------------------------------------

        void OnApplicationQuit()
        {
            input.Controls.Player.Disable();
            input.Controls.UI.Disable();
            input.Controls.Debug.Disable();

            SaveLoadSystem.Instance.SaveGlobalSettings();

            MonoBehaviour[] scripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (MonoBehaviour script in scripts)
            {
                if (script is PersistentSingleton<GameManager>) continue;
                script.enabled = false;
            }

            this.enabled = false;
        }

        public void TrackStats(StatisticsTracker.TrackedStat stat, int value)
        {
            TrackStat?.Invoke(stat, value);
        }

        public void TrackEntities(EntityData stat, EntityTrack value)
        {
            TrackEntity?.Invoke(stat, value);
        }

        public void TrackItems(ItemData item, int value)
        {
            TrackItem?.Invoke(item, value);
        }

        public void TrackLoots(EntityData item, int value)
        {
            TrackLoot?.Invoke(item, value);
        }
    }

    public enum Difficulty { Easy, Normal, Hard, Cataclysm }

    public enum OperatorType { Add, Multiply }

    public struct GlobalSettings
    {
        public LocalisedLanguage localisedLanguage;
        public int masterVolume;
        public int musicVolume;
        public int sfxVolume;
        public int uiVolume;
    }
}

