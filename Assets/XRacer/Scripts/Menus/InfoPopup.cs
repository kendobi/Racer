using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleTween;

/// <summary>
/// Show a temporary message on screen
/// This expects to find some child elements with particular names, specifically:
/// 	Countdown - this should be a CanvasGroup element that contains the countdown graphic
/// 	Progress - this should be a child of the Countdown object that is scaled to show the countdown progress
/// </summary>
public class InfoPopup : MonoBehaviour 
{
	private CanvasGroup canvasGroup;
	private Text text;

	private CanvasGroup countdown;
	private RectTransform countdownProgress;

	private Tween alphaTween;
	private Tween positionTween;
	private Tween countdownTween;
	private bool isShowing = false;

	void Start () 
	{
		canvasGroup = GetComponent<CanvasGroup>();
		text = GetComponentInChildren<Text>();
		countdown = transform.FindChild("Countdown").GetComponent<CanvasGroup>();
		countdownProgress = transform.FindChild("Countdown/Progress").GetComponent<RectTransform>();

		canvasGroup.alpha = 0.0f;
		countdown.alpha = 0.0f;
	}

	private void ShowInfoPopup(float duration, float fadeTime)
	{
		// if we're still showing a previous message, then cancel the previous tweens
		if(isShowing)
		{
			SimpleTweener.RemoveTween(alphaTween);
			SimpleTweener.RemoveTween(positionTween);
		}
		
		isShowing = true;

		// fade up the alpha, wait for our duration, then fade it out again.
		alphaTween = SimpleTweener.AddTween(()=>canvasGroup.alpha, x=>canvasGroup.alpha=x, 1.0f, fadeTime).OnCompleted(()=>{
			alphaTween = SimpleTweener.AddTween(()=>canvasGroup.alpha, x=>canvasGroup.alpha=x, 0.0f, fadeTime).Delay(duration).OnCompleted(()=>{
				// the popup has now disappeared
				isShowing = false;
			});
		});

		// also animate the x-position in a similar fashion.
		RectTransform rect = GetComponent<RectTransform>();
		Vector2 startPos = rect.anchoredPosition;
		startPos.x = 200.0f;
		rect.anchoredPosition = startPos;
		positionTween = SimpleTweener.AddTween(()=>rect.anchoredPosition, x=>rect.anchoredPosition=x, new Vector2(30, startPos.y), fadeTime).Ease(Easing.EaseLinear).OnCompleted(()=>{
			positionTween = SimpleTweener.AddTween(()=>rect.anchoredPosition, x=>rect.anchoredPosition=x, new Vector2(-30, startPos.y), duration).Ease(Easing.EaseLinear).OnCompleted(()=>{
				positionTween = SimpleTweener.AddTween(()=>rect.anchoredPosition, x=>rect.anchoredPosition=x, new Vector2(-200, startPos.y), fadeTime).Ease(Easing.EaseLinear);
			});
		});
	}

	/// <summary>
	/// Show an on screen message
	/// </summary>
	/// <param name="message">The message to display</param>
	/// <param name="duration">The duration to display it for (in seconds)</param>
	/// <param name="fadeTime">The transition in/out time.</param>
	public void ShowMessage(string message, float duration, float fadeTime)
	{
		// set the message text
		text.text = message;
		// hide the countdown graphic as we're only displaying a message
		countdown.alpha = 0.0f;
		// animate our panel
		ShowInfoPopup(duration, fadeTime);
	}

	/// <summary>
	/// Show a message along with a countdown
	/// </summary>
	/// <param name="message">The message to display</param>
	/// <param name="duration">The duration to display it for (also the duration of the countdown)</param>
	/// <param name="fadeTime">The transition in/out time.</param>
	public void ShowCountdown(string message, float duration, float fadeTime)
	{
		// if we are still showing a previous countdown, then cancel the previous tween
		if(isShowing)
		{
			SimpleTweener.RemoveTween(countdownTween);
		}

		// set the message text
		text.text = message;
		// make the countdown graphic visible
		countdown.alpha = 1.0f;

		// animate the countdown progress by scaling the progress graphic
		countdownTween = SimpleTweener.AddTween(()=>Vector3.one, x=>countdownProgress.localScale=x, new Vector3(0.0f, 1.0f, 1.0f), duration).Delay(fadeTime).Ease(Easing.EaseLinear);
		// animate the whole info panel
		ShowInfoPopup(duration, fadeTime);
	}
}
