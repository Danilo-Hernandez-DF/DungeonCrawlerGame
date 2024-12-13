using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using NavMeshPlus.Components;
using UnityEngine;
using UtilsModule;

namespace UtilsModule {
    public class NavMeshManager : ValidatedSingleton<NavMeshManager> {
        [SerializeField, Self] NavMeshSurface surface;
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
