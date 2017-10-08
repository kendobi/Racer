using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Store menu screen.
/// This script expects to find a child Text element name CreditText, which will be updated to display the
/// amount of credits the player has.
/// </summary>
public class StoreMenu : MenuScreen
{
	private Text availableFunds;

	protected override void Awake()
	{
		base.Awake();
		
		availableFunds = transform.FindChild("CreditText").GetComponent<Text>();
	}

	public override void Show(float fade, TransitionDirection direction, SimpleTween.Callback callback)
	{
		availableFunds.text = GameManager.TotalCredits.ToString();

		base.Show(fade, direction, callback);
	}

	public void ExitStore()
	{
		SoundManager.PlaySfx("ButtonPress");
		GameManager.MenuSystem.GoBack();
	}

	public void RefreshItemStatus()
	{
		// update the available funds display
		availableFunds.text = GameManager.TotalCredits.ToString();

		// also refresh all the items, so their status is consistent with the inventory.
		ShopMenuItem[] items = GetComponentsInChildren<ShopMenuItem>();
		for(int i=0; i<items.Length; ++i)
		{
			items[i].UpdateStatus();
		}
	}
}
