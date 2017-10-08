using UnityEngine;
using System.Collections;

/// <summary>
/// Base-class for purchasable items
/// </summary>
public class ShopItem : MonoBehaviour 
{
	[Tooltip("Unique id for this item-type, make sure this is unique amonst all shop items")]
	public string itemID = "UniqueID";
	[Tooltip("Price of this item in credits")]
	public int cost = 1000;
	[Tooltip("Maximum number of this item that the player can have in their inventory")]
	public int maxInventoryCount = 1;
	[Tooltip("Does the inventory count decrease when this item is 'used'")]
	public bool isConsumable = false;
	[Tooltip("Should the player own this item at the start of the game (i.e. without purchasing it)")]
	public bool ownByDefault = false;

	/// <summary>
	/// Get the inventory count for this item. 
	/// Note that we use the default PlayerPrefs class for storing what items the player owns, you may want to
	/// alter this to use a more secure method (or encrypt the values), especially if real money is involved.
	/// </summary>
	public int Count
	{
		get { return PlayerPrefs.GetInt(itemID, 0); }
		private set { PlayerPrefs.SetInt(itemID, value); }
	}

	public bool IsOwned
	{
		get { return Count > 0; }
	}

	/// <summary>
	/// Do we have enough credits to purchase this item
	/// </summary>
	public bool CanAfford()
	{
		return GameManager.TotalCredits >= cost;
	}

	/// <summary>
	/// Check if we have space in our inventory for another instance of this item
	/// </summary>
	public bool IsInventoryFull()
	{
		return Count >= maxInventoryCount;
	}

	/// <summary>
	/// Add this item to the inventory and debit the player's credits by the cost.
	/// If the player doesn't have sufficient funds, or they already own the maximum amount, do nothing.
	/// </summary>
	/// <returns><c>true</c>, if purchase was succefully made, <c>false</c> otherwise.</returns>
	public bool TryPurchase()
	{
		if(!CanAfford() || IsInventoryFull())
			return false;

		// debit funds and increase the inventory count
		GameManager.TotalCredits -= cost;
		Count++;

		OnActivated();
		return true;
	}

	/// <summary>
	/// Removes this item from the inventory.
	/// </summary>
	public void RemoveFromInventory()
	{
		Count = 0;
	}

	/// <summary>
	/// Called from the UI when this item is tapped, having already been previously purchased
	/// </summary>
	public virtual void OnActivated()
	{
	}

	/// <summary>
	/// Called at the start of a game if this item has a non-zero count in the player's inventory
	/// </summary>
	public virtual void Apply()
	{
		// override this to implement your own shop-items
	}

	public void OnGameStarted()
	{
		// at the start of the game, check if the player owns this item, and if so apply its effects.

		// check if the item should be owned by default, and award it if it's not in the inventory
		if(ownByDefault && Count == 0)
			Count++;

		if(Count > 0)
		{
			Apply();
			if(isConsumable)
				Count--;
		}
	}
}
