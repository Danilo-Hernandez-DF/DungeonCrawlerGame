using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class UIBase : MonoBehaviour {
        [SerializeField] protected GameObject handledUI;
        [SerializeField] protected GameObject startElement;
        [SerializeField] protected List<UIBase> children = new List<UIBase>();
        protected GameObject StartElement => !isParent? startElement: children[0].startElement;
        protected bool open => handledUI? handledUI.activeSelf: parent.open;
        protected UIBase parent;
        protected bool isParent => children.Count > 0;
        protected bool isChild => parent != null;

        protected virtual void Awake() {
            foreach(UIBase child in children) {
                child.parent = this;
            }
        }

        public void SetParent(UIBase parent) {
            this.parent = parent;
        }

        public void Open() {
            if(!isChild) {
                if(GameManager.Instance.openUI != null) return;
                handledUI.SetActive(true);
                GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);
            }

            OnOpen();
            
            if(isParent) foreach(UIBase child in children) {
                child.Open();
            }

            if(!isChild) GameManager.Instance.openUI = handledUI;
        }

        protected virtual void OnOpen() {
            //Implement in children
        }

        public void Close() {
            if(!isChild) {
                if(GameManager.Instance.openUI != handledUI) return;
                handledUI.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(null);
            }
            
            OnClose();
            
            if(isParent) foreach(UIBase child in children) {
                child.Open();
            }

            if(!isChild) GameManager.Instance.openUI = null;
        }

        protected virtual void OnClose() {
            //Implement in children
        }

        protected void OnEnable() {
            GameManager.Instance.input.Pause += Close;
        }

        protected void OnDisable() {
            GameManager.Instance.input.Pause -= Close;
        }
    }
}