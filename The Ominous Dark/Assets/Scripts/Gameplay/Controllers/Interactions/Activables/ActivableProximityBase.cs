using System;
using System.Collections.Generic;
using NOS.Item;
using UnityEngine;

namespace NOS.Controllers.Interactions
{
    [RequireComponent(typeof(Collider))]
    public abstract class ActivableProximityBase : ActivableBase
    {
        [SerializeField]
        protected ActivableProximityParametersClass parameters;

        [Space, SerializeField]
        protected List<GameObject> objectsInProximity = new();

        private readonly Dictionary<ActivableProximityActivationTags, string> _activationTags = new()
        {
            { ActivableProximityActivationTags.Player, "Player" },
            { ActivableProximityActivationTags.Item, "Item" }
        };


        #region Check Proximity & Manage Items

        private void OnTriggerEnter(Collider other)
        {
            if (!CanBeActivated) return;

            if (parameters.filters.useFilterByTag)
            {
                if (ObjectContainedInTagList(other.tag))
                {
                    AddObjectToProximityList(other.gameObject);
                    return;
                }
            }

            if (parameters.filters.useFilterByName)
            {
                for (int i = 0; i < parameters.filters.activatingNames.Length; i++)
                {
                    if (other.name.Contains(parameters.filters.activatingNames[i]))
                    {
                        AddObjectToProximityList(other.gameObject);
                        return;
                    }
                }
            }


            if (parameters.filters.useFilterByItemType)
            {
                ItemBase item = other.GetComponent<ItemBase>();

                if (item)
                {
                    for (int i = 0; i < parameters.filters.activatingItems.Length; i++)
                    {
                        if (item.itemParameters == parameters.filters.activatingItems[i])
                        {
                            AddObjectToProximityList(other.gameObject);
                            return;
                        }
                    }
                }
            }
        }

        private bool ObjectContainedInTagList(string objectTag)
        {
            for (int i = 0; i < parameters.filters.activatingTags.Length; i++)
            {
                if (objectTag == _activationTags[parameters.filters.activatingTags[i]])
                {
                    return true;
                }
            }

            return false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!CanBeActivated) return;

            RemoveObjectFromProximityList(other.gameObject);
        }

        private void AddObjectToProximityList(GameObject objectInProximity)
        {
            if (parameters.activableProximityMode == ActivableProximityModes.Activation && objectsInProximity.Count == 0)
            {
                Activate();
            }

            objectsInProximity.Add(objectInProximity);

            //Item handling//
            ItemBase item = objectInProximity.GetComponent<ItemBase>();
            if (item)
            {
                item.OnDisableItem += OnItemDisable;
            }
        }

        private void RemoveObjectFromProximityList(GameObject objectInProximity)
        {
            objectsInProximity.RemoveAll(objects => objects == objectInProximity);

            //Item handling//
            ItemBase item = objectInProximity.GetComponent<ItemBase>();
            if (item)
            {
                item.OnDisableItem -= OnItemDisable;
            }

            if (parameters.activableProximityMode == ActivableProximityModes.Activation && objectsInProximity.Count == 0)
            {
                Deactivate();
            }
        }

        private void OnItemDisable(GameObject item)
        {
            RemoveObjectFromProximityList(item);
        }

        #endregion Check Proximity & manage items

        protected virtual void Deactivate()
        {
        }

        private void Update()
        {
            if (parameters.activableProximityMode == ActivableProximityModes.Activation) return;

            if (objectsInProximity.Count != 0)
            {
                CurrentProgress01 += Time.deltaTime * (1 / parameters.progressIncreaseDuration);
            }
            else if (CurrentProgress01 != 0)
            {
                CurrentProgress01 -= Time.deltaTime * (1 / parameters.progressDecreaseDuration);
            }
        }
    }


    [Serializable]
    public class ActivableProximityParametersClass
    {
        [Header("Activation Mode")]
        public ActivableProximityModes activableProximityMode;

        [Header("Progress Parameters")]
        public float progressIncreaseDuration = 1;
        public float progressDecreaseDuration = 1;

        [Header("Activation Filters")]
        public ActivableFilters filters;
        
        [Serializable]
        public class ActivableFilters
        {
            [Header("Tags")]
            public bool useFilterByTag;
            public ActivableProximityActivationTags[] activatingTags;
            [Header("Names")]
            public bool useFilterByName;
            public string[] activatingNames;
            [Header("Item Types")]
            public bool useFilterByItemType;
            public ItemScriptableObject[] activatingItems;
        }
    }

    public enum ActivableProximityModes
    {
        Activation,
        Progress
    }

    public enum ActivableProximityActivationTags
    {
        Player,
        Item
    }
}