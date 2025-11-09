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
    public List<DungeonModifier> activeModifiers;
    [SerializeField] private List<TagData> excludedTags;
    private DungeonType selectedDungeon;

    public void UpdateUI(DungeonType[] data, DungeonModifier[] modifiers) {
        this.data = data;
        this.modifiers = modifiers;
    }
    
    public List<DungeonModifier> GetModifiers(DungeonType dType) {
        var mods = activeModifiers.ToList();
        mods.AddRange(dType.modifiers);

        return mods;
    }
    
    public void UpdateDisplay(DungeonType displayData) {
        selectedDungeon = displayData;
        selectedElementText.text = displayData.name;
        UpdateEntities(displayData);
        UpdateItems();
    }

    private List<GameObject> GetDisplaySlots(Transform tab) {
        var displaySlots = new List<GameObject>();
        
        for (int i = 0; i < tab.childCount; i++) {
            var child = tab.GetChild(i).gameObject;
            if (child.CompareTag("Display")) {  
                displaySlots.Add(child);
            }
        }

        return displaySlots;
    }
    
    private void PopulateDisplaySlots(Transform tab, int count) {
        var displaySlots = GetDisplaySlots(tab);
        
        if (displaySlots.Count < count) {
            for (int i = displaySlots.Count; i < count; i++) {
                var newChild = Instantiate(displaySlotPrefab, tab);
                displaySlots.Add(newChild);
            }
        } 
        
        for (int i = 0; i < displaySlots.Count; i++) {
            if (i < count) { 
                displaySlots[i].SetActive(true);
            }
            else { 
                displaySlots[i].SetActive(false);
            }
        }
    }

    public void UpdateItems() {
        var listItems = selectedDungeon.GetItems();
        listItems = listItems.Where(item => !item.inherentTags.Exists(tag => excludedTags.Contains(tag.data))).ToList();
        listItems = listItems.Sort(ItemSortType.Rarity, false);

        var addItems = new List<ItemData>();

        foreach (var mod in activeModifiers) {
            foreach (var item in mod.GetItems()) {
                if(addItems.Contains(item)) continue;
                addItems.Add(item);
            }
        }
        
        addItems = addItems.Where(item => !listItems.Contains(item)).ToList();
        addItems = addItems.Where(item => !item.inherentTags.Exists(tag => excludedTags.Contains(tag.data))).ToList();
        addItems = addItems.Sort(ItemSortType.Rarity, false);
        
        PopulateDisplaySlots(itemTab, listItems.Count + addItems.Count);
        var displaySlots = GetDisplaySlots(itemTab);
        
        for (int i = 0; i < listItems.Count; i++) {
            displaySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = listItems[i].DisplaySprite;
            displaySlots[i].GetComponent<Image>().color = Color.white;
        }

        if (listItems.Count + addItems.Count > listItems.Count) {
            for (int i = listItems.Count; i < listItems.Count + addItems.Count; i++) {
                displaySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = addItems[i - listItems.Count].DisplaySprite;
                displaySlots[i].GetComponent<Image>().color = Color.darkSalmon;
            }
        }
    }

    private void UpdateEntities(DungeonType displayData) {
        var entities = GetEntities(displayData);

        PopulateDisplaySlots(monsterTab, entities.Count);
        var displaySlots = GetDisplaySlots(monsterTab);
        
        for (int i = 0; i < entities.Count; i++) {
            displaySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = entities[i].sprite;
        }
    }
    
    private void UpdateModifiers(DungeonType displayData) {
        var mods = displayData.modifiers;

        for (int i = 0; i < modifierTab.childCount; i++) {
            var child = modifierTab.GetChild(i).gameObject.GetComponent<ModifierButton>();
            
            if(mods.Contains(child.dungeonModifier)) {
                child.Lock();
            }
            else {
                child.Unlock();
            }
        }
    }
    
    public void PopulateModifiers() {
        if (modifierTab.childCount > 0) return;
        
        foreach(DungeonModifier modifier in modifiers) {
            ModifierButton button = Instantiate(modifierButtonPrefab, modifierTab).GetComponent<ModifierButton>();
            button.Init(modifier, this);
        }
        
        UpdateModifiers(selectedDungeon);
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
        PopulateModifiers();
    }
    
    protected override void OnClose() {
        for (int i = 0; i < modifierTab.childCount; i++) {
            modifierTab.GetChild(i).GetComponent<ModifierButton>().OnDeselect(null);
        }
    }

    public void SelectModifier(bool selected, DungeonModifier modifier) {
        if(selected) {
            if(activeModifiers.Contains(modifier)) return;
            activeModifiers.Add(modifier);
        } else {
            if(!activeModifiers.Contains(modifier)) return;
            activeModifiers.Remove(modifier);
        }
    }

    private IEnumerator SelectDungeon() {
        CleanButtons();
        
        foreach(DungeonType diaplayData in data) {
            DungeonSelectionButton button = Instantiate(dungeonButtonPrefab, buttonHolder.transform).GetComponent<DungeonSelectionButton>();
            button.Init(diaplayData, this);
        }
        
        selectedDungeon = data[0];

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