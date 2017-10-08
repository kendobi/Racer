using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Level manager. This class is responsible for generating the sequence of blocks that make up
/// the level, and scrolling them past the player to simulate the player motion.
/// The world is made from LevelSection objects, that are each responsible for generating a section of blocks.
/// These LevelSections should be children of the LevelManager, ordered as the sections should appear in the world.
/// </summary>
public class LevelManager : MonoBehaviour 
{
	[Tooltip("Index of the level section to start at (useful for testing later sections)")]
	public int startSectionIndex = 1;
	[Tooltip("Distance to the horizon in meters")]
	public float horizonDist = 300.0f;
	[Tooltip("Width across the horizon in meters")]
	public float horizonWidth = 300.0f;

	public const float kBlockSize = 64.0f; 		// size of one block tile in x,z dimensions

	private List<GameObject> blocks = new List<GameObject>();	// the list of blocks that are currently 'live'

	private float currentScrollPos = 0.0f;		// our current world scroll position, used to know when we need to generate more rows.
	private float currentScrollX = 0.0f;		// our current x scroll position, this changes as the player moves left/right

	private LevelSection[] levelSections;		// the list of all the level sections in the world
	private int currentSection = 0;				// the current section we are generating rows from

	private float totalDistance;				// the total distance the player has advanced

	public float TotalDistance
	{
		get { return totalDistance; }
		set { totalDistance = value; }
	}
	
	void Start()
	{
		// get the level sections that make up the world
		levelSections = GetComponentsInChildren<LevelSection>();

		// make the fog opaque at the horizon distance, to hide the new rows being generated.
		RenderSettings.fogEndDistance = horizonDist;
	}

	/// <summary>
	/// Reset the level, ready for a new game
	/// </summary>
	public void Reset()
	{
		currentSection = 0;
		currentScrollPos = 0.0f;
		currentScrollX = 0.0f;
		totalDistance = 0.0f;

		// reset each section
		if(levelSections != null)
		{
			for(int i=0; i<levelSections.Length; ++i)
				levelSections[i].Reset();
		}

		// destroy any blocks left over from the previous game
		for(int i=0; i<blocks.Count; ++i)
			Destroy(blocks[i]);
		blocks.Clear();
	}

	public void StartGame()
	{
		Reset ();
		SetCurrentSectionIndex(startSectionIndex);
	}
	
	/// <summary>
	/// Generates a single row of scenery blocks.
	/// </summary>
	private void GenerateBlocks()
	{
		LevelSection levelSection = levelSections[currentSection];

		// calculate the number of blocks we need to generate to fill a row
		int numBlocksRadius = Mathf.CeilToInt(horizonWidth / kBlockSize) / 2;
		int numBlocksAcross = 2 * numBlocksRadius + 1;

		// get the current LevelSection to generate the blocks for this row
		levelSection.StartNewBlockRow(currentScrollPos);
		for(int x=-numBlocksRadius; x<=numBlocksRadius; ++x)
		{
			// generate a new block
			GameObject block = levelSection.GenerateBlock(currentScrollX + x * kBlockSize, currentScrollPos);

			// potentially spawn some powerups within the block
			if(levelSection.PowerupSpawner != null)
				levelSection.PowerupSpawner.SpawnPowerups(block);

			// add the block to our live list
			blocks.Add(block);
		}

		// check if the current section has now generated all its rows
		if(levelSection.IsCompleted())
		{
			levelSection.Reset();

			// move on to the next section, looping around when we reach the end
			int nextSectionIdx = currentSection + 1;
			if(nextSectionIdx == levelSections.Length)
				nextSectionIdx = startSectionIndex;

			SetCurrentSectionIndex(nextSectionIdx);
		}

		// update our scroll position by the size of one row
		currentScrollPos += kBlockSize;

		// remove any blocks that have moved behind the camera
		int blockRows = Mathf.CeilToInt(0.5f + (horizonDist  / kBlockSize));
		while(blocks.Count > numBlocksAcross * blockRows)
		{
			Destroy(blocks[0]);
			blocks.RemoveAt(0);
		}
	}

	private void SetCurrentSectionIndex(int idx)
	{
		currentSection = idx;

		// announce to any scripts that are listening (on the LevelSection object) that we're about to start generating this section.
		levelSections[currentSection].gameObject.SendMessage("OnSectionStarted", SendMessageOptions.DontRequireReceiver);
	}

	void Update () 
	{
		// make sure there are enough blocks to reach the horizon
		while(currentScrollPos < horizonDist)
			GenerateBlocks();

		// get the speeds to scroll the level forwards and sideways
		float zSpeed = 0.0f;
		float xSpeed = 0.0f;
		if(GameManager.CurrentState == GameManager.GameState.Playing)
		{
			zSpeed = GameManager.Player.Speed;
			xSpeed = GameManager.Player.Steer;
		}
		else if(GameManager.CurrentState == GameManager.GameState.InMenus)
		{
			zSpeed = 20.0f; // default scroll speed for when we're in the menus
		}

		// scroll all the blocks to give the impression the player is moving
		float zDelta = zSpeed * Time.deltaTime;
		float xDelta = xSpeed * Time.deltaTime;
		Vector3 posDelta = new Vector3(xDelta, 0.0f, zDelta);
		for(int i=0; i<blocks.Count; ++i)
		{
			GameObject block = blocks[i];
			Vector3 pos = block.transform.position;
			pos -= posDelta;	// negative because we're moving the scenery, not the player.
			block.transform.position = pos;
		}

		// update our current scroll position
		currentScrollPos -= zDelta;
		currentScrollX = Mathf.Repeat(currentScrollX - xDelta, kBlockSize);
		totalDistance += zDelta;
	}
}
