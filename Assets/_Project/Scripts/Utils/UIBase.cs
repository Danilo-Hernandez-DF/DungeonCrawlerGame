using Game;
using UnityEngine;
using UnityEngine.Events;

namespace UtilsModule {
    public class UIBase : MonoBehaviour {
        [SerializeField] protected GameObject handledUI;
        [SerializeField] protected GameObject startUI;
        public UnityAction OpenChildrenEvent = delegate { };
        public UnityAction CloseChildrenEvent = delegate { };
        public GameObject StartUI => startUI;

        protected bool open = false;

        public virtual void Open() {
            if(GameManager.Instance.openUI != null) return;
            handledUI.SetActive(true);
            GameManager.Instance.eventSystem.SetSelectedGameObject(startUI);
            GameManager.Instance.openUI = handledUI;
            open = true;
        }

        public virtual void Close() {
            if(GameManager.Instance.openUI != handledUI) return;
            handledUI.SetActive(false);
            GameManager.Instance.eventSystem.SetSelectedGameObject(null);
            GameManager.Instance.openUI = null;
            open = false;
        }

        protected void OnEnable() {
            GameManager.Instance.input.Pause += Close;
        }

        protected void OnDisable() {
            GameManager.Instance.input.Pause -= Close;
        }
    }
}