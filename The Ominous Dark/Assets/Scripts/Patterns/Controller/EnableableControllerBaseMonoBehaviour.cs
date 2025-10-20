using UnityEngine;

namespace NOS.Patterns.Controller
{
    public abstract class EnableableControllerBaseMonoBehaviour : MonoBehaviour
    {
        #region Enabledness

        //Is controller enabled//
        protected bool IsEnabled { get; private set; }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SetEnabledness(bool newEnablednessState)
        {
            IsEnabled = newEnablednessState;

            if (IsEnabled) //Enable
            {
                OnControllerEnable();
            }
            else //Disable
            {
                OnControllerDisable();
            }
        }

        #endregion Enabledness

        #region OnEnable, OnDisable

        //Activates immediately after controller being enabled//
        protected virtual void OnControllerEnable()
        {
        }

        //Activates just before controller being disabled//
        protected virtual void OnControllerDisable()
        {
        }

        #endregion OnEnable, OnDisable
    }
}