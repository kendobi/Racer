using UnityEngine;
using System.Collections;

/// <summary>
/// Simple class to scroll the UVs across a model
/// </summary>
public class UVScroller : MonoBehaviour 
{
	[Tooltip("Speed to scroll in the X direction")]
	public float xSpeed = 1.0f;
	[Tooltip("Speed to scroll in the Y direction")]
	public float ySpeed = 0.0f;

	private float x = 0.0f;
	private float y = 0.0f;
	private Material material;

	void Start()
	{
		material = GetComponent<Renderer>().material;
	}

	void Update () 
	{
		// update our uv offset values
		x = Mathf.Repeat(x + Time.deltaTime * xSpeed, 1.0f);
		y = Mathf.Repeat(y + Time.deltaTime * ySpeed, 1.0f);

		// set the offset on the material
		material.mainTextureOffset = new Vector2(x,y);
	}
}
