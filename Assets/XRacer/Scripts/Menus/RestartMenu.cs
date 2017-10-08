using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleTween;

/// <summary>
/// Restart menu screen, shown when the player has crashed.
/// This expects to find some child elements with particular names, specifically:
/// 	NormalScreen - A CanvasGroup element that contains things to be displayed when the race didn't result in a new record.
/// 	NewRecordScreen - A CanvasGroup element that contains things to be displayed when the race did result in a new record.
/// 	DistanceMeters - A Text element that is a child of the NormalScreen element, which is updated to show the player's total distance.
/// 	RecordMeters - A Text element that is a child of the NormalScreen element, which is updated to show the current record distance.
/// 	NewRecordMeters - A Text element that is a child of the NewRecordScreen element, which is updated to show the new record distance.
/// 	CreditScore - Text element that is updated to show the credits collected during the race.
/// </summary>
public class RestartMenu : MenuScreen
{
	private CanvasGroup normalScreen;
	private CanvasGroup newRecordScreen;
	private Text distanceText;
	private Text oldRecordText;
	private Text newRecordText;
	private Text creditScoreText;

	protected override void Awake()
	{
		base.Awake();

		// get references to the elements we'll need to change later.
		normalScreen = transform.FindChild("NormalScreen").GetComponent<CanvasGroup>();
		newRecordScreen = transform.FindChild("NewRecordScreen").GetComponent<CanvasGroup>();
		distanceText = transform.FindChild("NormalScreen/DistanceMeters").GetComponent<Text>();
		oldRecordText = transform.FindChild("NormalScreen/RecordMeters").GetComponent<Text>();
		newRecordText = transform.FindChild("NewRecordScreen/NewRecordMeters").GetComponent<Text>();
		creditScoreText = transform.FindChild("CreditScore").GetComponent<Text>();
	}

	public override void Show(float fade, TransitionDirection direction, SimpleTween.Callback callback)
	{
		// don't animate the elements if we're returning to this screen from a submenu.
		if(direction == TransitionDirection.Forward)
		{
			// get the distance and credits values.
			float distance = GameManager.LevelManager.TotalDistance;
			float record = GameManager.RecordDistance;
			int creditScore = GameManager.CreditsThisRace;

			if(distance == record)
			{
				// this is a new record, so show the newRecordScreen elements
				normalScreen.alpha = 0.0f;
				newRecordScreen.alpha = 1.0f;

				// animate the distance text up from 0, then kick the scale at the end for effect.
				SimpleTweener.AddTween(()=>0, x=>newRecordText.text = Mathf.CeilToInt(x).ToString() + "m", record, 0.5f).Delay(0.2f).OnCompleted(()=>{
					KickItem(newRecordText.transform, 1.5f, 0.5f);
				});
				SimpleTweener.AddTween(()=>0, x=>creditScoreText.text = "+" + x, creditScore, 0.5f).Delay(0.7f).OnCompleted(()=>{
					KickItem(creditScoreText.transform, 1.5f, 0.3f);
				});
			}
			else
			{
				// no new record, so show the normalScreen elements
				normalScreen.alpha = 1.0f;
				newRecordScreen.alpha = 0.0f;
				
				// animate the text up from 0, then kick the scale at the end for effect.
				SimpleTweener.AddTween(()=>0, x=>distanceText.text = Mathf.CeilToInt(x).ToString() + "m", distance, 0.5f).Delay(0.2f).OnCompleted(()=>{
					KickItem(distanceText.transform, 1.5f, 0.3f);
				});
				SimpleTweener.AddTween(()=>0, x=>oldRecordText.text = Mathf.CeilToInt(x).ToString() + "m", record, 0.5f).Delay(0.5f).OnCompleted(()=>{
					KickItem(oldRecordText.transform, 1.5f, 0.3f);
				});
				SimpleTweener.AddTween(()=>0, x=>creditScoreText.text = "+" + x, creditScore, 0.5f).Delay(1.0f).OnCompleted(()=>{
					KickItem(creditScoreText.transform, 1.5f, 0.3f);
				});
			}
		}

		base.Show(fade, direction, callback);
	}

	private void KickItem(Transform item, float amount, float time)
	{
		// animate the scale of an item to give it a little 'kick'
		SimpleTweener.AddTween(()=>item.localScale, x=>item.localScale=x, amount*item.localScale, time).Ease(Easing.EaseKick);
	}
}
