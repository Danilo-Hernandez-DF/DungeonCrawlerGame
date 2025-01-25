using Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilsModule {
    public class InventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler {
        public static Item heldItem;
        private static readonly int Pressed = Animator.StringToHash("Pressed");
        private static readonly int Selected = Animator.StringToHash("Selected");
        public static bool HeldItemEmpty => heldItem?.IsEmpty ?? true;

        [Header("Visuals")]
        [SerializeField] Image image;
        [SerializeField] Image textBg;
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] GameObject heldItemDisplay;
        [SerializeField] Image heldItemImage;
        [SerializeField] TextMeshProUGUI heldItemText;

        [Header("Events")]
        [SerializeField] ItemEventChannel itemDisplayChannel;

        [Header("Data")]
        [SerializeField] int index;
        Animator anim;
        int indexOffset = 0;
        private int OffsetIndex => index + indexOffset;
        private Item currentItem;
        public bool IsSelected { get; private set; }

        InventoryUI parent;

        private void OnPress(int action = 0) {
            if(!IsSelected) return;
            anim.SetTrigger(Pressed);
            parent.OnSlotPressed(OffsetIndex, action);
        }

        void Update() {
            anim.SetBool(Selected, IsSelected);
        }

        public void Init(int index) {
            this.index = index;
        }

        public void Refresh(int offset) {
            indexOffset = offset;
        }

        public void Clear() {
            IsSelected = false;
            DeactivateHeldItem();
        }

        public void OnUpdate(InventoryUI inventoryUI) {
            parent = inventoryUI;
            currentItem = inventoryUI.TargetInventory.items[OffsetIndex];
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
            GameManager.Instance.input.UIInteract += OnPress;
            anim = GetComponent<Animator>();
        }

        public void OnSelect(BaseEventData eventData) {
            IsSelected = true;
            UpdateData();
        }

        void UpdateData() {
            if(IsSelected && currentItem != null) itemDisplayChannel?.Invoke(currentItem);
            UpdateHeldItemDisplay();
        }

        void UpdateHeldItemDisplay() {
            if((heldItem?.IsEmpty ?? true) || !IsSelected) {
                DeactivateHeldItem();
            } else if(IsSelected) {
                heldItemDisplay.SetActive(true);
                heldItemImage.sprite = heldItem.data.DisplaySprite;
                heldItemImage.color = Color.white;
                heldItemText.text = heldItem.count.ToString();
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            IsSelected = false;
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