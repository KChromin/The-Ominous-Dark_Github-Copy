using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NOS.GameManagers.Input
{
    [Serializable]
    public class InputDataContainer
    {
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

        //Attack
        [Header("Attack")]
        public bool inputtingAttack;
        public Action OnPerformedAttack;
        public Action OnCancelAttack;
    }
}