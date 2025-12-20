using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI References")]
        //private Image backgroundImage;
        [SerializeField] public Image itemIcon;
        //[SerializeField] private TextMeshProUGUI itemCountText;
        //[SerializeField] private TextMeshProUGUI slotNumberText;
        
        private int slotIndex = -1;
        private Item currentItem;
        
        public void Initialize(int index, Item item)
        {
            slotIndex = index;
            currentItem = item;
            
            // Update UI elements
            UpdateVisuals();
            
            // Show slot number
            /*if (slotNumberText != null)
            {
                slotNumberText.text = (index + 1).ToString();
            }*/
        }
        
        public void UpdateSlot(Item item)
        {
            currentItem = item;
            UpdateVisuals();
        }
        
        private void UpdateVisuals()
        {
            // Update icon
            if (itemIcon != null)
            {
                if (currentItem != null && currentItem.icon != null)
                {
                    itemIcon.sprite = currentItem.icon;
                    itemIcon.color = Color.white;
                    itemIcon.enabled = true;
                }
                else
                {
                    itemIcon.sprite = null;
                    itemIcon.enabled = false;
                }
            }
            
            // Update count text
            /*if (itemCountText != null)
            {
                itemCountText.text = currentItem != null ? "1" : "";
            }*/
            
            // Update background
            /*if (backgroundImage != null)
            {
                backgroundImage.color = currentItem != null ? 
                    new Color(0.2f, 0.2f, 0.2f, 0.8f) : 
                    new Color(0.1f, 0.1f, 0.1f, 0.5f);
            }*/
        }
        
        public void SetSelected(bool selected, Color selectedColor)
        {
            /*if (backgroundImage != null)
            {
                if (selected)
                {
                    backgroundImage.color = selectedColor;
                }
                else
                {
                    backgroundImage.color = currentItem != null ? 
                        new Color(0.2f, 0.2f, 0.2f, 0.8f) : 
                        new Color(0.1f, 0.1f, 0.1f, 0.5f);
                }
            }*/
        }
        
        // Add click handler
        public void OnSlotClicked()
        {
            Debug.Log($"Slot {slotIndex} clicked. Item: {(currentItem != null ? currentItem.itemName : "Empty")}");
            
            // You could add functionality to use/equip item when clicked
            if (currentItem != null && PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.UseSelectedItem();
            }
        }
    }
}