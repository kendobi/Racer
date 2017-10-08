using UnityEngine;
using System.Collections;

/// <summary>
/// Show a message when a LevelSection is approaching
/// Attach this to the same GameObject as a LevelSection script to show an on screen message
/// when that section is approaching.
/// </summary>
public class SectionMessage : MonoBehaviour 
{
	[Tooltip("The message to display when this section is approaching")]
	public string message;

	public void OnSectionStarted()
	{
		// display a message when we're approaching this level section.
		// delay showing it slightly until we've almost reached it.
		Invoke ("ShowMessage", 1.0f);
	}

	private void ShowMessage()
	{
		GameManager.ShowInfo(message);
	}
}
