using NOS.Patterns.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NOS.GameManagers.Input
{
    [DefaultExecutionOrder(-89)]
    public class InputManager : SingletonPersistent<InputManager>
    {
        [field: SerializeField]
        public InputDataContainer CurrentInput { get; private set; }

        private GameInputs _inputs;

        #region Methodes

        protected override void Awake()
        {
            //Setup Singleton//
            base.Awake();

            //Create data container//
            CurrentInput = new InputDataContainer();

            //Create Input Actions//
            _inputs = new GameInputs();
            _inputs.Enable();

            InputEventSetup();
        }

        private void InputEventSetup()
        {
            //Look
            _inputs.Player.Look.performed += LookPerformed;
            _inputs.Player.Look.canceled += LookCanceled;

            //Move
            _inputs.Player.Move.performed += MovePerformed;
            _inputs.Player.Move.canceled += MoveCanceled;

            //Sprint
            _inputs.Player.Run.performed += RunPerformed;
            _inputs.Player.Run.canceled += RunCanceled;

            //Jump
            _inputs.Player.Jump.performed += JumpPerformed;
            _inputs.Player.Jump.canceled += JumpCanceled;

            //Crouch
            _inputs.Player.Crouch.performed += CrouchPerformed;
            _inputs.Player.Crouch.canceled += CrouchCanceled;

            //Interact
            _inputs.Player.Interact.performed += InteractPerformed;
            _inputs.Player.Interact.canceled += InteractCanceled;

            //Inventory
            _inputs.Player.InventoryScroll.performed += InventoryScrollPerformed;
            _inputs.Player.InventoryScroll.canceled += InventoryScrollCanceled;

            //Throw
            _inputs.Player.InventoryThrow.performed += InventoryThrowPerformed;
            _inputs.Player.InventoryThrow.canceled += InventoryThrowCanceled;

            //Numpad
            _inputs.Player.InventoryNumpad1.performed += InventoryNumpad1Performed;
            _inputs.Player.InventoryNumpad2.performed += InventoryNumpad2Performed;
            _inputs.Player.InventoryNumpad3.performed += InventoryNumpad3Performed;
            _inputs.Player.InventoryNumpad4.performed += InventoryNumpad4Performed;
            _inputs.Player.InventoryNumpad5.performed += InventoryNumpad5Performed;

            _inputs.Player.InventoryActionMain.performed += InventoryActionMainPerformed;
            _inputs.Player.InventoryActionMain.canceled += InventoryActionMainCanceled;

            _inputs.Player.InventoryActionSecondary.performed += InventoryActionSecondaryPerformed;
            _inputs.Player.InventoryActionSecondary.canceled += InventoryActionSecondaryCanceled;
        }

        #region Look

        //Look
        private void LookPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingLook = true;

            //Update input value//
            CurrentInput.inputLook = _inputs.Player.Look.ReadValue<Vector2>();
        }

        private void LookCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingLook = false;

            //Reset input//
            CurrentInput.inputLook = Vector2.zero;
        }

        #endregion Look

        #region Move

        private void MovePerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingMove = true;

            //Update input values//
            CurrentInput.inputMove = _inputs.Player.Move.ReadValue<Vector2>();
            CurrentInput.inputMove3D = new Vector3(CurrentInput.inputMove.x, 0, CurrentInput.inputMove.y);
        }

        private void MoveCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingMove = false;

            //Reset input//
            CurrentInput.inputMove = Vector2.zero;
            CurrentInput.inputMove3D = Vector3.zero;
        }

        #endregion Move

        #region Sprint

        private void RunPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingRun = true;
            CurrentInput.OnPerformedRun?.Invoke();
        }

        private void RunCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingRun = false;
            CurrentInput.OnCancelRun?.Invoke();
        }

        #endregion Sprint

        #region Jump

        private void JumpPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingJump = true;
            CurrentInput.OnPerformedJump?.Invoke();
        }

        private void JumpCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingJump = false;
            CurrentInput.OnCancelJump?.Invoke();
        }

        #endregion Jump

        #region Crouch

        private void CrouchPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingCrouch = true;
            CurrentInput.OnPerformedCrouch?.Invoke();
        }

        private void CrouchCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingCrouch = false;
            CurrentInput.OnCancelCrouch?.Invoke();
        }

        #endregion Crouch

        #region Interact

        private void InteractPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInteract = true;
            CurrentInput.OnPerformedInteract?.Invoke();
        }

        private void InteractCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInteract = false;
            CurrentInput.OnCancelInteract?.Invoke();
        }

        #endregion Interact

        #region Inventory

        private void InventoryScrollPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryScroll = true;
            Vector2 scrollValue = context.ReadValue<Vector2>();
            CurrentInput.inventoryScrollValue = scrollValue;
            CurrentInput.OnPerformedInventoryScroll?.Invoke(scrollValue);
        }

        private void InventoryScrollCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryScroll = false;
            CurrentInput.inventoryScrollValue = Vector2.zero;
            CurrentInput.OnCancelInventoryScroll?.Invoke(Vector2.zero);
        }

        private void InventoryThrowPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryThrow = true;
            CurrentInput.OnPerformedInventoryThrow?.Invoke();
        }

        private void InventoryThrowCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryThrow = false;
            CurrentInput.OnCancelInventoryThrow?.Invoke();
        }

        #region Numpad

        private void InventoryNumpad1Performed(InputAction.CallbackContext context)
        {
            CurrentInput.OnPerformedInventoryNumpad1?.Invoke();
        }

        private void InventoryNumpad2Performed(InputAction.CallbackContext context)
        {
            CurrentInput.OnPerformedInventoryNumpad2?.Invoke();
        }

        private void InventoryNumpad3Performed(InputAction.CallbackContext context)
        {
            CurrentInput.OnPerformedInventoryNumpad3?.Invoke();
        }

        private void InventoryNumpad4Performed(InputAction.CallbackContext context)
        {
            CurrentInput.OnPerformedInventoryNumpad4?.Invoke();
        }

        private void InventoryNumpad5Performed(InputAction.CallbackContext context)
        {
            CurrentInput.OnPerformedInventoryNumpad5?.Invoke();
        }

        #endregion Numpad

        private void InventoryActionMainPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryActionMain = true;
            CurrentInput.OnPerformedInventoryActionMain?.Invoke();
        }

        private void InventoryActionMainCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryActionMain = false;
            CurrentInput.OnCancelInventoryActionMain?.Invoke();
        }

        private void InventoryActionSecondaryPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryActionSecondary = true;
            CurrentInput.OnPerformedInventoryActionSecondary?.Invoke();
        }

        private void InventoryActionSecondaryCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingInventoryActionSecondary = false;
            CurrentInput.OnCancelInventoryActionSecondary?.Invoke();
        }

        #endregion Inventory

        private void OnDisable()
        {
            //Look
            _inputs.Player.Look.performed -= LookPerformed;
            _inputs.Player.Look.canceled -= LookCanceled;

            //Move
            _inputs.Player.Move.performed -= MovePerformed;
            _inputs.Player.Move.canceled -= MoveCanceled;

            //Sprint
            _inputs.Player.Run.performed -= RunPerformed;
            _inputs.Player.Run.canceled -= RunCanceled;

            //Jump
            _inputs.Player.Jump.performed -= JumpPerformed;
            _inputs.Player.Jump.canceled -= JumpCanceled;

            //Crouch
            _inputs.Player.Crouch.performed -= CrouchPerformed;
            _inputs.Player.Crouch.canceled -= CrouchCanceled;

            //Interact
            _inputs.Player.Interact.performed -= InteractPerformed;
            _inputs.Player.Interact.canceled -= InteractCanceled;

            //Inventory
            _inputs.Player.InventoryScroll.performed -= InventoryScrollPerformed;
            _inputs.Player.InventoryScroll.canceled -= InventoryScrollCanceled;

            //Throw
            _inputs.Player.InventoryThrow.performed -= InventoryThrowPerformed;
            _inputs.Player.InventoryThrow.canceled -= InventoryThrowCanceled;

            _inputs.Player.InventoryNumpad1.performed -= InventoryNumpad1Performed;
            _inputs.Player.InventoryNumpad2.performed -= InventoryNumpad2Performed;
            _inputs.Player.InventoryNumpad3.performed -= InventoryNumpad3Performed;
            _inputs.Player.InventoryNumpad4.performed -= InventoryNumpad4Performed;
            _inputs.Player.InventoryNumpad5.performed -= InventoryNumpad5Performed;

            _inputs.Player.InventoryActionMain.performed -= InventoryActionMainPerformed;
            _inputs.Player.InventoryActionMain.canceled -= InventoryActionMainCanceled;

            _inputs.Player.InventoryActionSecondary.performed -= InventoryActionSecondaryPerformed;
            _inputs.Player.InventoryActionSecondary.canceled -= InventoryActionSecondaryCanceled;

            _inputs.Disable();
        }

        #endregion Methodes
    }
}