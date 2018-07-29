using UnityEngine;
using System.Collections;

/// <summary>
/// Bonus credits powerup. Awards the player a specified number of credits when they collect this powerup.
/// </summary>
public class BonusCredits : Powerup 
{
	[Tooltip("The amount of credits to award")]
	public int bonusAmount = 500;

	public override void ApplyPowerup ()
	{
		// award the credits to the player
		GameManager.AwardCredits(bonusAmount);
		// display an on screen message
		GameManager.ShowInfo(infoText + " +" + bonusAmount);

		// nothing else to do.
		PowerupManager.OnPowerupCompleted(this);
	}

	public override void Cancel()
	{
	}
}
