using UnityEngine;

namespace UtilsModule {
    public class CameraManager : Singleton<CameraManager> {
        [SerializeField] public new Camera camera;
    }
}