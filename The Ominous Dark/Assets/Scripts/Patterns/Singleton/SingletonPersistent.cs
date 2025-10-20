using UnityEngine;

namespace NOS.Patterns.Singleton
{
    public class SingletonPersistent<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this as T;
            }

            #region Don't Destroy On Load

            //Make sure that it is persistent//
            //Check if object have parent//
            if (transform.parent != null)
            {
                //Check if root parent object is in DontDestroyOnLoad//
                if (transform.root.gameObject.scene.name != "DontDestroyOnLoad")
                {
                    //When root parent is not already in DontDestroyOnLoad, move him there//
                    DontDestroyOnLoad(transform.root.gameObject);
                }
            } //Without parent, just check the object//
            else if (gameObject.scene.name != "DontDestroyOnLoad")
            {
                //Move it to DontDestroyOnLoad//
                DontDestroyOnLoad(gameObject);
            }

            #endregion Don't Destroy On Load
        }
    }
}