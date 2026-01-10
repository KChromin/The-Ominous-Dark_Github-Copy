using UnityEngine;
using Random = UnityEngine.Random;

namespace NOS.Item
{
    public class TestItem : ItemBase
    {
        private Light _light;

        private void Start()
        {
            _light = GetComponent<Light>();
        }

        protected override void DisableItem()
        {
            _light.enabled = false;
        }

        protected override void ExecuteMainAction()
        {
            _light.enabled = !_light.enabled;
        }

        protected override void ExecuteSecondaryAction()
        {
            Vector4 newColor = new (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            _light.color = newColor;
        }
    }
}