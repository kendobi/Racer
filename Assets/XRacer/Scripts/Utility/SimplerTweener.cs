using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Very simple tweening library. 
// The interface is inspired by DOTween, so if you want a tweening library with many more features then I can recommend looking at DOTween.

// To use it you call the static AddTween method on the SimpleTweener class, passing in a delegate to get the current value, a delegate to set 
// the value, a target value, and a duration. The method is templated and there are currently tweening implementations for float, int, 
// Vector2, Vector3 and Color types. This function returns a Tween, which you can then chain further calls to in order to refine the behaviour.

// Example - to tween an object's position over 2 seconds, delayed by 0.5 seconds, calling a custom function once it has reached the target
//	SimpleTweener.AddTween(()=>transform.position, x=>transform.position=x, targetPos, 2.0f).Delay(0.5f).OnCompleted(()=>doSomething);

// Adding a second tween altering the same property before the first tween has finished will result in them both trying to update the
// value at the same time. Instead, call SimpleTweener.RemoveTween() passing in the first Tween object, before creating the second tween.

namespace SimpleTween
{
	public delegate void	Callback();
	public delegate T 		Getter<T>();
	public delegate void 	Setter<T>(T val);
	
	public enum Easing
	{
		EaseLinear,
		EaseIn,
		EaseOut,
		EaseInOut,
		EaseInBack,
		EaseOutBack,
		EaseKick			// <- this kicks the value to the target, then returns back to the start
	}

	public abstract class Tween
	{
		private float 		duration = 1.0f;			// the duration of the tween (in seconds)
		private float		delay = 0.0f;				// the delay before starting the tween
		private float		t = 0.0f;					// current t-value
		private Easing		ease = Easing.EaseOut;		// easing function
		private Callback	callback = null;			// callback to call once the tween is completed

		// For things that shouldn't be affected by Time.timeScale, don't use Time.deltaTime but instead calculate the actual time passed
		private bool 		useRealTime = false;

		public Tween()
		{
		}

		public Tween(float dur)
		{
			duration = Mathf.Max(0.01f, dur);
		}

		// overridden in subclasses to do the actual lerp between start and end values
		protected abstract void SetValue(float blend);

		public void Update(float dT)
		{
			if(useRealTime)
				dT = Time.unscaledDeltaTime;

			if(delay > 0.0f)
			{
				delay -= dT;
				return;
			}

			// calculate the blend value between start and end values
			t = Mathf.Clamp01(t + dT / duration);
			float blend = CalcEase(t);
			// defer to subclasses to actually set the value
			SetValue(blend);
		}

		/// <summary>
		/// Set a callback to be called when the tween completes
		/// </summary>
		/// <param name="cb">Callback function</param>
		public Tween OnCompleted(Callback cb)
		{
			callback = cb;
			return this;
		}

		/// <summary>
		/// Delay the tween by a specified number of seconds
		/// </summary>
		/// <param name="d">Seconds to delay the tween</param>
		public Tween Delay(float d)
		{
			delay = d;
			return this;
		}

		/// <summary>
		/// Set the easing function for the tween
		/// </summary>
		/// <param name="e">THe easing function to use</param>
		public Tween Ease(Easing e)
		{
			ease = e;
			return this;
		}

		/// <summary>
		/// Use an unscaled timer. Useful for things that need to animate whilst Time.timeScale == 0
		/// </summary>
		/// <param name="b">If true, use unscaled time</param>
		public Tween UseRealTime(bool b)
		{
			useRealTime = b;
			return this;
		}

		/// <summary>
		/// Check whether the tween has completed
		/// </summary>
		/// <value><c>true</c> if this tween has completed; otherwise, <c>false</c>.</value>
		public bool IsComplete
		{
			get { return t >= 1.0f; }
		}

		/// <summary>
		/// Gets the OnCompleted callback for this tween.
		/// </summary>
		/// <value>The callback.</value>
		public Callback Callback
		{
			get { return callback; }
		}

		/// <summary>
		/// Calculates the easing value for the given time (noramlised from 0-1)
		/// </summary>
		/// <returns>The easing value</returns>
		/// <param name="t">Normalised time value</param>
		private float CalcEase(float t)
		{
			const float velocity = 1.7f; // gives a default overshoot for ease in/out back.
			switch(ease)
			{
			case Easing.EaseLinear:
				return t;
			case Easing.EaseIn:
				return t*t*t;
			case Easing.EaseOut:
				t = 1.0f - t;
				return 1.0f - t*t*t;
			case Easing.EaseInOut:
				return 3.0f*t*t - 2.0f*t*t*t;
			case Easing.EaseInBack:
				return t*t*((velocity+1.0f)*t - velocity);
			case Easing.EaseOutBack:
				t = t - 1.0f;
				return t*t*((velocity+1.0f)*t + velocity) + 1.0f;
			case Easing.EaseKick:
				if(t < 0.3f)
				{
					t = 1.0f - t / 0.3f;
					return 1.0f - t*t*t;
				}
				else
				{
					t = (t-0.3f) / 0.7f;
					return 1.0f - (3.0f*t*t - 2.0f*t*t*t);
				}
			}

			// should never reach here, but it keeps the compiler happy.
			return t;
		}
	}

	/// <summary>
	/// Subclass for tweening float values.
	/// </summary>
	public class TweenFloat : Tween
	{
		protected Setter<float>	setter;
		protected float			startVal;
		protected float			endVal;

		public TweenFloat() {}
		public TweenFloat(Setter<float> set, float start, float end, float duration) : base(duration)
		{
			setter = set;
			startVal = start;
			endVal = end;

			setter(start);
		}

		protected override void SetValue (float blend)
		{
			setter(startVal * (1.0f - blend) + endVal * blend);
		}
	}

	/// <summary>
	/// Subclass for tweening int values.
	/// </summary>
	public class TweenInt : Tween
	{
		protected Setter<int>	setter;
		protected float			startVal;
		protected float			endVal;
		
		public TweenInt() {}
		public TweenInt(Setter<int> set, int start, int end, float duration) : base(duration)
		{
			setter = set;
			startVal = start;
			endVal = end;
			
			setter(start);
		}
		
		protected override void SetValue (float blend)
		{
			setter((int)(startVal * (1.0f - blend) + endVal * blend));
		}
	}

	/// <summary>
	/// Subclass for tweening Vector2 values.
	/// </summary>
	public class TweenVector2 : Tween
	{
		protected Setter<Vector2>	setter;
		protected Vector2			startVal;
		protected Vector2			endVal;
		
		public TweenVector2() {}
		public TweenVector2(Setter<Vector2> set, Vector2 start, Vector2 end, float duration) : base(duration)
		{
			setter = set;
			startVal = start;
			endVal = end;
			
			setter(start);
		}
		
		protected override void SetValue (float blend)
		{
			setter(startVal * (1.0f - blend) + endVal * blend);
		}
	}

	/// <summary>
	/// Subclass for tweening Vector3 values.
	/// </summary>
	public class TweenVector3 : Tween
	{
		protected Setter<Vector3>	setter;
		protected Vector3			startVal;
		protected Vector3			endVal;
		
		public TweenVector3() {}
		public TweenVector3(Setter<Vector3> set, Vector3 start, Vector3 end, float duration) : base(duration)
		{
			setter = set;
			startVal = start;
			endVal = end;
			
			setter(start);
		}
		
		protected override void SetValue (float blend)
		{
			setter(startVal * (1.0f - blend) + endVal * blend);
		}
	}

	/// <summary>
	/// Subclass for tweening Color values.
	/// </summary>
	public class TweenColor : Tween
	{
		protected Setter<Color>	setter;
		protected Color			startVal;
		protected Color			endVal;
		
		public TweenColor() {}
		public TweenColor(Setter<Color> set, Color start, Color end, float duration) : base(duration)
		{
			setter = set;
			startVal = start;
			endVal = end;
			
			setter(start);
		}
		
		protected override void SetValue (float blend)
		{
			setter(startVal * (1.0f - blend) + endVal * blend);
		}
	}

	/// <summary>
	/// Tween manager class. Don't add this to an object, it is created automatically when the first tween is requested.
	/// </summary>
	public class SimpleTweener : MonoBehaviour 
	{
		public int activeTweens = 0;						// number of active tweens, public so we can see it in the inspector while the game is running.
		private List<Tween>	tweens = new List<Tween>();		// list of active tweens.

		private static SimpleTweener instance = null;

		void Update() 
		{
			float dT = Time.deltaTime;

			// update all the active tweens, removing any that have completed
			int i = 0;
			while(i < tweens.Count)
			{
				Tween tween = tweens[i];
				tween.Update(dT);

				if(tween.IsComplete)
				{
					if(tween.Callback != null)
						tween.Callback();
					tweens.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			activeTweens = tweens.Count;
		}

		/// <summary>
		/// Add a new Tween
		/// </summary>
		/// <returns>The new Tween.</returns>
		/// <param name="getter">Getter function for retreiving the current value</param>
		/// <param name="setter">Setter function for setting the value</param>
		/// <param name="end">target value</param>
		/// <param name="duration">Duration in seconds</param>
		public static Tween AddTween<T>(Getter<T> getter, Setter<T> setter, T end, float duration)
		{
			// create our instance if necessary.
			if(instance == null)
			{
				GameObject obj = new GameObject("TweenHandler");
				instance = obj.AddComponent<SimpleTweener>();
			}

			// determine which subclass we need based on the type
			Tween tween = null;
			if(typeof(T) == typeof(float))
			{
				tween = new TweenFloat((Setter<float>)(object)setter, (float)(object)getter(), (float)(object)end, duration);
				instance.tweens.Add(tween);
			}
			else if(typeof(T) == typeof(int))
			{
				tween = new TweenInt((Setter<int>)(object)setter, (int)(object)getter(), (int)(object)end, duration);
				instance.tweens.Add(tween);
			}
			else if(typeof(T) == typeof(Vector2))
			{
				tween = new TweenVector2((Setter<Vector2>)(object)setter, (Vector2)(object)getter(), (Vector2)(object)end, duration);
				instance.tweens.Add(tween);
			}
			else if(typeof(T) == typeof(Vector3))
			{
				tween = new TweenVector3((Setter<Vector3>)(object)setter, (Vector3)(object)getter(), (Vector3)(object)end, duration);
				instance.tweens.Add(tween);
			}
			else if(typeof(T) == typeof(Color))
			{
				tween = new TweenColor((Setter<Color>)(object)setter, (Color)(object)getter(), (Color)(object)end, duration);
				instance.tweens.Add(tween);
			}
			else
			{
				// You're trying to tween a type that isn't implemented yet.
				// You'll need to subclass Tween and implement the SetValue method, then create a new instance of your
				// subclass above when the tpye T matches.
				Debug.LogError("Tween type not supported " + typeof(T));
			}

			return tween;
		}

		/// <summary>
		/// Remove a running tween
		/// </summary>
		/// <param name="tween">The tween to remove</param>
		public static void RemoveTween(Tween tween)
		{
			if(instance == null)
				return;

			instance.tweens.Remove(tween);
		}
	}

}