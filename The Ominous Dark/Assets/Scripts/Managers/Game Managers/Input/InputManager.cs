using NOS.Patterns.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NOS.GameManagers.Input
{
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

            //Attack
            _inputs.Player.Attack.performed += AttackPerformed;
            _inputs.Player.Attack.canceled += AttackCanceled;
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

        #region Attack

        private void AttackPerformed(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingAttack = true;
            CurrentInput.OnPerformedAttack?.Invoke();
        }

        private void AttackCanceled(InputAction.CallbackContext context)
        {
            CurrentInput.inputtingAttack = false;
            CurrentInput.OnCancelAttack?.Invoke();
        }

        #endregion Attack

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

            //Attack
            _inputs.Player.Attack.performed -= AttackPerformed;
            _inputs.Player.Attack.canceled -= AttackCanceled;

            _inputs.Disable();
        }

        #endregion Methodes
    }
}