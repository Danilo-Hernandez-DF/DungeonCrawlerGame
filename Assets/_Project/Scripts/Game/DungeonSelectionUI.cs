using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;

public class DungeonSelectionUI : UIBase
{
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] TextMeshProUGUI selectedElementText;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject dungeonButtonPrefab;
    [SerializeField] private GameObject modifierButtonPrefab;
    [SerializeField] private GameObject displaySlotPrefab;
    
    [Header("Tabs")]
    [SerializeField] private Transform monsterTab;
    [SerializeField] private Transform itemTab;
    [SerializeField] private Transform modifierTab;
    
    private DungeonType[] data;
    private DungeonModifier[] modifiers;

    public void UpdateUI(DungeonType[] data, DungeonModifier[] modifiers) {
        this.data = data;
        this.modifiers = modifiers;
    }
    
    public void UpdateDisplay(DungeonType diaplayData) {
        selectedElementText.text = diaplayData.name;
        var entities = GetEntities(diaplayData);

        if (monsterTab.childCount < entities.Count) {
            for (int i = monsterTab.childCount; i < entities.Count; i++) {
                Instantiate(displaySlotPrefab, monsterTab);
            }
        } 
        
        for (int i = 0; i < monsterTab.childCount; i++) {
            if (i < entities.Count) { 
                monsterTab.GetChild(i).gameObject.SetActive(true);
            }
            else { 
                monsterTab.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < entities.Count; i++) {
            monsterTab.GetChild(i).GetChild(0).GetComponent<Image>().sprite = entities[i].sprite;
        }
    }
    
    private List<EntityData> GetEntities(DungeonType diaplayData) {
        var eligible = diaplayData.spawnableEntities.Where(entity => entity.HasTag(GameManager.spawnerTag.nameKey)).ToList();
        List<EntityData> entities = diaplayData.spawnableEntities.Where(entity => entity.HasTag(GameManager.enemyTag.nameKey)).ToList();
        
        foreach (var entity in eligible) {
            var spawnables = entity.prefab.GetComponent<EnemySpawnManager>().enemyData
                .Where(spawnable => spawnable.HasTag(GameManager.enemyTag.nameKey)).ToList();

            foreach (var spwn in spawnables) {
                if(entities.Contains(spwn)) continue; 
                entities.Add(spwn);
            }
        }
        
        return entities;
    }
    
    protected override void OnOpen() {
        StartCoroutine(SelectDungeon());
    }

    private IEnumerator SelectDungeon() {
        CleanButtons();
        
        foreach(DungeonType diaplayData in data) {
            DungeonSelectionButton button = Instantiate(dungeonButtonPrefab, buttonHolder.transform).GetComponent<DungeonSelectionButton>();
            button.Init(diaplayData, this);
        }

        yield return null;
        
        GameManager.Instance.eventSystem.SetSelectedGameObject(buttonHolder.transform.GetChild(0).gameObject);
    }
    
    /*private void SelectModifiers() {
        CleanButtons();
        
        foreach(DungeonModifier modifier in modifiers) {
            GameObject button = Instantiate(modifierButtonPrefab, buttonHolder.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = modifier.name;
            button.GetComponent<Button>().onClick.AddListener();
        }
    } */
    
    private void CleanButtons() {
        for (int i = buttonHolder.transform.childCount; i > 0; i--) {
            Destroy(buttonHolder.transform.GetChild(i-1).gameObject);
        }
    }
}