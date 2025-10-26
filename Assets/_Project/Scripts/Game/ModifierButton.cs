using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class ModifierButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private static readonly int Pressed = Animator.StringToHash("Pressed");
        private static readonly int Selected = Animator.StringToHash("Selected");
        
        public DungeonModifier dungeonModifier { get; private set; }
        DungeonSelectionUI ui;
        private Button button;
        private bool selected = false;
        private bool locked = false;
        Image bg;
        
        public void Lock() {
            locked = true;
            bg.color = Color.darkSalmon;
        }
        
        public void Unlock() {
            locked = false;
            bg.color = Color.white;
        }
        
        public void Init(DungeonModifier modifier, DungeonSelectionUI ui) {
            bg = GetComponent<Image>();
            dungeonModifier = modifier;
            this.ui = ui;
            
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
            transform.GetChild(0).GetComponent<Image>().sprite = dungeonModifier.icon;
        }
        
        private void OnClick() {
            if(locked) return;
            
            GetComponent<Animator>().SetTrigger(Pressed);
            selected = !selected;
            ui.SelectModifier(selected, dungeonModifier);
            
            if(!selected) bg.color = Color.white;
            else bg.color = Color.lightBlue;
        }
        
        public void OnSelect(BaseEventData eventData) {
            GetComponent<Animator>().SetBool(Selected, true);
        }
    
        public void OnDeselect(BaseEventData eventData) {
            GetComponent<Animator>().SetBool(Selected, false);
        }
    }
}