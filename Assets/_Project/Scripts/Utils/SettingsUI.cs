using Game;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Localisation.LocalisationSystem;

namespace UtilsModule {
    public class SettingsUI : UIBase {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button LanguageButton;
        [SerializeField] private Button spanishButton;
        [SerializeField] private Button englishButton;
        [SerializeField] GameObject LanguagePanel;

        new void Awake()
        {
            base.Awake();
            mainMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenuScene");
                SaveLoadSystem.Instance.DeleteTempSaves();
            });
            saveButton.onClick.AddListener(() => SaveLoadSystem.Instance.SaveGame(false));
            spanishButton.onClick.AddListener(() =>
            {
                SetLanguage(LocalisedLanguage.Spanish);
                LanguagePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);
            });
            englishButton.onClick.AddListener(() =>
            {
                SetLanguage(LocalisedLanguage.English);
                LanguagePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);
            });
            LanguageButton.onClick.AddListener(() => LanguagePanel.SetActive(true));
        }
        
        void OnPause() {
            if(!open) Open();
            else Close();
        }
        
        protected override void OnClose() {
            LanguagePanel.SetActive(false);
        }
        
        protected new void OnEnable()
        {
            GameManager.Instance.input.Pause += OnPause;
        }

        protected new void OnDisable() {
            GameManager.Instance.input.Pause -= OnPause;
        }
    }
}