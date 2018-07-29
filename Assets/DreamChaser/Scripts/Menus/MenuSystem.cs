using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple Menu system for managing multiple menu screens and transitioning between them.
/// </summary>
public class MenuSystem : MonoBehaviour 
{
	// The set of menus screens we are managing, organised by name
	private Dictionary<string, MenuScreen> screens = new Dictionary<string, MenuScreen>();
	// The traversal history, so we can return through previous menus
	private Stack<MenuScreen> screenHistory = new Stack<MenuScreen>();
	// The menu system also manages the HUD
	private HUD hud;

	/// <summary>
	/// Gets the currently displayed screen (or null if none is open)
	/// </summary>
	/// <value>The current menu screen.</value>
	public MenuScreen CurrentScreen
	{
		get { return screenHistory.Count == 0 ? null : screenHistory.Peek(); }
	}

	void Awake()
	{
		// find all the menu screens in the scene
		MenuScreen[] allScreens = GameObject.FindObjectsOfType<MenuScreen>();
		for(int i=0; i<allScreens.Length; ++i)
		{
			// add them to our dictionary for easy lookup
			MenuScreen screen = allScreens[i];
			screens[screen.name] = screen;			
		}
		// also find the hud object
		hud = GameObject.FindObjectOfType<HUD>();
	}

	void Start()
	{
		// hide all the screens to start with
		// this is done in Start() instead of Awake() so the screens themselves get the chance to execute
		// their Awake() methods.

		MenuScreen[] allScreens = GameObject.FindObjectsOfType<MenuScreen>();
		MenuScreen currentScreen = CurrentScreen;
		for(int i=0; i<allScreens.Length; ++i)
		{
			MenuScreen screen = allScreens[i];
			if(screen != currentScreen)
			{
				screen.gameObject.SetActive(false);
			}
		}
	}

	public HUD HUD
	{
		get { return hud; }
	}

	/// <summary>
	/// Get a menu screen by name
	/// </summary>
	/// <returns>The menu screen</returns>
	/// <param name="name">The name of screen to find</param>
	public MenuScreen GetScreen(string name)
	{
		return screens[name];
	}

	/// <summary>
	/// Show a particular menu screen
	/// </summary>
	/// <param name="screen">The screen to show</param>
	/// <param name="fade">The transition time.</param>
	/// <param name="callback">A Callback function to call once the transition is complete</param>
	public void ShowScreen(MenuScreen screen, float fade=0.5f, SimpleTween.Callback callback=null)
	{
		if(screen == null)
		{
			Debug.LogWarning("Attempting to show null Menu Screen");
			return;
		}

		MenuScreen currentScreen = CurrentScreen;

		// add the new screen to the history
		screenHistory.Push(screen);

		// hide the current screen before showing the new one
		if(currentScreen != null)
		{
			currentScreen.Hide(fade, MenuScreen.TransitionDirection.Forward, ()=> {
				screen.Show(fade, MenuScreen.TransitionDirection.Forward, callback);
			});
		}
		else
		{
			screen.Show(fade, MenuScreen.TransitionDirection.Forward, callback);
		}
	}

	/// <summary>
	/// Return to the previous menu screen in the history
	/// </summary>
	/// <param name="fade">Transition time</param>
	/// <param name="callback">Callback funtion to call when the transition is complete</param>
	public void GoBack(float fade=0.5f, SimpleTween.Callback callback=null)
	{
		if(screenHistory.Count == 0)
			return;

		MenuScreen currentScreen = screenHistory.Pop();
		MenuScreen prevScreen = screenHistory.Count == 0 ? null : screenHistory.Peek();

		// hide the current screen and then show the previous screen (if there is one).
		if(prevScreen != null)
		{
			currentScreen.Hide(fade, MenuScreen.TransitionDirection.Back, ()=> {
				prevScreen.Show (fade, MenuScreen.TransitionDirection.Back, callback);
			});
		}
		else
		{
			currentScreen.Hide(fade, MenuScreen.TransitionDirection.Back, callback);
		}
	}

	/// <summary>
	/// Exits all of the menus without going back through the history
	/// </summary>
	/// <param name="fade">Transition time</param>
	/// <param name="callback">Callback function to call when the transition is complete</param>
	public void ExitAll(float fade=0.5f, SimpleTween.Callback callback=null)
	{
		if(screenHistory.Count == 0)
			return;

		// hide the current menu screen
		MenuScreen currentScreen = screenHistory.Pop();
		currentScreen.Hide(fade, MenuScreen.TransitionDirection.Back, callback);

		// clear the history, since we have now completely exited the menus.
		screenHistory.Clear();
	}
}
