using UnityEngine;
using System.Collections;

/// <summary>
/// This class is responsible for randomly spawning powerups inside a LevelSection.
/// It should be attached to the same object that has the LevelSection script.
/// </summary>
public class PowerupSpawner : MonoBehaviour 
{
	[Tooltip("Probability of generating a powerup within a level section block")]
	[Range(0.0f, 1.0f)]
	public float powerupFrequency = 0.25f;
	[Tooltip("The set of Powerup prefabs that can potentially be spawned within this section")]
	public GameObject[] powerupPrefabs;

	/// <summary>
	/// Spawn powerups within a given level section
	/// </summary>
	public void SpawnPowerups(GameObject levelSection)
	{
		// First find the available spawnpoints within this section. This ensures that we never create powerups
		// in places that are impossible for the player.
		PowerupSpawnPoint[] spawnPoints = levelSection.GetComponentsInChildren<PowerupSpawnPoint>();

		// Check that we have at least one spawn point to pick from
		if(spawnPoints.Length == 0)
		{
			Debug.LogWarning("Attempting to spawn powerup, but no spawn points found in block " + levelSection.name);
			return;
		}

		// generate a powerup with our given probability
		if(Random.value < powerupFrequency)
		{
			// pick a spawn point at random
			Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
			// pick a powerup prefab at random, and instantiate it at the spawn point.
			GameObject powerup = Instantiate(powerupPrefabs[Random.Range(0, powerupPrefabs.Length)],
			                                 spawnPoint.position, spawnPoint.rotation) as GameObject;

			// parent the powerup to the spawn point so it scrolls along with the rest of the level sections.
			powerup.transform.parent = spawnPoint;
		}
	}
}
