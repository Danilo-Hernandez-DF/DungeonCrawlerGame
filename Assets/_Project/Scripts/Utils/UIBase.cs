using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class UIBase : MonoBehaviour {
        [SerializeField] protected GameObject handledUI;
        [SerializeField] protected GameObject startElement;
        [SerializeField] protected List<UIBase> children = new List<UIBase>();
        protected GameObject StartElement => !IsParent? startElement: children[0].startElement;
        protected bool open => handledUI? handledUI.activeSelf: Parent.open;
        private UIBase Parent;
        private bool IsParent => children.Count > 0;
        private bool IsChild => Parent != null;

        protected virtual void Awake() {
            foreach(UIBase child in children) {
                child.Parent = this;
            }
        }

        public void SetParent(UIBase parent) {
            this.Parent = parent;
        }

        public void Open() {
            if(!IsChild) {
                if(GameManager.Instance.openUI != null) return;
                handledUI.SetActive(true);
                GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);
            }

            OnOpen();
            
            if(IsParent) foreach(UIBase child in children) {
                child.Open();
            }

            if(!IsChild) GameManager.Instance.openUI = handledUI;
        }

        protected virtual void OnOpen() {
            //Implement in children
        }

        protected void Close() {
            if(!IsChild) {
                if(GameManager.Instance.openUI != handledUI) return;
                handledUI.SetActive(false);
                GameManager.Instance.eventSystem.SetSelectedGameObject(null);
            }
            
            OnClose();
            
            if(IsParent) foreach(UIBase child in children) {
                child.Open();
            }

            if(!IsChild) GameManager.Instance.openUI = null;
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