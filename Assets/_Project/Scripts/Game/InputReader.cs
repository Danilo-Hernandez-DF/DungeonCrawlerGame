using static Game.InputControls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UtilsModule;
using UnityEditor;

namespace Game {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Utils/Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IDebugActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Aim = delegate { };
        public event UnityAction DebugRefresh = delegate { };
        public event UnityAction EnableMouseControlCursor = delegate { };
        public event UnityAction DisableMouseControlCursor = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction Attack = delegate { };
        public event UnityAction Pause = delegate { };
        public event UnityAction OpenMenu = delegate { };
        
        [SerializeField] IntEventChannel PageChangeCahnnel;

        InputControls inputActions;

        public Vector3 Direction => inputActions.Player.Movement.ReadValue<Vector2>();
        public Vector3 AimPosition => inputActions.Player.Aim.ReadValue<Vector2>();

        void OnEnable() {
            if(inputActions == null) {
                inputActions = new InputControls();
                inputActions.Player.SetCallbacks(this);
                inputActions.Debug.SetCallbacks(this);
            }
            inputActions.Enable();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            Vector2 scenePos = CameraManager.Instance.camera.ScreenToWorldPoint(context.ReadValue<Vector2>());
            Aim.Invoke(scenePos, IsDeviceMouse(context));
        }

        public void OnArtifact(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnAttack_Main(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started) {
                Attack.Invoke();
            }
        }

        public void OnAttack_Special(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnChange_Consumable(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnConsumable(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            switch(context.phase) {
                case InputActionPhase.Started:
                    Dash.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Dash.Invoke(false);
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPause(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) Pause.Invoke();
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnRefreshMap(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) DebugRefresh.Invoke();
        }

        public void OnOpenMenu(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) OpenMenu.Invoke();
        }
        
        public void OnPageNavigation(InputAction.CallbackContext context) { 
            if(context.phase == InputActionPhase.Started) PageChangeCahnnel?.Invoke((int)context.ReadValue<float>());
        }
    }
}