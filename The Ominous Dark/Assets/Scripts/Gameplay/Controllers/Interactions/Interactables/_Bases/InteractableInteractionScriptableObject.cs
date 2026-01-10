using System;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    [CreateAssetMenu(fileName = "InteractableParameters", menuName = "ScriptableObjects/Interactable")]
    public class InteractableInteractionScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public bool DisableAfterInteraction { get; set; } = true;

        [field: Space, SerializeField]
        public InteractionModes InteractionMode { get; set; } = InteractionModes.OnHold;

        [field: Space, SerializeField]
        public InteractableOnHoldSettingsClass OnHoldParameters { get; set; } = new();
    }
}