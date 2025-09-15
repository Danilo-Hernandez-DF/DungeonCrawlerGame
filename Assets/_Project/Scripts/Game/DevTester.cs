using UnityEngine;
using UtilsModule;
using UnityEngine.UI;
using System;
using TMPro;

namespace Game
{
    public class DevTester : MonoBehaviour, IInteractable, IEffector
    {
        [SerializeField] UIBase panel;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Button confirmButton;
        [SerializeField] Transform spawnPoint;
        [SerializeField] InventoryHolder inventoryHolder;

        GameObject spawnedObject;

        void Start()
        {
            confirmButton.onClick.AddListener(ConfirmInput);
        }

        void ConfirmInput()
        {
            string input = inputField.text;
            string[] args = input.Split(' ');

            if (args.Length < 2) return;

            switch (args[0])
            {
                case "spawn":
                    string entityName = args[1];
                    GameObject prefab = GameManager.GetEntity(entityName)?.prefab;

                    if (prefab != null) SpawnObject(prefab);
                    break;
                case "status":
                    string effectName = args[1];
                    StatusEffectData effect = GameManager.GetStatusEffect(effectName);

                    if (effect != null)
                    {
                        if (spawnedObject == null)
                        {
                            ApplyStatus(PlayerDetector.GetPlayer().GetComponent<Entity>(), effect.GetStatusEffect());
                        }
                        else
                        {
                            Entity entity = spawnedObject.GetComponent<Entity>();
                            ApplyStatus(entity, effect.GetStatusEffect());
                        }
                    }
                    break;
                case "item":
                    if (args.Length < 2) return;
                    int count = 1;
                    string itemName = args[1];
                    ItemData itemData = GameManager.GetItem(itemName);

                    if (args.Length > 2)
                    {
                        if (args[2] == "max") count = itemData.maxCount;
                        else int.TryParse(args[2], out count);
                    }

                    Item item = itemData.GetItem();

                    inventoryHolder.Inventory.TryAdd(item, count);
                    break;
                case "clone":
                    int slot = 0;
                    int cloneCount = 1;

                    if (args.Length > 1) int.TryParse(args[1], out slot);
                    if (args.Length > 2) int.TryParse(args[2], out cloneCount);

                    Item cloned = inventoryHolder.Inventory.GetItem(slot);

                    inventoryHolder.Inventory.TryAdd(cloned, cloneCount);
                    break;
                case "clear":
                    inventoryHolder.Inventory.Clear();
                    break;
                case "kill":
                    Destroy(spawnedObject);
                    spawnedObject = null;
                    break;
                case "loot":
                    if (args.Length < 2) return;
                    int rolls = 1;
                    if (args.Length > 2) int.TryParse(args[2], out rolls);
                    ItemLootTable lootTable = GameManager.GetItemLootTable(args[1]);

                    inventoryHolder.Generate(lootTable, rolls);
                    break;
                case "effect":
                    //stat modifier effects
                    break;
                case "edit":
                    //edit item with tags
                    break;
                case "room":
                    //generate room
                    break;
            }


            panel.Close();
        }

        void SpawnObject(GameObject prefab)
        {
            if (spawnedObject != null)
            {
                if (spawnedObject.GetComponent<Entity>()) spawnedObject.GetComponent<Entity>().TakeDamage(int.MaxValue);
                else
                {
                    Destroy(spawnedObject);
                    spawnedObject = null;
                }
            }
            spawnedObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }

        public void OnInteract()
        {
            if(GameManager.Instance.Paused) return;
            if(!InRange()) return;
            panel.Open();
        }

        bool InRange() => IInteractable.InRange(transform.position);

        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect)
        {
            var modifier = modifierEffect.operatorType switch
            {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, modifierEffect.duration),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, modifierEffect.duration, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier);
        }

        public void ApplyStatus(Entity entity, StatusEffect statusEffect)
        {
            entity.Status.Add(statusEffect);
        }

        public void Visit<T>(T visitable) where T : Component, IVisitable
        {
            return;
        }
        
        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
    }
}