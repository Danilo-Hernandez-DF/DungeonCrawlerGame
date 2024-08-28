using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using NavMeshPlus.Components;
using UnityEngine;
using UtilsModule;

namespace UtilsModule {
    public class NavMeshManager : ValidatedSingleton<NavMeshManager> {
        [SerializeField, Self] NavMeshSurface surface;

        int framesSinceLast = 4;

        public static void BakeNavMesh() {
            Instance.surface.BuildNavMesh();
        }

        public void ClearData() {
            surface.RemoveData();
        }

        void FixedUpdate() {
            if(framesSinceLast == 4) {
                surface.UpdateNavMesh(surface.navMeshData);
                framesSinceLast = 0;
            } else framesSinceLast++;
        }
    }
}
