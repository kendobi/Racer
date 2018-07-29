using UnityEngine;
using System.Collections;

// base class for all powerups
public abstract class Powerup : MonoBehaviour 
{
	[Tooltip("An audioclip to play when the powerup is collected")]
	public AudioClip collectedSfx;
	[Tooltip("A message to display when the powerup is collected")]
	public string infoText;
	
	/// <summary>
	/// Called when this powerup is collected
	/// </summary>
	public abstract void ApplyPowerup();

	/// <summary>
	/// If a second powerup is collected before this one has completed Cancel will be called on the first powerup, to
	/// stop them interferring if for example they are both trying to alter the player's speed.
	/// </summary>
	public abstract void Cancel();
}
