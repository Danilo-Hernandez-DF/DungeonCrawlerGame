using Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilsModule {
    [RequireComponent(typeof(Button))]
    public class InventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler {
        public static Item heldItem;

        [Header("Visuals")]
        [SerializeField] Image image;
        [SerializeField] Image textBg;
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] GameObject heldItemDisplay;
        [SerializeField] Image heldItemImage;
        [SerializeField] TextMeshProUGUI heldItemText;

        [Header("Events")]
        [SerializeField] ItemEventChannel itemDisplayChannel;
        [SerializeField] IntEventChannel slotIndexChannel;

        [Header("Data")]
        [SerializeField] int index;
        int indexOffset = 0;
        public int offsetIndex => index + indexOffset;
        private Item currentItem;
        bool selected;

        public void OnPress() {
            slotIndexChannel.Invoke(index);
        }

        public void Init(int index, IntEventChannel slotIndexChannel) {
            this.slotIndexChannel = slotIndexChannel;
            this.index = index;
        }

        public void Refresh(int offset) {
            indexOffset = offset;
        }

        public void Clear() {
            selected = false;
            DeactivateHeldItem();
        }

        public void OnUpdate(Inventory inventory) {
            currentItem = inventory.items[offsetIndex];
            if(currentItem.IsEmpty) {
                image.sprite = null;
                image.color = Color.clear;
                textBg.color = Color.clear;
                amountText.text = "";
            } else {
                image.sprite = currentItem.data.DisplaySprite;
                image.color = Color.white;
                textBg.color = Color.white;
                amountText.text = currentItem.count.ToString();
            }

            UpdateData();
        }

        void Awake() {
            GetComponent<Button>().onClick.AddListener(OnPress);
        }

        public void OnSelect(BaseEventData eventData) {
            selected = true;
            UpdateData();
        }

        void UpdateData() {
            if(selected) itemDisplayChannel?.Invoke(currentItem);
            UpdateHeldItemDisplay();
        }

        void UpdateHeldItemDisplay() {
            if(heldItem?.IsEmpty ?? true || !selected) {
                DeactivateHeldItem();
            } else if(selected) {
                heldItemDisplay.SetActive(true);
                heldItemImage.sprite = heldItem.data.DisplaySprite;
                heldItemImage.color = Color.white;
                heldItemText.text = heldItem.count.ToString();
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            selected = false;
            DeactivateHeldItem();
        }

        void DeactivateHeldItem() {
            heldItemDisplay.SetActive(false);
            heldItemImage.sprite = null;
            heldItemImage.color = Color.clear;
            heldItemText.text = "";
        }
    }
}