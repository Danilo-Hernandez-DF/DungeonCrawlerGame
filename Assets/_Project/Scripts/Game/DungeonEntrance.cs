using System;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using Game;
using Systems.Persistence;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsModule;

public class DungeonEntrance : MonoBehaviour, IInteractable {
    [SerializeField] private DungeonType[] potentialDungeons;
    [SerializeField] private DungeonModifier[] modifiers;
    [SerializeField] private DungeonSelectionUI selectionUI;

    private void Awake() {
        modifiers = GameManager.GetDungeonModifiers();
    }

    public void OnInteract() {
        if(InRange()) {
            selectionUI.UpdateUI(potentialDungeons, modifiers);
            selectionUI.Open();
        }
    }

    void OnEnable() {
        GameManager.Instance.input.Interact += OnInteract;
    }

    void OnDisable() {
        GameManager.Instance.input.Interact -= OnInteract;
    }

    private bool InRange() => IInteractable.InRange(transform.position);
}