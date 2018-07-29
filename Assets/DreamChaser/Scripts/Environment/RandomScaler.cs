using UnityEngine;
using System.Collections;

/// <summary>
/// Give an object a random scale - used to quickly get a lot a variation out of the same models
/// </summary>
public class RandomScaler : MonoBehaviour 
{
	[Tooltip("The minimum scale value to use for this object")]
	public Vector3 minScale = Vector3.one;
	[Tooltip("The maximum scale value to use for this object")]
	public Vector3 maxScale = Vector3.one;

	void Start () 
	{
		Vector3 scale;
		scale.x = Mathf.Lerp (minScale.x, maxScale.x, Random.value);
		scale.y = Mathf.Lerp (minScale.y, maxScale.y, Random.value);
		scale.z = Mathf.Lerp (minScale.z, maxScale.z, Random.value);
		transform.localScale = scale;
	}	
}
