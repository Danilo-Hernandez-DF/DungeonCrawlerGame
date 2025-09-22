using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Project.Scripts.Utils;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsModule;
using static Localisation.LocalisationSystem;

namespace Systems.Persistence {
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        public GameData gameData;
        FileDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MainMenuScene") return;
            BindAll();

            if (SceneData.Instance.canBeSaved)
            {
                gameData.currentLevelName = scene.name;
            }
        }

        void BindAll()
        {
            //Bind<PlayerController, PlayerData>(gameData.playerData);
            Bind<SaveableInventoryHolder, InventoryData>(gameData.inventoryData);
            Bind<SaveableFilteredHolder, InventoryData>(gameData.filteredInventoryData);
            Bind<GameSettings, GameSettingsData>(gameData.gameSettingsData);
            Bind<NPC, NPCData>(gameData.NpcDatas);
        }

        public ISaveable GetData(SerializableGuid id)
        {
            SaveGame();
            GameData temp = dataService.Load(gameData.name);
            return temp.MatchID(id);
        }

        public void SetData(SerializableGuid id, ISaveable newVal)
        {
            SaveGame();
            GameData temp = dataService.Load(gameData.name);
            temp.MatchIDAndUpdate(id, newVal);
            gameData = temp;
            SaveGame();
            BindAll();
        }

        static void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity == null) return;
            data ??= new TData { Id = entity.Id };
            entity.Bind(data);
        }

        static void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            if (datas == null) return;

            foreach (var entity in entities)
            {
                if (!entity) continue;
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData() { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame(string gameName = "New Game")
        {
            if (gameName == "New Game")
            {
                gameName = "Game " + (GetSavedGames().Count + 1);
            }

            gameData = new GameData
            {
                name = gameName,
                currentLevelName = "HubScene"
            };
            SaveGame();
            LoadGame(gameData.name + "_temp");
        }

        public void SaveGame(bool tempSave = true) => dataService.Save(gameData, true, tempSave);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);

            if (String.IsNullOrWhiteSpace(gameData.currentLevelName))
            {
                NewGame(gameName);
            }

            SceneManager.LoadScene(gameData.currentLevelName);
        }

        public void ReloadGame() => LoadGame(gameData.name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);

        public List<GameData> GetSavedGames()
        {
            List<GameData> games = new List<GameData>();
            foreach (string fileName in dataService.ListSaves())
            {
                if (fileName.EndsWith("_temp")) continue;
                games.Add(dataService.Load(fileName));
            }

            return games;
        }

        public void SaveGlobalSettings()
        {
            string datapath = dataService.GetPathToFile(Path.Combine(Application.persistentDataPath, "globalSettings"));
            File.WriteAllText(datapath, dataService.serializer.Serialize(GameManager.Instance.globalSettings));
        }

        public void LoadGlobalSettings()
        {
            string datapath = dataService.GetPathToFile(Path.Combine(Application.persistentDataPath, "globalSettings"));
            GlobalSettings settings;
            if (File.Exists(datapath))
            {
                settings = dataService.serializer.Deserialize<GlobalSettings>(File.ReadAllText(datapath));
                GameManager.Instance.globalSettings = settings;
            }
            else
            {
                settings = new GlobalSettings()
                {
                    localisedLanguage = LocalisedLanguage.English,
                    masterVolume = 50,
                    musicVolume = 50,
                    sfxVolume = 50,
                    uiVolume = 50
                };
                GameManager.Instance.globalSettings = settings;
                SaveGlobalSettings();
            }
        }

        void OnApplicationQuit()
        {
            DeleteTempSaves();         
        }

        public void DeleteTempSaves()
        {
            List<string> saves = dataService.ListSaves().ToList();
            foreach (string save in saves)
            {
                if (save.EndsWith("_temp"))
                {
                    dataService.Delete(save);
                    Debug.Log($"Deleted {save}");
                }
            }
        }
    }
}