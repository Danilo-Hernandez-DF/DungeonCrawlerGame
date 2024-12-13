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
        public bool Active { get; private set; } = true;
        public void DeActivate() => Active = false;
        public Orientation orientation => dir.ToOrientation();
        public Vector2 pos => transform.position;
        Collider2D col;

        void Awake() {
            col = GetComponent<Collider2D>();
        }

        public List<RoomEntrance> GetOverlappingEntrances() {
            List<RoomEntrance> result = Physics2D.OverlapBoxAll(pos, col.bounds.size, 0, entraceMask)
                .Select(x => x.GetComponent<RoomEntrance>()).Where(x => x != this).ToList();

            return result;
        }
    }
}