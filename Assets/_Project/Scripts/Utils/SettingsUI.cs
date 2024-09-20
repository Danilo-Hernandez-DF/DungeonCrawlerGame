using Game;
using UnityEngine;

namespace UtilsModule {
    public class SettingsUI : UIBase {
        void OnPause() {
            if(!open) Open();
            else Close();
        }

        protected new void OnEnable() {
            GameManager.Instance.input.Pause += OnPause;
        }

        protected new void OnDisable() {
            GameManager.Instance.input.Pause -= OnPause;
        }
    }
}