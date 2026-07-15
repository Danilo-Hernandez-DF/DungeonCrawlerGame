using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class TabButton: MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private static readonly int Pressed = Animator.StringToHash("Pressed");
        private static readonly int Selected = Animator.StringToHash("Selected");
        
        private Animator anim;

        private void Awake() {
            anim = GetComponent<Animator>();
            GetComponent<Button>().onClick.AddListener(() => {
                anim.SetTrigger(Pressed);
            });
        }

        public void OnSelect(BaseEventData eventData) {
            anim.SetBool(Selected, true);
        }
        
        public void OnDeselect(BaseEventData eventData) {
            anim.SetBool(Selected, false);
        }
    }
}