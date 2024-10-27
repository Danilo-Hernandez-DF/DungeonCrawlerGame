using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilsModule;

namespace Game {
    public class GameManager : Singleton<GameManager> {
        //------------------------------------------------
        //Variables
        //------------------------------------------------

        public ItemData EmptyItem;
        public EventSystem eventSystem;
        public GameObject openUI;
        public InputReader input;
        public bool Paused => openUI != null;

        //------------------------------------------------
        //Methods
        //------------------------------------------------

        protected override void Awake() {
            base.Awake();
        }

        //------------------------------------------------
    }

    public enum OperatorType {Add, Multiply}
}