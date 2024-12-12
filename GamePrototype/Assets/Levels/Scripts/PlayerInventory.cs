using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ethan added this script
// Script to manage player inventory

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int quantity;
        public Sprite itemIcon;
    }

    // List to store inventory items
    public List<InventoryItem> items = new List<InventoryItem>();

    // Maximum inventory capacity
    public int maxInventorySlots = 10;

    // Add item to inventory
    public bool AddItem(string itemName, int quantity, Sprite itemIcon = null)
    {
        // Check if inventory is full
        if (items.Count >= maxInventorySlots)
        {
            Debug.Log($"Inventory is full!");
            return false;
        }

        // Check if item already exists in inventory
        InventoryItem existingItem = items.Find(item => item.itemName == itemName);

        if (existingItem != null)
        {
            // Update quanity if item exists
            existingItem.quantity += quantity;
        }
        else
        {
            // Create new inventory item
            InventoryItem newItem = new InventoryItem();
            {
                itemName = itemName;
                quantity = quantity;
                itemIcon = itemIcon;
            }
            items.Add(newItem);
        }
        return true;

    }

    // Remove item from inventory
    public bool RemoveItem(string itemName, int quantity) 
    {
            InventoryItem existingItem = items.Find(item => item.itemName == itemName);

            if (existingItem != null)
            {
                if (existingItem.quantity >= quantity)
                {
                    existingItem.quantity -= quantity;

                    // Remove item if quantity is 0
                    if (existingItem.quantity == 0)
                    {
                        items.Remove(existingItem);
                    }
                    return true;
                }
                else
                {
                    Debug.Log($"Not enough {itemName} in inventory!");
                    return false;
                }

            }
            Debug.Log($"{itemName} not found in inventory!");
            return false;
    }

    // Check item quantity
   int GetItemQuantity(string itemName)
   {
      InventoryItem existingItem = items.Find(item => item.itemName == itemName);
      return existingItem != null ? existingItem.quantity : 0;
   }

        // Print inventory contents
   void PrintInventory()
   {
      Debug.Log("----- Inventory Contents -----");
      foreach (var item in items)
      {
         Debug.Log($"{item.itemName}: {item.quantity}");
      }
   }
}




