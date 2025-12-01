using System;
using JetBrains.Annotations;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    public interface IInteractable
    {
        //If object Can Be Used at all//
        public bool CanBeUsed { get; set; }
        
        //Interaction Object Name//
        public string ObjectName { get; set; }

        //Interaction Name//
        public InteractionActionName InteractionName { get; set; }

        //Interaction Mode//
        public InteractionModes InteractionMode { get; set; }

        //Player Starts Interacting//
        public void BeginInteraction();

        //Player Stops Interacting//
        public void EndInteraction();
        
    }

    public enum InteractionModes
    {
        OnClick,
        OnHold
    }

    public enum InteractionActionName
    {
        Use,
        Take,
        Open,
        Pull,
        Push
    }
}