using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UtilsModule;

namespace ProcGen {
    public class RoomController :  ValidatedMonoBehaviour {
        [SerializeField, Self] public BoxCollider2D col;
        public List<Dir> exits;
        private RoomTrigger triggerCondition;
        [SerializeField] bool completed;

        // List of spawners
        // List of spawned enemies

        void InitRoom() {
            if(completed) return;

            //close doors
            //spawn enemies
        }

        void EndRoom() {
            //spawn rewards
            //open doors

            completed = true;
        }

        void Update() {
            // room end logic
        }

        void OnTriggerEnter2D(Collider2D other) {
            if(!other.CompareTag("Player")) return;

            if(triggerCondition == RoomTrigger.Enter) InitRoom();
        }
    }

    public enum RoomType {NoRoom, StartRoom, PlaceholderRoom, EmptyRoom, HostileRoom, TreasureRoom, ShopRoom, BossRoom}
    public enum RoomTrigger {Enter}
}
