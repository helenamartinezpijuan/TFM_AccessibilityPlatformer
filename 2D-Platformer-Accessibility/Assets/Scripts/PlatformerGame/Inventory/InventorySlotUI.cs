using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private int slotIndex;
        private Item currentItem;

        public void Initialize(int index, Item item)
        {
            slotIndex = index;
            UpdateSlot(item);
            SetSelected(false);
        }

        public void UpdateSlot(Item item)
        {
            currentItem = item;

            if (item != null && itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.color = Color.white;
                
                if (itemCountText != null)
                    itemCountText.text = "1";          
            }
            else
            {
                if (itemIcon != null)
                {
                    itemIcon.sprite = null;
                    itemIcon.color = Color.clear;
                }
                else
                {
                    Debug.Log("Item icon reference missing");
                }
                
                if (itemCountText != null)
                    itemCountText.text = "";
            }
        }

        public void SetSelected(bool selected)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = selected ? selectedColor : normalColor;
            }
        }
    }
}