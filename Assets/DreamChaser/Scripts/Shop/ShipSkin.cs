using UnityEngine;
using System.Collections;

/// <summary>
/// A purchasable item that gives the player a new ship skin to fly.
/// </summary>
public class ShipSkin : ShopItem
{
	[Tooltip("The player ship model prefab to use")]
	public GameObject modelPrefab;

	public override void OnActivated()
	{
		SetAsActiveSkin();
	}
	
	public override void Apply()
	{
		// if this is the currently selected skin, then send our model prefab to the player
		if(IsActiveSkin())
		{
			GameManager.Player.SetShipModel(modelPrefab);
		}
	}
	
	public bool IsActiveSkin()
	{
		// retreive the active skin id from the player prefs, and check it against our own id.
		return PlayerPrefs.GetString("ActiveSkin", "") == itemID;
	}
	
	private void SetAsActiveSkin()
	{
		// store this skin in the player prefs as the active skin.
		PlayerPrefs.SetString("ActiveSkin", itemID);
	}
}