using System.Collections.Generic;
using _Project.Scripts.Utils;
using Systems.Persistence;
using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/ItemReward")]
    public class ItemRewardQuest : Quest {
        public WeightedItem[] rewards;
        
        public override void OnCompletion(NPC questGiver = null) {
            if(questGiver) {
                if(SaveLoadSystem.Instance.GetData(questGiver.Id) is NPCData data) {
                    NPCData tempData = data;
                    data.questComplete = true;
                    SaveLoadSystem.Instance.SetData(questGiver.Id, data);
                }
            }

            List<Item> itemRewards = new List<Item>();
            foreach (WeightedItem wItem in rewards) {
                itemRewards.Add(wItem.GetItem(wItem.countMin));
            }

            if(!GameSettings.Instance.questRewardInv.Inventory.TryAddBulk(itemRewards.ToArray())) {
                Debug.Log("Quest rewards can't be claimed right now");
                return;
            }
            
            Debug.Log("Claimed quest rewards");
        }
    }
}