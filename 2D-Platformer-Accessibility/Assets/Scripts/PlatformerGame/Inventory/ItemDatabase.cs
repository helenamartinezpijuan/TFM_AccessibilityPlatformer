using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "PlatformerGame/Inventory/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public List<Item> allItems = new List<Item>();
        
        private Dictionary<string, Item> itemDictionary;
        
        public void Initialize()
        {
            itemDictionary = new Dictionary<string, Item>();
            foreach (var item in allItems)
            {
                if (item != null && !string.IsNullOrEmpty(item.itemName))
                {
                    itemDictionary[item.itemName] = item;
                }
            }
        }
        
        public Item GetItemByName(string itemName)
        {
            if (itemDictionary == null) Initialize();
            
            if (itemDictionary.TryGetValue(itemName, out Item item))
            {
                return item;
            }
            
            Debug.LogWarning($"Item '{itemName}' not found in database");
            return null;
        }
    }
}