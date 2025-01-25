using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Utils;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsModule;

namespace Systems.Persistence {
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem> {
        public GameData gameData;
        [SerializeField] private bool loadDevGame;
        [SerializeField] private bool loadOnStart;

        IDataService dataService;
        
        protected override void Awake() {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }
        
        void Start() {
            if(loadDevGame) {
                gameData.name = "DevSaveFile";
            }

            if (loadOnStart) {
                LoadGame(gameData.name);
            }
        }

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if(scene.name == "Menu") return;
            Bind<PlayerController, PlayerData>(gameData.playerData);
            Bind<SaveableInventoryHolder, InventoryData>(gameData.inventoryData);
            Bind<SaveableFilteredHolder, InventoryData>(gameData.filteredInventoryData);
            Bind<GameSettings, GameSettingsData>(gameData.gameSettingsData);
        }

        static void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if(entity == null) return;
            data ??= new TData { Id = entity.Id };
            entity.Bind(data);
        }

        static void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            if(datas == null) return;
            
            foreach(var entity in entities) {
                if (!entity) continue;
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if(data == null) {
                    data = new TData() { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame(string gameName = "New Game") {
            gameData = new GameData {
                name = gameName,
                currentLevelName = "DevScene"
            };
            SceneManager.LoadScene(gameData.currentLevelName);
        }

        public void SaveGame() => dataService.Save(gameData, true);

        public void LoadGame(string gameName) {
            gameData = dataService.Load(gameName);

            if(String.IsNullOrWhiteSpace(gameData.currentLevelName)) {
                NewGame(gameName);
            }

            SceneManager.LoadScene(gameData.currentLevelName);
        }

        public void ReloadGame() => LoadGame(gameData.name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}