using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using static Localisation.LocalisationSystem;

namespace Game {
    public class SettingsUI : UIBase {
        [Header("Pause Menu")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button settingsButton;

        [Header("Settings")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button LanguageButton;
        [SerializeField] private Button spanishButton;
        [SerializeField] private Button englishButton;
        [SerializeField] GameObject LanguagePanel;

        new void Awake()
        {
            base.Awake();

            settingsButton.onClick.AddListener(() =>
            {
                settingsPanel.SetActive(true);
                pausePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(LanguageButton.gameObject);
            });

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
                GameManager.Instance.eventSystem.SetSelectedGameObject(LanguageButton.gameObject);
            });

            englishButton.onClick.AddListener(() =>
            {
                SetLanguage(LocalisedLanguage.English);
                LanguagePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(LanguageButton.gameObject);
            });

            LanguageButton.onClick.AddListener(() => LanguagePanel.SetActive(true));
        }

        void OnPause()
        {
            if (!open)
            {
                Open();
                return;
            }

            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                pausePanel.SetActive(true);
                LanguagePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(startElement);
            }
            else
            {
                Close();
            }
        }
        
        protected override void OnOpen()
        {
            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);
            LanguagePanel.SetActive(false);
        }
        
        protected override void OnClose()
        {
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