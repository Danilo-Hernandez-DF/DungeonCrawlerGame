using System;
using System.Collections.Generic;
using Systems.Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Localisation.LocalisationSystem;

namespace Game {
    public class MainMenu : MonoBehaviour {
        [SerializeField] private Button loadButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;
        
        [SerializeField] private GameObject loadMenu;
        [SerializeField] private Button quitLoadMenuButton;
        [SerializeField] private Button buttonPrefab;
        private List<Button> loadButtons;
        
        [SerializeField] private GameObject optionsMenu;
        [SerializeField] private Button quitOptionsMenuButton;

        [SerializeField] private GameObject languagePanel;
        [SerializeField] private Button languageButton;
        [SerializeField] private Button spanishButton;
        [SerializeField] private Button englishButton;

        private int savedGames;

        private void Start() {
            List<GameData> games = SaveLoadSystem.Instance.GetSavedGames();
            savedGames = games.Count;
            loadButtons = new List<Button>();
            
            for (int i = 0; i < games.Count; i++) {
                var button = Instantiate(buttonPrefab, loadMenu.transform.GetChild(0), true);
                string path = games[i].name;
                button.GetComponentInChildren<TMP_Text>().text = path;
                button.onClick.AddListener(() => LoadGame(path));
                loadButtons.Add(button);
            }

            languageButton.onClick.AddListener(() => languagePanel.SetActive(true));
            spanishButton.onClick.AddListener(() =>
            {
                SetLanguage(LocalisedLanguage.Spanish);
                languagePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(quitOptionsMenuButton.gameObject);
            });
            englishButton.onClick.AddListener(() =>
            {
                SetLanguage(LocalisedLanguage.English);
                languagePanel.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(quitOptionsMenuButton.gameObject);
            }); 
            
            loadButton.onClick.AddListener(() => {
                loadMenu.SetActive(true);
                GameManager.Instance.eventSystem.SetSelectedGameObject(quitLoadMenuButton.gameObject);
                ToggleButtons(false);
            });
            newGameButton.onClick.AddListener(NewGame);
            optionsButton.onClick.AddListener(() => {
                optionsMenu.SetActive(true);
                GameManager.Instance.eventSystem.SetSelectedGameObject(quitOptionsMenuButton.gameObject);
                ToggleButtons(false);
            });
            quitButton.onClick.AddListener(Quit);
            
            quitLoadMenuButton.onClick.AddListener(ReturnToMenu);
            quitOptionsMenuButton.onClick.AddListener(ReturnToMenu);
            
            ToggleButtons(true);
            GameManager.Instance.eventSystem.SetSelectedGameObject(
                loadButton.gameObject.activeInHierarchy? loadButton.gameObject : newGameButton.gameObject);
        }
        
        private void ToggleButtons(bool active) {
            loadButton.gameObject.SetActive(savedGames != 0 && active);
            newGameButton.gameObject.SetActive(active);
            optionsButton.gameObject.SetActive(active);
            quitButton.gameObject.SetActive(active);
        }
        private void LoadGame(string path) {
            GameManager.Instance.eventSystem.SetSelectedGameObject(quitLoadMenuButton.gameObject);
            //GameManager.Instance.Invoke(nameof(GameManager.Instance.FreshGame), 1f);
            SaveLoadSystem.Instance.LoadGame(path);
        }
        
        private void ReturnToMenu() {
            loadMenu.SetActive(false);
            optionsMenu.SetActive(false);
            languagePanel.SetActive(false);
            ToggleButtons(true);
            GameManager.Instance.eventSystem.SetSelectedGameObject(
                loadButton.gameObject.activeInHierarchy? loadButton.gameObject : newGameButton.gameObject);
        }
        
        private void NewGame() {
            //GameManager.Instance.Invoke(nameof(GameManager.Instance.FreshGame), 1f);
            SaveLoadSystem.Instance.NewGame();
        }
        
        private void Quit() {
            Application.Quit();
        }
    }
}