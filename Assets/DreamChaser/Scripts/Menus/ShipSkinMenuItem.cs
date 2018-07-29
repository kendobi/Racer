using UnityEngine;
using System.Collections;

/// <summary>
/// Subclass of ShopMenuItem for ship skin purchasables, which also indicates which skin is currently active.
/// </summary>
public class ShipSkinMenuItem : ShopMenuItem
{
	private ShipSkin skin;

	protected override void Awake()
	{
		base.Awake();
		skin = item as ShipSkin;
	}

	public override void UpdateStatus ()
	{
		base.UpdateStatus();

		// if this is the currently active skin, then change our status text to say so.
		if(skin && skin.IsActiveSkin())
		{
			statusText.text = "ACTIVE";
		}
	}
}
