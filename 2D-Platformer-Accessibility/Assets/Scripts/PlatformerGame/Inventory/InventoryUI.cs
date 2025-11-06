using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform inventorySlotsParent;
        [SerializeField] private GameObject inventorySlotPrefab;
        [SerializeField] private Image selectedSlotHighlight;
        
        private Inventory playerInventory;
        private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
        private int lastSelectedPosition = -1;

        private void Start()
        {
            // Find the player inventory in the main scene
            FindPlayerInventory();
            InitializeUI();
        }

        private void FindPlayerInventory()
        {
            // Look for inventory in the main scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
                if (playerInventory != null)
                {
                    SubscribeToEvents();
                }
            }
        }

        private void InitializeUI()
        {
            if (playerInventory == null) return;

            // Clear existing slots
            foreach (Transform child in inventorySlotsParent)
            {
                Destroy(child.gameObject);
            }
            slotUIs.Clear();

            // Create slots based on inventory size
            for (int i = 0; i < playerInventory.Items.Count; i++)
            {
                GameObject slotObject = Instantiate(inventorySlotPrefab, inventorySlotsParent);
                InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
                
                if (slotUI != null)
                {
                    slotUIs.Add(slotUI);
                    slotUI.Initialize(i, playerInventory.Items[i]);
                }
            }

            // Update selection highlight
            UpdateSelectionHighlight(playerInventory.CurrentSelectedPosition);
        }

        private void SubscribeToEvents()
        {
            playerInventory.OnItemAdded += OnItemAdded;
            playerInventory.OnItemRemoved += OnItemRemoved;
            playerInventory.OnSelectionChanged += OnSelectionChanged;
            playerInventory.OnInventoryToggle += OnInventoryToggle;
        }

        private void UnsubscribeFromEvents()
        {
            if (playerInventory != null)
            {
                playerInventory.OnItemAdded -= OnItemAdded;
                playerInventory.OnItemRemoved -= OnItemRemoved;
                playerInventory.OnSelectionChanged -= OnSelectionChanged;
                playerInventory.OnInventoryToggle -= OnInventoryToggle;
            }
        }

        private void OnItemAdded(Item item)
        {
            RefreshSlot(item.inventoryPosition);
        }

        private void OnItemRemoved(Item item)
        {
            RefreshSlot(item.inventoryPosition);
        }

        private void OnSelectionChanged(int newPosition)
        {
            UpdateSelectionHighlight(newPosition);
        }

        private void OnInventoryToggle(bool isOpen)
        {
            if (isOpen)
            {
                RefreshAllSlots();
                UpdateSelectionHighlight(playerInventory.CurrentSelectedPosition);
            }
        }

        private void RefreshSlot(int position)
        {
            if (position >= 0 && position < slotUIs.Count && playerInventory != null)
            {
                slotUIs[position].UpdateSlot(playerInventory.GetItem(position));
            }
        }

        private void RefreshAllSlots()
        {
            for (int i = 0; i < slotUIs.Count; i++)
            {
                RefreshSlot(i);
            }
        }

        private void UpdateSelectionHighlight(int newPosition)
        {
            if (lastSelectedPosition >= 0 && lastSelectedPosition < slotUIs.Count)
            {
                slotUIs[lastSelectedPosition].SetSelected(false);
            }

            if (newPosition >= 0 && newPosition < slotUIs.Count)
            {
                slotUIs[newPosition].SetSelected(true);
                lastSelectedPosition = newPosition;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}