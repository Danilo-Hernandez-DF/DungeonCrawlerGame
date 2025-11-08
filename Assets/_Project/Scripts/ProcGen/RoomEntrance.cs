using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilsModule;

namespace ProcGen {
    public class RoomEntrance : MonoBehaviour { 
        [SerializeField] LayerMask entraceMask;
        public GameObject doorPrefab;
        public GameObject wallPrefab;
        public Dir dir;
        public RoomController room;
        public RoomEntrance connectedEntrance;
        public bool Active { get; private set; } = true;
        public void DeActivate() => Active = false;
        public Orientation Orientation => dir.ToOrientation();
        public Vector2 Pos => transform.position;
        Collider2D col;

        void Awake() {
            col = GetComponent<Collider2D>();
        }

        public RoomEntrance GetOverlappingEntrance() {
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(Pos, col.bounds.size, 0, entraceMask);
            List<RoomEntrance> result = new();
                
            foreach(var overlap in overlaps) {
                var entrance = overlap.GetComponent<RoomEntrance>();
                if(!entrance) continue;
                if(entrance != this) result.Add(entrance);
            }

            return result.Count > 0 ? result[0] : null;
        }
    }
}