using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UtilsModule {
    public class ItemDisplay : MonoBehaviour {
        [SerializeField] Image sprite;
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] TextMeshProUGUI tags;
        [SerializeField] Color inherentTagColor;
        [SerializeField] Color defaultTagColor;

        public void UpdateDisplay(Item item) {
            if(item?.data == null || item.IsEmpty) {
                sprite.sprite = null;
                sprite.color = Color.clear;
                itemName.text = "---";
                description.text = "---\n---\n---\n---";
                tags.text = "<color=#5558><size=60%>-----Tags-----</size></color><size=30%>\n\n</size>---";

                return;
            }

            sprite.sprite = item.data.DisplaySprite;
            sprite.color = Color.white;
            itemName.text = item.data.name;
            description.text = item.data.description;

            string tagString = $"<color=#{ColorUtility.ToHtmlStringRGB(defaultTagColor)}>";
            string inherentTags = $"<color=#{ColorUtility.ToHtmlStringRGB(inherentTagColor)}>";
            foreach(var tag in item.tags) {
                if(tag.inherent) inherentTags += tag.ToString() + "\n";
                else tagString += tag.ToString() + "\n";
            }

            tagString += "</color>"; 
            inherentTags += "</color>";

            tags.text = "<color=#5558><size=60%>-----Tags-----</size></color><size=30%>\n\n</size>" +
                tagString + "<size=30%>\n<color=#5558><size=60%>-----Inherent-----</size></color>\n\n</size>" + inherentTags;
        }
    }
}