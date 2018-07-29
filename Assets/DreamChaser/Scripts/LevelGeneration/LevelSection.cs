using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for Level Section Generators
/// Subclasses of this will generate pieces of the level as the player approaches them
/// It could pick blocks randomly from a set, or generate them in a fixed sequence or anything it likes
/// </summary>
public abstract class LevelSection : MonoBehaviour 
{
	// a reference to the PowerupSpawner component attached to this section (if there is one)
	protected PowerupSpawner powerupSpawner;

	public PowerupSpawner PowerupSpawner
	{
		get { return powerupSpawner; }
	}

	void Start()
	{
		// store the PowerupSpawner component attached to this section (if there is one)
		powerupSpawner = GetComponent<PowerupSpawner>();
	}

	/// <summary>
	/// Called when the level manager starts generating a new row of blocks
	/// </summary>
	/// <param name="zPos">Z position of the new row</param>
	public abstract void StartNewBlockRow(float zPos);

	/// <summary>
	/// Generates a block in the current row at the given position
	/// This is the meat of this class, where the block pieces that make up the world are picked
	/// </summary>
	/// <returns>A new block</returns>
	/// <param name="xPos">X position to generate at</param>
	/// <param name="zPos">Z position to generate at</param>
	public abstract GameObject GenerateBlock(float xPos, float zPos);

	/// <summary>
	/// Determine whether this section has completed generating, in which case the LevelManager will move
	/// onto the next LevelSection
	/// </summary>
	/// <returns><c>true</c> if this section has generated all its rows; otherwise, <c>false</c>.</returns>
	public abstract bool IsCompleted();

	/// <summary>
	/// Reset this section so it is ready to start generating again
	/// </summary>
	public abstract void Reset();
}
