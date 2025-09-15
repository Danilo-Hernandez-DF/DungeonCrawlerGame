using System.Collections.Generic;
using _Project.Scripts.Utils;
using Game;
using Systems.Persistence;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsModule;

public class DungeonExit : MonoBehaviour, IInteractable {
    [SerializeField] private SceneAsset nextLevelScene;
    [SerializeField] private bool isEntrance = false;
    [SerializeField] private LootTable<DungeonData> potentialDungeons;
    [SerializeField] private BoolDungeonDataEventChannel eventChannel;

    public void OnInteract() {
        if(InRange()) {
            List<DungeonData> options = new List<DungeonData>();
            if(isEntrance) {
                DungeonData dungeonData = potentialDungeons.GetWeightedItem();
                options.Add(dungeonData);
            } else {
                for(int i = 0; i < 3; i++) {
                    var dungeon = potentialDungeons.GetWeightedItem();
                    if(!options.Contains(dungeon)) {
                        options.Add(dungeon);
                    } else {
                        i--;
                    }
                }
            }

            eventChannel.Invoke(isEntrance, options.ToArray());
        }
    }

    public void TransitionScene(DungeonData dungeonData) {
        if(!InRange()) return;

        GameManager.Instance.LoadScene(nextLevelScene.name, isEntrance);
        SceneManager.LoadScene(nextLevelScene.name);
    }

    void OnEnable() {
        GameManager.Instance.input.Interact += OnInteract;
    }

    void OnDisable() {
        GameManager.Instance.input.Interact -= OnInteract;
    }

    private bool InRange() => IInteractable.InRange(transform.position);
}