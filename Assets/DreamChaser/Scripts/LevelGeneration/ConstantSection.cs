using UnityEngine;
using System.Collections;

/// <summary>
/// Generate Level Sections using a single repeated block prefab
/// </summary>
public class ConstantSection : LevelSection
{
	[Tooltip("The block prefab to use")]
	public GameObject block;
	[Tooltip("How many rows to generate in this section")]
	public int length = 3;
	[Tooltip("Whether this section should repeat forever (in which case ignore the length parameter)")]
	public bool isInfinite = false;

	private int rowCount = 0;

	public override void StartNewBlockRow(float zPos)
	{
		// keep a count of how many rows we've generated
		rowCount++;
	}
	
	public override GameObject GenerateBlock(float xPos, float zPos)
	{
		// instantiate our prefab at the given position
		Vector3 pos = new Vector3(xPos, 0, zPos);
		return Instantiate(block, pos, Quaternion.identity) as GameObject;
	}
	
	public override bool IsCompleted()
	{
		// check if we've generated enough rows yet
		return !isInfinite && rowCount >= length;
	}
	
	public override void Reset()
	{
		rowCount = 0;
	}
}
