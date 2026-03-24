using UnityEngine;
using UnityEngine.Rendering;

namespace NOS.Controllers.Interactions
{
    public class LightProximityDetector : ActivableProximityBase
    {
        private Light _light;
        private readonly float _lightMaxIntensity = 3000;
        
        private void Awake()
        {
            _light = GetComponent<Light>();
            _light.lightUnit = LightUnit.Candela;
        }

        public override void Activate()
        {
            _light.intensity = _lightMaxIntensity;
        }

        protected override void Deactivate()
        {
            _light.intensity = 0;
        }
    }
}
