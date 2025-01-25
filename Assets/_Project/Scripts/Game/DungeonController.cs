using System;
using System.Collections.Generic;
using ProcGen;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class DungeonController : Singleton<DungeonController> {
        public DungeonGenerator dungeonGenerator;
        
        public void Start() {
            dungeonGenerator.Generate();
        }
        
        void ResetRooms() {
            foreach(RoomController room in dungeonGenerator.rooms) {
                room.Reset();
            }
        }
    }
}