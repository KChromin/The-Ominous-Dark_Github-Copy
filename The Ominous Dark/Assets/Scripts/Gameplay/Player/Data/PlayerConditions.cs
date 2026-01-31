using System;
using UnityEngine;

namespace NOS.Player.Data
{
    [Serializable]
    public class PlayerConditions
    {
        #region General

        [field: SerializeField]
        public GeneralConditionsClass General { get; set; } = new();

        [Serializable]
        public class GeneralConditionsClass
        {
            #region Cases

            public CasesClass cases = new();

            [Serializable]
            public class CasesClass
            {
                public bool isAlive = true; //When not alive player cannot do anything//
                public bool isChangingPosition; //Currently position in world is changing//
            }

            #endregion Cases

            #region Possibilities

            public PossibilitiesClass possibilities = new();

            [Serializable]
            public class PossibilitiesClass
            {
                public bool canChangePosition = true; //Can change position in world//
            }

            #endregion Possibilities
        }

        #endregion General

        #region Default

        [field: Space, SerializeField]
        public DefaultConditionsClass Default { get; set; } = new();

        [Serializable]
        public class DefaultConditionsClass
        {
            #region Cases

            public CasesClass cases = new();

            [Serializable]
            public class CasesClass
            {
                public bool isGrounded; //Player is on the ground//
                [Space]
                public bool isOnTooSteepSlope; //Player should be sliding off that slope//
                public bool wantsToMoveOnTooSteepSlope;
                [Space]
                public bool isLooking; //When currently looking around//
                [Space]
                public bool wantsToMove; //Player gave input to move
                public bool isMoving; //When player is in any moving state//
                public bool isMovingAboveMinimalThreshold; //If it's changing position in speed that justifies head bobbing
                [Space]
                public bool wantsToRun; //Player gave input to run
                public bool isRunning; //When player is in running state//
                public bool isRunningAboveMinimalThreshold; //If it's changing position in speed that justifies head bobbing
                [Space]
                public bool wantsToJump; //Player gave input to jump//
                public bool isJumping; //Already jumped//
                public bool jumpIsOnCooldown;
                [Space]
                public bool wantsToDoCrouchAction; //Player gave/released/clicked again input to crouch
                public bool isCrouching; //When player 
                public bool isAbleToStandUp; //When player can stand up//
                [Space]
                public bool staminaWasFullyDepleted; //When stamina is fully used and player cannot run or jump
                public bool staminaIsTooLowToJump;
                [Space]
                public bool isInteracting; //Is interacting with something//
                [Space]
                //Inventory
                public bool inventoryItemSwitchingIsBlocked;
                public bool inventoryWantsToThrowItem;
                public bool inventoryIsTransitioningItems;
            }

            #endregion Cases

            #region Possibilities

            public PossibilitiesClass possibilities = new();

            [Serializable]
            public class PossibilitiesClass
            {
                public bool canLookY = true;
                public bool canLookX = true;
                [Space]
                public bool canMove = true;
                public bool canRun = true;
                public bool canJump = true;
                public bool canCrouch = true;
                [Space]
                public bool canInteract = true;
            }

            #endregion Possibilities
        }

        #endregion Default
    }
}