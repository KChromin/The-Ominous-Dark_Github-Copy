using System;
using UnityEngine;

namespace NOS.Helpers
{
    public class TriggerCallbackForwarder : MonoBehaviour
    {
        public event Action<Collider> OnTriggerEnterCallback;
        public event Action<Collider> OnTriggerExitCallback;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterCallback?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitCallback?.Invoke(other);
        }
    }
}