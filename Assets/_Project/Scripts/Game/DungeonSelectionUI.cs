using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;

public class DungeonSelectionUI : UIBase {
    [SerializeField] private Button[] objectiveButtons;
    [SerializeField] private DungeonDataEventChannel startChannel;
    private List<TextMeshProUGUI> objectiveText;
    private DungeonData[] dungeons;
    private List<Image> objectiveImages;
    private bool isEntrance = false;

    new void Awake() {
        base.Awake();
        objectiveText = new List<TextMeshProUGUI>();
        objectiveImages = new List<Image>();

        foreach(var button in objectiveButtons) {
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if(text != null) {
                objectiveText.Add(text);
            }

            var image = button.transform.GetChild(1).GetComponent<Image>();
            if(image != null) {
                objectiveImages.Add(image);
            }
        }
    }

    public void RecieveEvent(bool isEntrance, DungeonData[] dungeons) {
        this.dungeons = dungeons;
        this.isEntrance = isEntrance;
        Open();
    }

    protected override void OnOpen() {
        if(!isEntrance) {
            foreach(var button in objectiveButtons) {
                button.gameObject.SetActive(true);
            }

            for(int i = 0; i < objectiveButtons.Length; i++) {
                objectiveButtons[i].onClick.RemoveAllListeners();
                int index = i;
                objectiveButtons[i].onClick.AddListener(() => startChannel.Invoke(dungeons[index]));
                objectiveText[i].text = dungeons[i].dungeonName;
                objectiveImages[i].sprite = dungeons[i].dungeonType.sprite;
            }
        } else {
            foreach(var button in objectiveButtons) {
                button.gameObject.SetActive(false);
            }

            objectiveButtons[0].gameObject.SetActive(true);
            objectiveButtons[0].onClick.RemoveAllListeners();    
            objectiveButtons[0].onClick.AddListener(() => startChannel.Invoke(dungeons[0]));
            objectiveText[0].text = dungeons[0].dungeonName;
            objectiveImages[0].sprite = dungeons[0].dungeonType.sprite;
        }
    }
}
