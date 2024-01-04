using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Input
{
    public class InputReader : MonoBehaviour, Controls.IPlayerActions
    {
        public static InputReader Instance;
    
        [SerializeField] private Camera mainCam;
        public Vector2 MovementValue { get; private set; }
        public Vector2 LookValue { get; private set; }
        public Vector2 JoystickAim { get; private set; }
        public Vector2 MousePosition { get; private set; }
    
        public bool IsUsingGamePadAiming { get; private set; }
    
        private Controls _controls;
        private bool _isMouseOverUI;

        public event Action OnShootButtonPress;
        public event Action OnShootButtonReleased;
        public event Action OnDashButtonPress;
        public event Action OnPauseButtonPress;

        private void Awake()
        {
            Instance = this;
        }

        private void Start(){
            _controls = new Controls();
            _controls.Player.SetCallbacks(this); //bind input class to controls class

            _controls.Player.Enable();
        }

        private void OnDestroy() {
            if ( _controls == null) { return; }
            _controls.Player.Disable();
        }
    
        public void OnMove(InputAction.CallbackContext context)
        {
            MovementValue = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookValue = context.ReadValue<Vector2>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(_isMouseOverUI) return;
            if (context.performed) OnShootButtonPress?.Invoke();
            if (context.canceled) OnShootButtonReleased?.Invoke();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if(context.ReadValue<Vector2>() == Vector2.zero) return;
            JoystickAim = context.ReadValue<Vector2>();
            IsUsingGamePadAiming = true;
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
            IsUsingGamePadAiming = false;
        }

        public void OnAnyKey(InputAction.CallbackContext context)
        {
            IsUsingGamePadAiming = false;
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(!context.performed)return;
            OnDashButtonPress?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnPauseButtonPress?.Invoke();
        }

        public void MouseEntersUI()
        {
            _isMouseOverUI = true;
        }

        public void MouseLeavesUi()
        {
            _isMouseOverUI = false;
        }
    }
}
