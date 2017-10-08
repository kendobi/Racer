using UnityEngine;
using System.Collections;

// Just an empty class to signify valid powerup spawn positions within a level block.

public class PowerupSpawnPoint : MonoBehaviour 
{
	void OnDrawGizmos()
	{
		// draw a red sphere in the editor so we can see the valid powerup spawn points.
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
