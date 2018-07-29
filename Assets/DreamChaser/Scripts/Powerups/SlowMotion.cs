using UnityEngine;
using System.Collections;
using SimpleTween;

/// <summary>
/// Slow motion powerup
/// </summary>
public class SlowMotion : Powerup
{
	[Tooltip("Factor by which to slow down time")]
	public float speedFactor = 0.4f;
	[Tooltip("Duration of the effect")]
	public float duration = 2.0f;
	[Tooltip("Fade in/out time for the effect")]
	public float fadeTime = 0.4f;
	[Tooltip("Amount of credits to award the player for collecting the powerup")]
	public int creditBonus = 200;

	private Tween tween;

	public override void ApplyPowerup ()
	{
		// display a powerup message on screen
		string msg = infoText + " +" + creditBonus;
		GameManager.ShowCountdown(msg, duration, fadeTime);
		// award the player some credits
		GameManager.AwardCredits(creditBonus);

		// animate down the speed of the player, wait for our duration, then animate it back up again.
		PlayerControl player = GameManager.Player;
		tween = SimpleTweener.AddTween(()=>player.SpeedMultiplier, x=>player.SpeedMultiplier=x, speedFactor, fadeTime).OnCompleted(()=>{
			tween = SimpleTweener.AddTween(()=>player.SpeedMultiplier, x=>player.SpeedMultiplier=x, 1.0f, fadeTime).Delay(duration).OnCompleted(()=>{
				PowerupManager.OnPowerupCompleted(this);
			});
		});
	}

	public override void Cancel()
	{
		// to avoid conficts (if e.g. we collected a speedboost powerup while this powerup is still running), we have
		// to remove our tween and reset the player's speed if another powerup is collected.
		if(tween != null)
		{
			SimpleTweener.RemoveTween(tween);
			GameManager.Player.SpeedMultiplier = 1.0f;
			tween = null;
		}
	}
}
