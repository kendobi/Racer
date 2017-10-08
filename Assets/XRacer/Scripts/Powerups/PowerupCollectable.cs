using UnityEngine;
using System.Collections;

/// <summary>
/// Powerup collectable. This is the object that is spawned in the world which the player can collect. It should contain
/// a child object with a subclass of the Powerup script attached, plus any sort of visual representation.
/// </summary>
public class PowerupCollectable : MonoBehaviour 
{
	private Powerup powerup;		// the powerup that will be activated if this object is colided with by the player.

	void Start()
	{
		// find our child Powerup object.
		powerup = GetComponentInChildren<Powerup>();

		// set our layer so we're not considered to be an obstacle.
		gameObject.layer = LayerMask.NameToLayer("Powerup");
	}

	public void OnCollected()
	{
		// reparent the powerup item so it doesn't get destroyed along with this object (when it scrolls out of view).
		powerup.transform.parent = null;

		// activate the powerup
		PowerupManager.OnPowerupCollected(powerup);

		// destroy the visual powerup ring since it has now been collected
		Destroy(gameObject);
	}
}
