using UnityEngine;
using System.Collections;

/// <summary>
/// Powerup manager. Handles activating and canceling powerups.
/// There should be exactly one of these in the scene.
/// </summary>
public class PowerupManager : MonoBehaviour 
{
	private Powerup activePowerup;
	private static PowerupManager instance;

	void Awake () 
	{
		instance = this;
		activePowerup = null;
	}

	/// <summary>
	/// Gets the active powerup (if there is one)
	/// </summary>
	/// <value>The active powerup (or null if there is no powerup currently active)</value>
	public static Powerup ActivePowerup
	{
		get { return instance.activePowerup; }
	}

	public static void CancelAllPowerups()
	{
		instance.Reset();
	}

	public static void OnPowerupCollected(Powerup powerup)
	{
		instance.ApplyPowerup(powerup);
	}

	public static void OnPowerupCompleted(Powerup powerup)
	{
		instance.DeletePowerup(powerup);
	}

	private void ApplyPowerup(Powerup powerup)
	{
		if(activePowerup == powerup)
			return;

		// cancel any previously running powerups.
		if(activePowerup != null)
		{
			activePowerup.Cancel();
			Destroy(activePowerup.gameObject);
		}

		SoundManager.PlaySfx(powerup.collectedSfx);

		activePowerup = powerup;
		powerup.ApplyPowerup();
	}

	private void DeletePowerup(Powerup powerup)
	{
		if(activePowerup == powerup)
		{
			activePowerup = null;
		}
		Destroy(powerup.gameObject);
	}

	private void Reset()
	{
		// Cancel the active powerup if there is one.
		if(activePowerup != null)
		{
			activePowerup.Cancel();
			Destroy(activePowerup.gameObject);
			
			activePowerup = null;
		}
	}
}
