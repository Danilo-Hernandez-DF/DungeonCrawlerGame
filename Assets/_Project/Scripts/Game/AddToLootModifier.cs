using System;
using Localisation;
using UnityEngine;
using UtilsModule;

namespace Game
{
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/Add To Loot")]
    public class AddToLootModifier : DungeonModifier
    {
        [SerializeField] private int rarity;
        [SerializeField] private ItemData data;
        [SerializeField] private TagData tag;
        [SerializeField] private int rolls;
        [SerializeField] private Color specialTextColor;
        [SerializeField] private string rarityKey; // Items with rarity // Objetos con rareza
        [SerializeField] private string dataKey; // Items // Objetos
        [SerializeField] private string tagKey; // Items containing // Objetos conteniendo
        [SerializeField] private string higherThanKey; // Higher than // Mayor que
        
        public override void OnLootGenerated(Lootable loot) {
            var table = loot.inventoryGenrator.lootTable;
            Item item = null;

            if (data) {
                item = table.GetBiasedItem(data, rolls);
            } else if (tag) {
                item = table.GetBiasedItem(tag, rolls);
            } else {
                item = table.GetBiasedItem(rarity, rolls);
            }
            
            loot.Inventory.SetAtRandom(item); 
        }

        public override string Description() {
            var description = LocalisationSystem.GetLocalisedValue(descriptionKey);
            string hexColor = ColorUtility.ToHtmlStringRGBA(specialTextColor);
            
            string itemName = "";
            if (data) {
                itemName = LocalisationSystem.GetLocalisedValue(dataKey) + " " + $"<color=#{hexColor}>{data.name}</color>";
            } else if (tag) {
                itemName = LocalisationSystem.GetLocalisedValue(tagKey) + " " + $"<color=#{hexColor}>{tag.name}</color>";
            } else {
                itemName = LocalisationSystem.GetLocalisedValue(rarityKey) + " " +
                           LocalisationSystem.GetLocalisedValue(higherThanKey) + " " + $"<color=#{hexColor}>{rarity}</color>";
            }
            
            description = description.Replace("{0}", itemName);
            
            //{0} Has a higher chance to be found.
            //{0} Tiene una mayor posibilidad de ser encontrado.
            
            return description;
        }
    }
}