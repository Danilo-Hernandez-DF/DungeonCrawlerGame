using NavMeshPlus.Components;
using UnityEngine;

namespace UtilsModule {
    public class NavMeshManager : Singleton<NavMeshManager> {
        [SerializeField] NavMeshSurface surface;
        bool initialized = false;

        int framesSinceLast = 4;

        public static void BakeNavMesh() {
            Instance.surface.BuildNavMesh();
            Instance.initialized = true;
        }

        public void ClearData() {
            surface.RemoveData();
        }

        void FixedUpdate() {
            if(!initialized) return;
            if(framesSinceLast == 4) {
                surface.UpdateNavMesh(surface.navMeshData);
                framesSinceLast = 0;
            } else framesSinceLast++;
        }
    }
}
