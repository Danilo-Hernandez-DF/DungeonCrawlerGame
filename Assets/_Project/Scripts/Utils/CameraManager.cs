using KBCore.Refs;
using UnityEngine;

namespace UtilsModule {
    public class CameraManager : ValidatedSingleton<CameraManager> {
        [SerializeField, Child] public new Camera camera;
    }
}