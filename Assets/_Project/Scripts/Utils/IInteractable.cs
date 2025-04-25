using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule {
    public interface IInteractable {
        void OnInteract();
        
        static bool InRange(Vector3 position) {
            var colliders = Physics2D.OverlapCircleAll(position, 1f);
            return colliders.Any(collider => collider.CompareTag("Player"));
        }
    }
}