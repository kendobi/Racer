using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleTween;

/// <summary>
/// Simple HUD that displays the current distance and the current record.
/// This expects to find some child Text elements with particular names, specifically:
/// 	DistanceMeters - this should be a Text element that is updated every frame with the current disance value
/// 	RecordMeters - this should be a Text element that is updated to show the current record distance
/// </summary>
public class HUD : MonoBehaviour 
{
	private CanvasGroup canvasGroup;
	private Text distanceText;
	private Text recordText;

	void Start () 
	{
		canvasGroup = GetComponent<CanvasGroup>();
		// find our child text components
		distanceText = transform.FindChild("DistanceMeters").GetComponent<Text>();
		recordText = transform.FindChild("RecordMeters").GetComponent<Text>();

		// start with HUD off.
		canvasGroup.alpha = 0.0f;
	}
	
	void Update () 
	{
		// update the text values
		distanceText.text = Mathf.CeilToInt(GameManager.LevelManager.TotalDistance) + "m";
		recordText.text = Mathf.CeilToInt(GameManager.RecordDistance).ToString() + "m";
	}

	public void Show()
	{
		// slide on
		RectTransform rect = canvasGroup.GetComponent<RectTransform>();
		SimpleTweener.AddTween(()=>new Vector2(-80, 0), x=>rect.anchoredPosition=x, Vector2.zero, 0.5f);
		SimpleTweener.AddTween(()=>canvasGroup.alpha, x=>canvasGroup.alpha=x, 1.0f, 0.5f);
	}

	public void Hide()
	{
		// slide off
		RectTransform rect = canvasGroup.GetComponent<RectTransform>();
		SimpleTweener.AddTween(()=>Vector2.zero, x=>rect.anchoredPosition=x, new Vector2(-80, 0), 0.5f).Ease(Easing.EaseIn);
		SimpleTweener.AddTween(()=>canvasGroup.alpha, x=>canvasGroup.alpha=x, 0.0f, 0.5f).Ease (Easing.EaseIn);
	}

	public void OnNewRecord()
	{
		// give a little visual kick to the distance labels
		RectTransform distRect = distanceText.GetComponent<RectTransform>();
		RectTransform recordRect = recordText.GetComponent<RectTransform>();
		SimpleTweener.AddTween(()=>distRect.localScale, x=>distRect.localScale=x, 1.5f*Vector3.one, 0.5f).Ease(Easing.EaseKick);
		SimpleTweener.AddTween(()=>recordRect.localScale, x=>recordRect.localScale=x, 1.5f*Vector3.one, 0.5f).Delay(0.1f).Ease(Easing.EaseKick);
	}
}
