using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UtilsModule {
    public class ItemDisplay : MonoBehaviour {
        [SerializeField] Image sprite;
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI tags;
        [SerializeField] private GameObject panel;
        private RectTransform panelT;

        private void Awake() {
            panelT = panel.GetComponent<RectTransform>();
        }

        public void UpdateDisplay(Item item) {
            if(item?.data == null || item.IsEmpty) {
                sprite.sprite = null;
                sprite.color = Color.clear;
                itemName.text = "";
                tags.text = "";
                panel.SetActive(false);

                return;
            }

            sprite.sprite = item.data.DisplaySprite;
            sprite.color = Color.white;
            itemName.text = item.data.name;
            panel.SetActive(true);

            string tagString = "<size=60%><color=#AAAAAA>-----Tags-----\n\n</size></color>";
            string inherentTags = "<size=60%><color=#AAAAAA>-----Inherent-----\n\n</size></color>";
            int inherent = 0;
            int regular = 0;
            foreach(var tag in item.tags) {
                if(tag.data.hidden) continue;
                
                if(tag.inherent) {
                    inherentTags += tag + "\n";
                    inherent++;
                } else {
                    tagString += tag + "\n";
                    regular++;
                }
            }

            tags.text = "";
            float panelH = 0;
            
            if(regular > 0) {
                tags.text += tagString + "\n";
                panelH += 61.2f;
            }
            
            if(inherent > 0) {
                tags.text += inherentTags + "\n";
                panelH += 61.2f;
            }
            
            panelH += 27.8f * (regular + inherent);
            
            panelT.sizeDelta = new Vector2(panelT.sizeDelta.x, panelH + 112);
        }
    }
}