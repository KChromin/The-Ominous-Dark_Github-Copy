using System;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    public class ActivableLightSwitch : ActivableBase
    {
        [SerializeField]
        protected Light myLight;

        private void Awake()
        {
            if (!myLight)
            {
                myLight = GetComponent<Light>();
            }
        }

        public override void Activate()
        {
            myLight.enabled = !myLight.enabled;
        }
    }
}