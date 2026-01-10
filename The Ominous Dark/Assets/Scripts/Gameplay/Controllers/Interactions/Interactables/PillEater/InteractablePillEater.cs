using System;
using System.Collections.Generic;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    public class InteractablePillEater : InteractableItemSlotActivator
    {
        [SerializeField]
        private Light[] lights;

        protected override void OnActivation()
        {
            if (lights[0].enabled)
            {
                foreach (var lightPoint in lights)
                {
                    lightPoint.enabled = false;
                }
            }
            else
            {
                foreach (var lightPoint in lights)
                {
                    lightPoint.enabled = true;
                }
            }
        }
    }
}