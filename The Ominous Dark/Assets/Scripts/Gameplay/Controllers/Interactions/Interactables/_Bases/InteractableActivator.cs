using System.Collections.Generic;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    public class InteractableActivator : InteractableBase
    {
        [Header("Objects To Activate"), SerializeField]
        public List<ActivableBase> objectsToActivate;

        #region Setup

#if UNITY_EDITOR
        private void Awake()
        {
            if (objectsToActivate == null || objectsToActivate.Count == 0)
            {
                Debug.LogError("Activator Has No Objects To Activate!!!", this);
            }
        }
#endif

        #endregion Setup

        #region Instant

        protected override void Interact()
        {
            foreach (var activable in objectsToActivate)
            {
                if (activable == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("Missing Reference To Activable!!!", this);
#endif
                    continue;
                }

                //Simple Activation
                activable.Activate();
            }
        }

        #endregion Instant

        #region Progression Update

        protected override void Update()
        {
            if (!InteractableSettings.CanBeUsed) return;
            if (InteractableSettings.InteractionParameters.InteractionMode == InteractionModes.OnHoldProgressionOnly)
            {
                base.Update();
                UpdateActivablesProgress();
            }
            else
            {
                base.Update();
            }
        }


        private void UpdateActivablesProgress()
        {
            float currentProgress = GetCurrentProgress();

            if (currentProgress is 0 or 1) return;

            foreach (ActivableBase activable in objectsToActivate)
            {
                if (!activable)
                {
#if UNITY_EDITOR
                    Debug.LogError("Missing Reference To Activable!!!", this);
#endif
                    continue;
                }

                activable.CurrentProgress01 = currentProgress;
            }
        }

        #endregion Progression Update
    }
}