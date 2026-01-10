using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.GameManagers.Input
{
    [Serializable]
    public class InputDataContainer
    {
        public const float MinimumInputMagnitudeBeforeTrigger = 0.125f;

        //Look
        [Header("Look")]
        public bool inputtingLook;
        public Vector2 inputLook;

        //Move
        [Header("Move")]
        public bool inputtingMove;
        public Vector2 inputMove;
        public Vector3 inputMove3D;

        //Run
        [Header("Run")]
        public bool inputtingRun;
        public Action OnPerformedRun;
        public Action OnCancelRun;

        //Jump
        [Header("Jump")]
        public bool inputtingJump;
        public Action OnPerformedJump;
        public Action OnCancelJump;

        //Crouch
        [Header("Crouch")]
        public bool inputtingCrouch;
        public Action OnPerformedCrouch;
        public Action OnCancelCrouch;

        //Interact
        [Header("Interact")]
        public bool inputtingInteract;
        public Action OnPerformedInteract;
        public Action OnCancelInteract;

        //Inventory
        [Header("Inventory Scrolls")]
        public bool inputtingInventoryScroll;
        public Vector2 inventoryScrollValue;
        public Action<Vector2> OnPerformedInventoryScroll;
        public Action<Vector2> OnCancelInventoryScroll;

        [Header("Inventory Throw")]
        public bool inputtingInventoryThrow;
        public Action OnPerformedInventoryThrow;
        public Action OnCancelInventoryThrow;

        //Numpad
        [Header("Inventory Numpad")]
        public Action OnPerformedInventoryNumpad1;
        public Action OnPerformedInventoryNumpad2;
        public Action OnPerformedInventoryNumpad3;
        public Action OnPerformedInventoryNumpad4;
        public Action OnPerformedInventoryNumpad5;

        [Header("Inventory Actions")]
        public bool inputtingInventoryActionMain;
        public Action OnPerformedInventoryActionMain;
        public Action OnCancelInventoryActionMain;

        public bool inputtingInventoryActionSecondary;
        public Action OnPerformedInventoryActionSecondary;
        public Action OnCancelInventoryActionSecondary;
    }
}