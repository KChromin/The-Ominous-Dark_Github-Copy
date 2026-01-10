using System;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    public class ActivableLightDimmer : ActivableBase
    {
        [SerializeField]
        protected Light myLight;
        private float _startIntensity;

        private void Awake()
        {
            if (!myLight)
            {
                myLight = GetComponent<Light>();
            }

            _startIntensity = myLight.intensity;
        }

        public override void Activate()
        {
        }

        public override void OnProgressUpdate()
        {
            myLight.intensity = Mathf.Lerp(_startIntensity, 0, CurrentProgress01);
        }
    }
}