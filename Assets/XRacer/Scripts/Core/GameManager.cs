using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using SimpleTween;

/// <summary>
/// Main Game Manager, handles the overall state of the game. 
/// There should be exactly one of these in the scene.
/// </summary>
public class GameManager : MonoBehaviour 
{
	[Header("Mobile Options")]
	[Tooltip("Disable the fullscreen blur under the main menu when running on mobile devices")]
	public bool disableMenuBlurOnMobile = false;
	[Tooltip("Disable the fullscreen motion-blur effect when running on mobile devices")]
	public bool disableMotionBlurOnMobile = true;

	public enum GameState
	{
		InMenus,
		Playing,
		Paused,
		Dead
	}

	private GameState state;

	// references to our player and level manager.
	private PlayerControl player;
	private LevelManager levelManager;

	// references to various menu screens
	private MenuSystem menuSystem;
	private MenuScreen mainMenu;
	private MenuScreen pauseMenu;
	private MenuScreen restartMenu;
	private MenuScreen storeMenu;
	private InfoPopup infoPopup;
	private CanvasGroup screenFade;

	// our fullscreen effects (that may need disabling on mobile).
	private CameraMotionBlur motionBlur;
	private BlurOptimized screenBlur;

	// multiplier value for all credits collected in the current race.
	private float creditMultiplier = 1.0f;
	// number of credits collected in the current race.
	private int creditsThisRace;

	// the current record distance (at the start of the race)
	private float targetDistance;
	// whether we have broken the record distance this race
	private bool brokenRecord;

	private static GameManager instance;

	/// <summary>
	/// Get a reference to the player object
	/// </summary>
	/// <value>The player object</value>
	public static PlayerControl Player
	{
		get { return instance.player; }
	}

	/// <summary>
	/// Get a reference to the level manager
	/// </summary>
	/// <value>The level manager</value>
	public static LevelManager LevelManager
	{
		get { return instance.levelManager; }
	}

	/// <summary>
	/// Get a reference to the global menu system.
	/// </summary>
	/// <value>The menu system.</value>
	public static MenuSystem MenuSystem
	{
		get { return instance.menuSystem; }
	}

	/// <summary>
	/// Get the current game state
	/// </summary>
	/// <value>The current game state</value>
	public static GameState CurrentState
	{
		get { return instance.state; }
	}

	/// <summary>
	/// Gets or sets the record distance.
	/// </summary>
	/// <value>The record distance.</value>
	public static float RecordDistance
	{
		get { return PlayerPrefs.GetFloat("RecordDistance", 0.0f); }
		set { PlayerPrefs.SetFloat("RecordDistance", value); }
	}

	/// <summary>
	/// Gets or sets the total credits the player currently has.
	/// Note that we use the default PlayerPrefs class for storing the credits the player owns, you may want to
	/// alter this to use a more secure method (or encrypt the values), especially if real money is involved.
	/// </summary>
	/// <value>The total credits the player owns</value>
	public static int TotalCredits
	{
		get { return PlayerPrefs.GetInt("TotalCredits", 0); }
		set { PlayerPrefs.SetInt("TotalCredits", value); }
	}

	/// <summary>
	/// Gets the amount of credits collected during this race.
	/// </summary>
	/// <value>The credits collected this race.</value>
	public static int CreditsThisRace
	{
		get { return instance.creditsThisRace; }
	}

	void Start () 
	{
		instance = this;

		// cache references to our player and level manager objects
		player = GameObject.FindObjectOfType<PlayerControl>();
		levelManager = GameObject.FindObjectOfType<LevelManager>();

		// cache some references to various menu screens and so on
		menuSystem = GameObject.FindObjectOfType<MenuSystem>();
		mainMenu = menuSystem.GetScreen("MainMenu");
		pauseMenu = menuSystem.GetScreen("PauseMenu");
		restartMenu = menuSystem.GetScreen("RestartMenu");
		storeMenu = menuSystem.GetScreen("StoreMenu");
		screenFade = GameObject.Find("ScreenFade").GetComponent<CanvasGroup>();
		infoPopup = GameObject.FindObjectOfType<InfoPopup>();

		// find our full screen effects
		motionBlur = GameObject.FindObjectOfType<CameraMotionBlur>();
		screenBlur = GameObject.FindObjectOfType<BlurOptimized>();

		// start with the screen fader opaque
		screenFade.alpha = 1.0f;

		creditMultiplier = 1.0f;

		CheckShopItemIDs();	
		InitFullscreenEffects();

		OnEnterStateMenus();
	}

	private void CheckShopItemIDs()
	{
		// check the shop items all have unique ids
		ShopItem[] shopItems = GameObject.FindObjectsOfType<ShopItem>();
		List<string> itemIDs = new List<string>();
		for(int i=0; i<shopItems.Length; ++i)
		{
			ShopItem item = shopItems[i];
			if(itemIDs.Contains(item.itemID))
				Debug.LogError("Two Shop Items Have the ID " + item.itemID + ". Please make sure all items have a unique ID string");
			itemIDs.Add(item.itemID);
		}
	}

	private void InitFullscreenEffects()
	{
		if(motionBlur != null)
		{
			// because our camera stays stationary whilst the geometry scrolls past, the normal calculation of
			// camera motion blur won't work. Instead we use the 'preview' mode to simulate a particular camera velocity.
			motionBlur.preview = true;
			motionBlur.previewScale = Vector3.zero;
			
			if(Application.isMobilePlatform && disableMotionBlurOnMobile)
				motionBlur.enabled = false;
		}

		if(screenBlur != null && Application.isMobilePlatform && disableMenuBlurOnMobile)
		{
			screenBlur.enabled = false;
			screenBlur = null;
		}

		// antialiasing is expensive, and not really necessary on mobile due to the much higher dpi
		Antialiasing aaFilter = GameObject.FindObjectOfType<Antialiasing>();
		if(aaFilter != null && Application.isMobilePlatform)
			aaFilter.enabled = false;

	}

	private void OnEnterStateMenus()
	{
		// show the main menu screen.
		menuSystem.ShowScreen(mainMenu,0.5f);

		// don't show the player under the menus
		player.gameObject.SetActive(false);
		levelManager.Reset();
		creditMultiplier = 1.0f;
		state = GameState.InMenus;

		if(screenBlur != null)
			screenBlur.enabled = true;

		SoundManager.PlayMusic("IntroLoop");
		ScreenFaderFadeOut(1.0f);
	}

	private void OnEnterStateGame()
	{
		// show the on screen HUD
		menuSystem.HUD.Show();

		// disable the fullscreen blur
		if(screenBlur != null)
			screenBlur.enabled = false;

		// enable the player
		player.gameObject.SetActive(true);
		player.Reset();

		// reset the credit multiplier and credits collected this race.
		creditMultiplier = 1.0f;
		creditsThisRace = 0;

		// apply all purchased items
		ShopItem[] shopItems = GameObject.FindObjectsOfType<ShopItem>();
		for(int i=0; i<shopItems.Length; ++i)
			shopItems[i].OnGameStarted();

		targetDistance = RecordDistance;
		brokenRecord = false;

		// don't report a new record the first time we play.
		if(targetDistance == 0)
			brokenRecord = true;

		// start the level scrolling!
		levelManager.StartGame();
		state = GameState.Playing;

		SoundManager.PlayMusic("GameLoop");
		ScreenFaderFadeOut(0.4f);
	}

	private void OnEnterStateDeath()
	{
		menuSystem.ShowScreen(restartMenu, 0.5f);
	}

	public void OnShowMainMenu()
	{
		SoundManager.PlaySfx("ButtonPress");
		menuSystem.ExitAll();
		ScreenFaderFadeIn(0.8f, ()=>OnEnterStateMenus());
	}

	public void OnStartGamePressed()
	{
		SoundManager.PlaySfx("ButtonPress");
		menuSystem.ExitAll();
		ScreenFaderFadeIn(0.8f, ()=>OnEnterStateGame());
	}

	public void OnShowStorePressed()
	{
		SoundManager.PlaySfx("ButtonPress");
		menuSystem.ShowScreen(storeMenu, 0.5f);
	}

	public void ClearInventory()
	{
		// remove all items from the inventory (including any purchased items)
		ShopItem[] shopItems = GameObject.FindObjectsOfType<ShopItem>();
		for(int i=0; i<shopItems.Length; ++i)
			shopItems[i].RemoveFromInventory();
	}

	public static void SetCreditMultiplier(float multiplier)
	{
		// all credits collected during this race will be multiplied by the given value
		instance.creditMultiplier = multiplier;
	}

	public static void AwardCredits(int credit)
	{
		// award some credits to the player
		instance.creditsThisRace += Mathf.CeilToInt(credit * instance.creditMultiplier);
	}

	public void GameOver()
	{
		// check if it's a new record
		if(levelManager.TotalDistance > RecordDistance)
			RecordDistance = levelManager.TotalDistance;

		// add the credits collected this race to the player inventory
		TotalCredits += creditsThisRace;

		// cancel any active powerups, we don't want them to still be running when we restart.
		PowerupManager.CancelAllPowerups();
		state = GameState.Dead;

		menuSystem.HUD.Hide();
		SoundManager.PauseMusic(0.2f);

		// pause then show the restart menu.
		Invoke ("OnEnterStateDeath", 1.0f);
	}

	public void PauseGame()
	{
		if(state != GameState.Playing)
			return;

		Time.timeScale = 0.0f;
		state = GameState.Paused;

		SoundManager.PlaySfx("Pause");
		SoundManager.PauseMusic(0.5f);
		pauseMenu.Show(0.3f);
	}

	public void ResumeGame()
	{
		Time.timeScale = 1.0f;
		state = GameState.Playing;

		SoundManager.PlaySfx("Pause");
		SoundManager.UnpauseMusic();
		pauseMenu.Hide(0.3f);
	}

	public static void OnGameOver()
	{
		instance.GameOver();
	}
	
	public static void ShowInfo(string msg)
	{
		// show a temporary on screen message
		instance.infoPopup.ShowMessage(msg, 2.0f, 0.3f);
	}

	public static void ShowCountdown(string msg, float duration, float fadeTime)
	{
		// show a temporary on screen message with a countdown
		instance.infoPopup.ShowCountdown(msg, duration, fadeTime);
	}

	private void ScreenFaderFadeIn(float time, SimpleTween.Callback onCompletedCB)
	{
		// fade the screen to white
		screenFade.gameObject.SetActive(true);
		screenFade.alpha = 0.0f;
		SimpleTweener.AddTween(()=>screenFade.alpha, x=>screenFade.alpha=x, 1.0f, time).OnCompleted(onCompletedCB);
	}

	private void ScreenFaderFadeOut(float time)
	{
		// fade out the white overlay
		if(screenFade.gameObject.activeSelf)
			SimpleTweener.AddTween(()=>screenFade.alpha, x=>screenFade.alpha=x, 0.0f, time).OnCompleted(()=>screenFade.gameObject.SetActive(false));
	}

	void Update () 
	{
		if(motionBlur != null)
		{
			// update the motion blur values.
			// because our camera stays stationary whilst the geometry scrolls past, the normal calculation of
			// camera motion blur won't work. Instead we use the 'preview' mode to simulate a particular camera velocity.
			Vector3 motion = new Vector3(player.Steer, 0.0f, player.Speed);
			motion = Vector3.ClampMagnitude(motion, 10.0f);
			motionBlur.previewScale = motion;
		}

		if(state == GameState.Playing)
		{
			// check if we've set a new record
			if(levelManager.TotalDistance > targetDistance)
			{
				// if we haven't already informed the player, then pop up a message
				if(!brokenRecord)
				{
					// a new record!
					ShowInfo("NEW RECORD!");
					menuSystem.HUD.OnNewRecord();
					brokenRecord = true;
				}
				RecordDistance = levelManager.TotalDistance;
			}

			// press space to pause the game.
			if(Input.GetKeyUp(KeyCode.Space) && !pauseMenu.isActiveAndEnabled)
				PauseGame();
		}
	}
}
