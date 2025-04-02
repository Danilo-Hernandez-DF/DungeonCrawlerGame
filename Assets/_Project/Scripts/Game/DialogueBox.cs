using System.Collections;
using _Project.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Game {
    public class DialogueBox : MonoBehaviour {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI textBox;
        [SerializeField] private DialogueElement[] dialogue;
        private int currentIndex;
        private bool playingText;
        public bool Open { get; private set; }

        public void Init(DialogueElement[] newDialogue) {
            dialogue = newDialogue;
            
        }

        private IEnumerator PlayText(int index, NPC questGiver) {
            playingText = true;
            for(int i = 0; i < dialogue[index].textContent.Length; i++) {
                if(!playingText) {
                    textBox.text = dialogue[index].textContent;
                    break;
                }
                textBox.text = dialogue[index].textContent[..(i + 1)];
                yield return new WaitForSeconds(0.1f);
            }

            playingText = false;
            if(dialogue[index].questGiven) GameSettings.Instance.AddQuest(dialogue[index].questGiven, questGiver);
        }

        public void Interacted(NPC questGiver) {
            if(!Open) {
                Open = true;
                currentIndex = 0;
                panel.SetActive(true);
                GameManager.Instance.openUI = panel;
                StartCoroutine(PlayText(currentIndex, questGiver));
                return;
            }
            
            if(playingText) {
                playingText = false;
            } else {
                currentIndex++;
                if (currentIndex >= dialogue.Length) {
                    Open = false;
                    panel.SetActive(false);
                    GameManager.Instance.openUI = null;
                    return;
                }
                StartCoroutine(PlayText(currentIndex, questGiver));
            }
        }
    }
}