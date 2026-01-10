using UnityEngine;
using Random = UnityEngine.Random;

namespace NOS.Controllers.Interactions
{
    public class InteractableColorRandomizer : InteractableBase
    {
        private Renderer _myRenderer;

        private void Awake()
        {
            _myRenderer = GetComponent<Renderer>();
        }

        protected override void Interact()
        {
            _myRenderer.material.color = new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        }
    }
}