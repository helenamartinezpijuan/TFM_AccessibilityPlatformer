using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using PlatformerGame.Inventory;

namespace PlatformerGame.Managers
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        
        [Header("Inventory Settings")]
        [SerializeField] private bool persistBetweenScenes = true;
        
        private List<Item> savedItems = new List<Item>();
        private Dictionary<string, object> gameState = new Dictionary<string, object>();
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                if (persistBetweenScenes)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            Debug.Log("Inventory Manager initialized");
        }
        
        public void InitializeNewGame()
        {
            savedItems.Clear();
            gameState.Clear();
            
            Debug.Log("New game inventory initialized");
        }
        
        public void SaveInventory()
        {
            // Get current inventory if it exists
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            if (inventory != null)
            {
                savedItems.Clear();
                savedItems.AddRange(inventory.Items);
                
                // Save to PlayerPrefs
                SaveToPlayerPrefs();
                
                Debug.Log($"Inventory saved with {savedItems.Count} items");
            }
        }
        
        public void LoadInventory()
        {
            // Load from PlayerPrefs
            LoadFromPlayerPrefs();
            
            // Apply to current inventory
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            if (inventory != null)
            {
                // Clear current inventory
                for (int i = 0; i < inventory.InventorySize; i++)
                {
                    inventory.RemoveItem(i);
                }
                
                // Add saved items
                foreach (Item item in savedItems)
                {
                    if (item != null)
                    {
                        inventory.AddItem(item);
                    }
                }
                
                Debug.Log($"Inventory loaded with {savedItems.Count} items");
            }
        }
        
        private void SaveToPlayerPrefs()
        {
            // Save item count
            PlayerPrefs.SetInt("InventoryItemCount", savedItems.Count);
            
            // Save each item (simplified - you'd need a better serialization system)
            for (int i = 0; i < savedItems.Count; i++)
            {
                if (savedItems[i] != null)
                {
                    PlayerPrefs.SetString($"InventoryItem_{i}", savedItems[i].itemName);
                }
            }
            
            PlayerPrefs.Save();
        }
        
        private void LoadFromPlayerPrefs()
        {
            savedItems.Clear();
            
            int itemCount = PlayerPrefs.GetInt("InventoryItemCount", 0);
            
            for (int i = 0; i < itemCount; i++)
            {
                string itemName = PlayerPrefs.GetString($"InventoryItem_{i}", "");
                if (!string.IsNullOrEmpty(itemName))
                {
                    // You'll need an Item database or resource loading system here
                    // For now, we'll just note that we need to load these
                    Debug.Log($"Need to load item: {itemName}");
                    
                    // In a real implementation, you'd load the Item ScriptableObject
                    // Item item = Resources.Load<Item>($"Items/{itemName}");
                    // if (item != null) savedItems.Add(item);
                }
            }
        }
        
        public void ClearSavedInventory()
        {
            savedItems.Clear();
            
            // Clear PlayerPrefs
            PlayerPrefs.DeleteKey("InventoryItemCount");
            
            // Clear all inventory items
            for (int i = 0; i < 100; i++) // Reasonable upper limit
            {
                if (PlayerPrefs.HasKey($"InventoryItem_{i}"))
                {
                    PlayerPrefs.DeleteKey($"InventoryItem_{i}");
                }
                else
                {
                    break;
                }
            }
            
            PlayerPrefs.Save();
            Debug.Log("Saved inventory cleared");
        }
        
        public void SaveGameState(string key, object value)
        {
            gameState[key] = value;
            
            // Also save to PlayerPrefs if it's a basic type
            if (value is int intValue)
                PlayerPrefs.SetInt(key, intValue);
            else if (value is float floatValue)
                PlayerPrefs.SetFloat(key, floatValue);
            else if (value is string stringValue)
                PlayerPrefs.SetString(key, stringValue);
            
            PlayerPrefs.Save();
        }
        
        public T LoadGameState<T>(string key, T defaultValue = default)
        {
            if (gameState.ContainsKey(key))
                return (T)gameState[key];
            
            // Try loading from PlayerPrefs
            if (typeof(T) == typeof(int))
                return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
            else if (typeof(T) == typeof(float))
                return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
            else if (typeof(T) == typeof(string))
                return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
            
            return defaultValue;
        }
        
        // Called when scene changes
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene loaded: {scene.name}");
            
            // Don't load inventory in main menu
            if (scene.name == "MainMenu") return;
            
            // Check if this is a new game
            bool isNewGame = PlayerPrefs.GetInt("IsNewGame", 1) == 1;
            
            if (!isNewGame)
            {
                LoadInventory();
            }
        }
    }
}