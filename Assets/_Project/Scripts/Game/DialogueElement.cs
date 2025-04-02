using System;

namespace Game {
    [Serializable]
    public class DialogueElement {
        public string textContent;
        public Quest questGiven;
        public DialogueElement[] subDialogue;
    }
}