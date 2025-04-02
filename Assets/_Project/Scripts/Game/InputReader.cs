using static Game.InputControls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UtilsModule;
using UnityEngine.Serialization;

namespace Game {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Utils/Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IDebugActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Aim = delegate { };
        public event UnityAction<int> UIInteract = delegate { };
        public event UnityAction DebugRefresh = delegate { };
        public event UnityAction EnableMouseControlCursor = delegate { };
        public event UnityAction DisableMouseControlCursor = delegate { };
        public event UnityAction Dash = delegate { };
        public event UnityAction Attack = delegate { };
        public event UnityAction Pause = delegate { };
        public event UnityAction OpenMenu = delegate { };
        public event UnityAction Interact = delegate { };
        public event UnityAction CheckQuests = delegate { }; 
        
        [FormerlySerializedAs("PageChangeCahnnel")] [SerializeField] IntEventChannel pageChangeCahnnel;

        InputControls inputActions;

        public InputControls Controls { get => inputActions; }

        public Vector3 Direction => inputActions.Player.Movement.ReadValue<Vector2>();
        public Vector2 AimPosition => CameraManager.Instance.camera.ScreenToWorldPoint(inputActions.Player.Aim.ReadValue<Vector2>());

        string GetContolScheme(InputAction.CallbackContext context) {
            foreach(var scheme in context.action.actionMap.controlSchemes) {
                if (scheme.SupportsDevice(context.control.device)) return scheme.name;
            }

            return null;
        }

        void OnEnable() {
            if(inputActions == null) {
                inputActions = new InputControls();
                inputActions.Player.SetCallbacks(this);
                inputActions.Debug.SetCallbacks(this);
            }
            inputActions.Enable();
        }

        public void OnAim(InputAction.CallbackContext context) {
            Aim.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
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

        public void OnDash(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) Dash.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Canceled) Interact.Invoke();
        }

        public void OnMovement(InputAction.CallbackContext context) {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPause(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) Pause.Invoke();
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => GetContolScheme(context) == "Keyboard-Mouse";

        public void OnRefreshMap(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) DebugRefresh.Invoke();
        }

        public void OnOpenMenu(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) OpenMenu.Invoke();
        }
        
        public void OnPageNavigation(InputAction.CallbackContext context) { 
            if(context.phase == InputActionPhase.Started) pageChangeCahnnel?.Invoke((int)context.ReadValue<float>());
        }

        public void OnUIInteract(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Started) UIInteract.Invoke((int)context.ReadValue<float>());
        }

        public void OnCheckQuests(InputAction.CallbackContext context) {
            if(context.phase == InputActionPhase.Canceled) CheckQuests.Invoke();
        }
    }
}