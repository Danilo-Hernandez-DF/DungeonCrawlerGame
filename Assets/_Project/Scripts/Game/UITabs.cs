using System.Collections.Generic;
using Localisation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class UITabs : MonoBehaviour
    {
        [SerializeField] private List<Button> tabs;
        [SerializeField] private List<GameObject> panels;
        [SerializeField] private GameObject tabBtn;
        [SerializeField] private string[] tabNames;
        
        void Start() {
            for(int i = 0; i < Mathf.Min(tabNames.Length, transform.GetChild(1).childCount); i++) {
                var btn = Instantiate(tabBtn, transform.GetChild(0));
                btn.GetComponentInChildren<TextLocaliserUI>().Key = tabNames[i];
                tabs.Add(btn.GetComponent<Button>());
                panels.Add(transform.GetChild(1).GetChild(i).gameObject);

                var index = i;
                tabs[i].onClick.AddListener(() => SelectPanel(index));
            }

            for(int i = 0; i < transform.GetChild(1).childCount; i++) {
                transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
            
            SelectPanel(0);
        }

        private void SelectPanel(int index) {
            for(int i = 0; i < panels.Count; i++) {
                if (i != index) {
                    panels[i].SetActive(false);
                    tabs[i].GetComponent<Image>().color = Color.white;
                }
                else {
                    panels[i].SetActive(true);
                    tabs[i].GetComponent<Image>().color = Color.lightBlue;
                }
            }
        }
    }
}