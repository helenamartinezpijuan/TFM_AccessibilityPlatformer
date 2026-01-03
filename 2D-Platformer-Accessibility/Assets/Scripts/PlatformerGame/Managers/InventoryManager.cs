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
        [SerializeField] private ItemDatabase itemDatabase;
        
        //private List<Item> savedItems = new List<Item>();
        private List<string> savedItemNames = new List<string>();
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

            // Initialize item database if assigned
            if (itemDatabase != null)
            {
                itemDatabase.Initialize();
            }
            
            Debug.Log("Inventory Manager initialized");
            LoadSavedItemNames();
        }
        
        /*public void InitializeNewGame()
        {
            savedItems.Clear();
            gameState.Clear();
            
            Debug.Log("New game inventory initialized");
        }
        
        public void SaveInventory()
        {
            // Get current inventory if it exists
            //PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                savedItems.Clear();
                savedItems.AddRange(inventory.Items);
                
                // Save to PlayerPrefs
                SaveToPlayerPrefs();
                
                Debug.Log($"Inventory saved with {savedItems.Count} items");
            }
        }*/
        
        /*public void LoadInventory()
        {
            // Load from PlayerPrefs
            LoadFromPlayerPrefs();
            
            // Apply to current inventory
            //PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
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
        }*/

        #region PlayerPrefs Logic

        private void LoadSavedItemNames()
        {
            savedItemNames.Clear();
            
            int itemCount = PlayerPrefs.GetInt("InventoryItemCount", 0);
            
            for (int i = 0; i < itemCount; i++)
            {
                string itemName = PlayerPrefs.GetString($"InventoryItem_{i}", "");
                if (!string.IsNullOrEmpty(itemName))
                {
                    savedItemNames.Add(itemName);
                }
            }
            
            Debug.Log($"Loaded {savedItemNames.Count} item names from save");
        }
        
        // NEW: Get saved item names
        public List<string> GetSavedItemNames()
        {
            return new List<string>(savedItemNames);
        }
        
        // NEW: Save current inventory
        public void SaveCurrentInventory(PlayerInventory inventory)
        {
            if (inventory == null) return;
            
            savedItemNames.Clear();
            
            // Save item names
            foreach (Item item in inventory.Items)
            {
                if (item != null)
                {
                    savedItemNames.Add(item.itemName);
                }
            }
            
            // Save to PlayerPrefs
            SaveToPlayerPrefs();
            
            Debug.Log($"Inventory saved with {savedItemNames.Count} items");
        }
        
        // NEW: Clear current save
        public void ClearCurrentSave()
        {
            savedItemNames.Clear();
            SaveToPlayerPrefs();
        }
        
        private void SaveToPlayerPrefs()
        {
            // Save item count
            PlayerPrefs.SetInt("InventoryItemCount", savedItemNames.Count);
            
            // Save each item name
            for (int i = 0; i < savedItemNames.Count; i++)
            {
                PlayerPrefs.SetString($"InventoryItem_{i}", savedItemNames[i]);
            }
            
            PlayerPrefs.Save();
        }
        
        /*private void LoadFromPlayerPrefs()
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
                    Item item = Resources.Load<Item>($"Items/{itemName}");
                    if (item != null) savedItems.Add(item);
                }
            }
        }*/
        
        public void ClearSavedInventory()
        {
            savedItemNames.Clear();
            
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
        #endregion
        
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
            if (scene.name == "MainMenu" || scene.buildIndex == 0) return;
            
            // Find player inventory in the new scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    // The PlayerInventory will load from savedItemNames in its Awake()
                    Debug.Log("Player inventory found in new scene");
                }
            }
        }
    }
}