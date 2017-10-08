using UnityEngine;
using System.Collections;

/// <summary>
/// Increase the player speed when we reach this section
/// Attach this to the same GameObject as a LevelSection script to increase the difficulty when the player
/// is approaching this section.
/// </summary>
public class SpeedIncreaser : MonoBehaviour 
{
	[Tooltip("Factor to increase the speed by")]
	public float increaseFactor = 1.1f;
	[Tooltip("Message to display")]
	public string message = "Speed Increased!";

	public void OnSectionStarted()
	{
		GameManager.Player.speed *= increaseFactor;
		GameManager.ShowInfo(message);
	}
}
