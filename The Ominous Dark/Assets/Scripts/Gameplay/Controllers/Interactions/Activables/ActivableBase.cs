using System;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    [Serializable]
    public abstract class ActivableBase : MonoBehaviour, IActivable
    {
        public bool CanBeActivated { get; set; } = true;
        
        private float _currentProgress01;
        
        public float CurrentProgress01
        {
            get => _currentProgress01;
            set
            {
                _currentProgress01 = value;
                _currentProgress01 = Mathf.Clamp(_currentProgress01, 0, 1);
                OnProgressUpdate();
            }
        }
        
        public virtual void Activate()
        {
            
        }

        public virtual void OnProgressUpdate()
        {
            
        }
    }

    public interface IActivable
    {
        public bool CanBeActivated { get; set; }
        
        //Current Activation Progress//
        public float CurrentProgress01 { get; set; }

        public void Activate();
    }
}