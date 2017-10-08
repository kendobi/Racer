using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Shop menu item. This is the base class for things to be displayed in the shop.
/// This expects to find some child elements with particular names, specifically:
/// 	StatusText - A Text element that will display some status text ('owned', 'purchase' 'not enough credits' etc)
/// 	CostText - A Text element that says "Cost:" - the color and visibilty of this item will be set depending on the status.
/// 	CostNumber - A Text element that will be updated with the cost of the item
/// 	DisabledOverlay - An element that will be enabled if the player doesn't have enough credits to buy this item.
/// </summary>
public class ShopMenuItem : MonoBehaviour
{
	[Tooltip("The shop item prefab that this menu item is for")]
	public GameObject itemPrefab;

	protected ShopItem item;		// an instantiated copy of our shop item prefab.

	protected Text statusText;
	protected Text costText;
	protected Text costNumText;
	protected RectTransform disabledOverlay;

	// some static text colors to use
	protected static Color greenText = new Color(0.06f, 0.4f, 0.16f);
	protected static Color blueText = new Color(0.25f, 0.25f, 0.37f);
	protected static Color redText = new Color(0.75f, 0.11f, 0.11f);
	protected static Color greyText = new Color(0.2f, 0.2f, 0.2f);
	
	protected virtual void Awake()
	{
		// instantiate our shop item, so it exists in the scene
		item = Instantiate(itemPrefab).GetComponent<ShopItem>();
		if(item == null)
		{
			Debug.LogError("Shop Item prefab " + itemPrefab.name + " doesn't have a ShopItem subclass attached");
		}

		// get our child text components
		statusText = transform.FindChild("StatusText").GetComponent<Text>();
		costText = transform.FindChild("CostText").GetComponent<Text>();
		costNumText = transform.FindChild("CostNumber").GetComponent<Text>();
		disabledOverlay = transform.FindChild("DisabledOverlay").GetComponent<RectTransform>();

		// set the cost text
		costNumText.text = item.cost.ToString();
	}

	void OnEnable()
	{
		UpdateStatus();
	}

	/// <summary>
	/// Show or hide the text elements based on the inventory state of this item
	/// </summary>
	public virtual void UpdateStatus()
	{
		if(item.IsOwned)
		{
			statusText.text = "OWNED";
			statusText.color = blueText;
			disabledOverlay.gameObject.SetActive(false);

			costText.enabled = false;
			costNumText.enabled = false;
		}
		else
		{
			costText.enabled = true;
			costNumText.enabled = true;
			if(item.CanAfford())
			{
				statusText.text = "PURCHASE";
				statusText.color = greenText;
				disabledOverlay.gameObject.SetActive(false);

				costText.color = greyText;
				costNumText.color = greyText;
			}
			else
			{
				statusText.text = "NOT ENOUGH CREDITS";
				statusText.color = Color.white;
				disabledOverlay.gameObject.SetActive(true);

				costText.color = redText;
				costNumText.color = redText;
			}
		}
	}
	
	public virtual void OnPressed()
	{
		if(!item.IsInventoryFull())
		{
			if(item.TryPurchase())
				SoundManager.PlaySfx("ButtonPress");
			else
				SoundManager.PlaySfx("ButtonInvalid");
		}
		else
		{
			item.OnActivated();
			SoundManager.PlaySfx("ButtonPress");
		}

		// inform the store menu, so it can update the available credits etc.
		GetComponentInParent<StoreMenu>().RefreshItemStatus();
	}
}
