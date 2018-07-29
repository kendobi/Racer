using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using SimpleTween;

/// <summary>
/// Menu screen base class. This is used by the MenuSystem for showing and hiding various screens.
/// If a screen requires specific behaviour, this can be subclassed to provide it (see e.g. the RestartMenu).
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MenuScreen : MonoBehaviour 
{
	[Tooltip("this item (if not null) will be set as the focused ui item when this menu screen opens")]
	public GameObject defaultUIItem;

	public enum TransitionDirection
	{
		Forward,		// we are transitioning into this screen for the first time
		Back			// we are returning to this screen from a sub-menu
	}

	private CanvasGroup canvasGroup;
	private RectTransform rectXfm;

	private Tween alphaTween;
	private Tween posTween;

	public CanvasGroup CanvasGroup
	{
		get { return canvasGroup; }
	}

	protected virtual void Awake () 
	{
		// get references to our canvas group and transform for animating later.
		canvasGroup = GetComponent<CanvasGroup>();
		rectXfm = GetComponent<RectTransform>();
		// start off transparent.
		canvasGroup.alpha = 0.0f;
	}

	/// <summary>
	/// Show this menu screen.
	/// </summary>
	/// <param name="fade">Transition time</param>
	/// <param name="direction">Direction - Back if we are returning from a sub-menu, Forward otherwise</param>
	/// <param name="callback">Callback to call once the transition is finished</param>
	public virtual void Show(float fade, TransitionDirection direction=TransitionDirection.Forward, SimpleTween.Callback callback=null)
	{
		gameObject.SetActive(true);

		// cancel any running tweens in case we are returning to this menu immediately after having left it.
		SimpleTweener.RemoveTween(alphaTween);
		SimpleTweener.RemoveTween(posTween);

		if(fade == 0.0f)
		{
			canvasGroup.alpha = 1.0f;
			if(callback != null)
				callback();
		}
		else
		{
			// animate the position and alpha of the screen
			alphaTween = SimpleTweener.AddTween(()=>CanvasGroup.alpha, x=>CanvasGroup.alpha = x, 1.0f, fade).OnCompleted(callback).UseRealTime(true);
			posTween = SimpleTweener.AddTween(()=>new Vector2(0,-30), x=>rectXfm.anchoredPosition = x, Vector2.zero, fade).UseRealTime(true);
		}

		// set our default UI item so it has focus.
		EventSystem.current.SetSelectedGameObject(defaultUIItem);
	}

	/// <summary>
	/// Hide this menu screen
	/// </summary>
	public virtual void Hide(float fade, TransitionDirection direction=TransitionDirection.Back, SimpleTween.Callback callback=null)
	{
		// cancel any running tweens in case we are leaving this menu immediately after having opened it.
		SimpleTweener.RemoveTween(alphaTween);
		SimpleTweener.RemoveTween(posTween);

		if(fade == 0.0f)
		{
			canvasGroup.alpha = 0.0f;

			// deactivate this gameobject when the screen is not visible
			gameObject.SetActive(false);

			if(callback != null)
				callback();
		}
		else
		{
			// fade out the canvas group, then disable the gameobject once it's invisible
			alphaTween = SimpleTweener.AddTween(()=>CanvasGroup.alpha, x=>CanvasGroup.alpha = x, 0.0f, fade).UseRealTime(true).OnCompleted(()=> {
				gameObject.SetActive(false);
				if(callback != null)
					callback();
			});
			posTween = SimpleTweener.AddTween(()=>Vector2.zero, x=>rectXfm.anchoredPosition = x, new Vector2(0,-30), fade).UseRealTime(true);
		}
	}
}
