using Game;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UtilsModule {
    public class SettingsUI : UIBase {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button saveButton;

        new void Awake() {
            base.Awake();
            mainMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenuScene");
                SaveLoadSystem.Instance.DeleteTempSaves();
            });
            saveButton.onClick.AddListener(() =>SaveLoadSystem.Instance.SaveGame(false));
        }
        
        void OnPause() {
            if(!open) Open();
            else Close();
        }
        
        protected new void OnEnable() {
            GameManager.Instance.input.Pause += OnPause;
        }

        protected new void OnDisable() {
            GameManager.Instance.input.Pause -= OnPause;
        }
    }
}