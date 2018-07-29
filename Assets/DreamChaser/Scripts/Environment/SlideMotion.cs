using UnityEngine;
using System.Collections;

/// <summary>
/// Give an object some motion by sliding between two positions
/// </summary>
public class SlideMotion : MonoBehaviour
{
	public Vector3 offset = Vector3.up;		// the offset from the starting position to the end position
	public float cycleTime = 4.0f;			// the time in seconds for one there-and-back-again cycle
	// a curve which defines the interpolation between the start and end positions over one cycle. usually
	// this will start at 0, increase to 1, and then return to 0 (making the object move from the 
	// starting position to the end position and back again).
	public AnimationCurve motionCurve = new AnimationCurve(new Keyframe[] {
																new Keyframe(0.0f, 0.0f),
																new Keyframe(0.5f, 1.0f),
																new Keyframe(1.0f, 0.0f)
		             										});

	private Vector3 startPos;
	private float phase;

	void Awake()
	{
		// get our starting position
		startPos = transform.localPosition;

		// start with random phase
		phase = Random.value;
	}
	
	void Update () 
	{
		// respond to the player speed multipler so we can go into slow-motion mode, but don't go any faster.
		float speedMultiplier = Mathf.Min(1.0f, GameManager.Player.SpeedMultiplier);

		// increment our phase taking into account the total length of one cycle
		phase += speedMultiplier * Time.deltaTime / cycleTime;
		// loop the cycle if it has passed the end
		phase = Mathf.Repeat(phase, 1.0f);
		// evaluate our motion curve to get the interpolation blend value at the current phase
		float blend = motionCurve.Evaluate(phase);

		// now just lerp the local position between the start and end positions by this blend value
		transform.localPosition = Vector3.Lerp(startPos, startPos + offset, blend);
	}
}
