using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DungeonSelectionButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private static readonly int Pressed = Animator.StringToHash("Pressed");
    private static readonly int Selected = Animator.StringToHash("Selected");
    
    public DungeonSelectionUI ui;
    public DungeonType data;
    public Button button;
    public Image icon;

    public void Init(DungeonType data, DungeonSelectionUI ui) {
        this.data = data;
        this.ui = ui;
        
        button = GetComponent<Button>();
        icon = GetComponent<Image>();
        icon.sprite = data.icon;
        
        button.GetComponent<Button>().onClick.AddListener((() => {
            GetComponent<Animator>().SetTrigger(Pressed);
            GameManager.Instance.LoadScene(data.sceneName, data, true, ui.GetModifiers(data));
        }));
    }
    
    public void OnSelect(BaseEventData eventData) {
        GetComponent<Animator>().SetBool(Selected, true);
        ui.UpdateDisplay(data);
    }
    
    public void OnDeselect(BaseEventData eventData) {
        GetComponent<Animator>().SetBool(Selected, false);
    }
}