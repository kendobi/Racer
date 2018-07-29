using UnityEngine;
using System.Collections;
using SimpleTween;

/// <summary>
/// Speed boost powerup
/// </summary>
public class SpeedBoost : Powerup
{
	[Tooltip("Factor by which to increase the player's speed")]
	public float boostAmount = 1.3f;
	[Tooltip("Duration of the speed boost in seconds")]
	public float boostTime = 2.0f;
	[Tooltip("Time to fade up to the new speed")]
	public float fadeInTime = 0.5f;
	[Tooltip("Time to fade back to the original speed")]
	public float fadeOutTime = 1.0f;
	[Tooltip("Amount of credits to award the player for collecting the powerup")]
	public int creditBonus = 500;

	private Tween tween;

	public override void ApplyPowerup ()
	{
		// display a powerup message on screen
		string msg = infoText + " +" + creditBonus;
		GameManager.ShowCountdown(msg, boostTime, fadeInTime);
		// award the player some credits
		GameManager.AwardCredits(creditBonus);

		// animate up the speed of the player, wait for our duration, then animate it back down again.
		PlayerControl player = GameManager.Player;
		tween = SimpleTweener.AddTween(()=>player.SpeedMultiplier, x=>player.SpeedMultiplier=x, boostAmount, fadeInTime).OnCompleted(()=>{
			tween = SimpleTweener.AddTween(()=>player.SpeedMultiplier, x=>player.SpeedMultiplier=x, 1.0f, fadeOutTime).Delay(boostTime).OnCompleted(()=>{
				PowerupManager.OnPowerupCompleted(this);
			});
		});
	}

	public override void Cancel()
	{
		// to avoid conficts (if e.g. we collected a slowmotion powerup while this powerup is still running), we have
		// to remove our tween and reset the player's speed if another powerup is collected.
		if(tween != null)
		{
			SimpleTweener.RemoveTween(tween);
			GameManager.Player.SpeedMultiplier = 1.0f;
			tween = null;
		}
	}
}
