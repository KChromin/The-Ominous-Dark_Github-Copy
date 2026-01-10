using UnityEngine;
using UnityEngine.Localization;

namespace NOS.Item
{
    [CreateAssetMenu(fileName = "ItemParameters", menuName = "ScriptableObjects/Item")]
    public class ItemScriptableObject : ScriptableObject
    {
        [Header("Item Description")]
        public LocalizedString itemName;
        public LocalizedString itemDescription;
        public Texture2D itemEquipmentPicture;

        [Header("Item Actions")]
        public ItemActionType itemMainActionInteractionType = ItemActionType.OnClick;
        public LocalizedString itemMainActionDescription;
        [Space]
        public ItemActionType itemSecondaryActionInteractionType = ItemActionType.OnClick;
        public LocalizedString itemSecondaryActionDescription;
    }
}