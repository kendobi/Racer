using UnityEngine;
using System.Collections;

/// <summary>
/// Generate rows of blocks in a sequence
/// </summary>
public class SectionSequence : LevelSection
{
	[Tooltip("The blocks to use in the sequence (in order)")]
	public GameObject[] blockSequence; 
	[Tooltip("How many rows of each block to generate")]
	public int rowsPerBlock = 3;
	[Tooltip("How many times the whole sequence should repeat")]
	public int repeats = 1;

	private int sequenceIndex = 0;		// which block in the sequence are we currently at
	private int blockCount = 0;			// how many rows of that block have we generated
	private int loops = 0;				// how many loops of the whole sequence have we done

	public override void StartNewBlockRow(float zPos)
	{
		blockCount++;
		if(blockCount == rowsPerBlock)
		{
			// move to the next block in the sequence
			blockCount = 0;
			sequenceIndex++;

			if(sequenceIndex == blockSequence.Length)
			{
				// repeat the whole sequence
				sequenceIndex = 0;
				loops++;
			}
		}
	}
	
	public override GameObject GenerateBlock(float xPos, float zPos)
	{
		// generate a new block from our sequence
		Vector3 pos = new Vector3(xPos, 0, zPos);
		return Instantiate(blockSequence[sequenceIndex], pos, Quaternion.identity) as GameObject;
	}
	
	public override bool IsCompleted()
	{
		// once we've repeated the whole sequence the specified number of times, we're done
		return loops >= repeats;
	}
	
	public override void Reset()
	{
		sequenceIndex = 0;
		blockCount = 0;
		loops = 0;
	}
}
