using UnityEngine;
using System.Collections;

/// <summary>
/// A purchasable item that multiplies the value of any credits collected during the race by a given amount
/// </summary>
public class CreditMultiplier : ShopItem
{
	[Tooltip("The factor to increase the value of collected credits by")]
	public float creditMultiplier = 2.0f;

	public override void Apply()
	{
		GameManager.SetCreditMultiplier(creditMultiplier);
	}
}