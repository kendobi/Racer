using UnityEngine;
using System.Collections;

/// <summary>
/// Generates Level Sections by picking at random from a set of blocks
/// </summary>
public class RandomSection : LevelSection
{
	[Tooltip("The set of block prefabs to pick from")]
	public GameObject[] blocks;
	[Tooltip("The number of rows to generate in this section")]
	public int length = 10;
	public bool randomizeXPosition = true;

	private int rowCount = 0;			// the number of rows we have generated so far
	private float rowXOffset = 0.0f;	// a random x-offset to apply to the rows
	
	public override void StartNewBlockRow(float zPos)
	{
		if(randomizeXPosition)
		{
			// pick a new random x-offset for this row.
			// this is in steps of 1/16th of a block size, because the artwork fits with that, but it could be any value really.
			rowXOffset = LevelManager.kBlockSize * (Random.Range(-8, 8) / 16.0f);
		}
		else
		{
			rowXOffset = 0.0f;
		}

		rowCount++;
	}

	public override GameObject GenerateBlock(float xPos, float zPos)
	{
		// pick a block at random from our set
		int idx = Random.Range(0, blocks.Length);
		// instantiate a new prefab 
		Vector3 pos = new Vector3(xPos + rowXOffset, 0, zPos);
		GameObject block = Instantiate(blocks[idx], pos, Quaternion.identity) as GameObject;

		return block;
	}

	public override bool IsCompleted()
	{
		// once we've generated enough rows this section is complete
		return rowCount >= length;
	}

	public override void Reset()
	{
		// reset the row count, ready to generate again
		rowCount = 0;
	}
}
