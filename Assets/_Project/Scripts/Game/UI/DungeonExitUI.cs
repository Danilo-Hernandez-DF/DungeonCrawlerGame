using Localisation;
using TMPro;
using UnityEngine.UI;
using Utils;

namespace Game
{
    public class DungeonExitUI: UIBase
    {
        [SerializeField] Image dungeonImage;
        [SerializeField] TextMeshProUGUI dungeonName;
        [SerializeField] Button exitButton;
        [SerializeField] Button nextLevelButton;
        
        private DungeonData nextLevel;
        private DungeonType dungeonType;
        
        private void Awake() {
            exitButton.onClick.AddListener(Exit);
            nextLevelButton.onClick.AddListener(NextLevel);
        }
        
        public void Init(DungeonData nextLevel, DungeonType dungeonType) {
            this.nextLevel = nextLevel;
            this.dungeonType = dungeonType;
        }
        
        private void Exit() {
            GameManager.Instance.LoadScene("HubScene", dungeonType, true);
        }
        
        private void NextLevel() {
            GameManager.Instance.LoadDungeonScene(nextLevel, DungeonController.Instance.modifiers);
        }
        
        public void UpdateUI() {
            if (!nextLevel) {
                dungeonImage.gameObject.SetActive(false);
                nextLevelButton.gameObject.SetActive(false);
                startElement = exitButton.gameObject;
                return;
            } 
            
            dungeonImage.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(true);
            startElement = nextLevelButton.gameObject;
            
            dungeonImage.sprite = nextLevel.dungeonType.icon;
            dungeonName.text = nextLevel.dungeonName + " " + LocalisationSystem.GetLocalisedValue("dungeon_level") +
                               ": " + GameManager.Instance.currentLevel;
        }
    }
}