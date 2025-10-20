namespace NOS.Patterns.Controller
{
    public abstract class ControllerBase
    {
        #region Methodes

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void OnDestroy()
        {
        }

        #region Editor Only

#if UNITY_EDITOR
        public virtual void OnDrawGizmos()
        {
        }

#endif

        #endregion Editor Only

        #endregion Methodes
    }
}