using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class PlayerDetector : MonoBehaviour {
        public static GameObject GetPlayer() {
            return GameObject.FindGameObjectWithTag("Player");
        }
        
        public static PlayerController GetPlayerComponent() => GetPlayer().GetComponent<PlayerController>();
    }
}
