using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary;
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }
    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach(var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot) // slot value -> the backend inventory slot
            {
                slot.Key.UpdateUISlot(updatedSlot); // slot key -> UI representation of the value
            }
        }
    }

    public void SlotClicked(InventorySlot_UI clickedSlot)
    {
        Debug.Log($"{ clickedSlot.AssignedInventorySlot.ItemData.DisplayName} clicked!"); 
    }

}
