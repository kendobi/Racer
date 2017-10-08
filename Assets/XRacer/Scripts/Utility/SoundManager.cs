using UnityEngine;
using System.Collections;

/// <summary>
/// A simple sound manager class for handling the playing of sound effects and music
/// </summary>
public class SoundManager : MonoBehaviour 
{
	public AudioClip[] sounds;					// list of available sound clips
	public AudioClip[] music;					// list of available music tracks

	private static SoundManager soundMan;		// global SoundManager instance
	private AudioSource sfxAudio;				// AudioSource component for playing sound fx.
	private AudioSource musicAudio;				// AudioSource component for playing music

	void Awake()
	{
		if(soundMan != null)
		{
			Debug.LogError("More than one SoundManager found in the scene");
			return;
		}

		soundMan = this;
		sfxAudio = gameObject.AddComponent<AudioSource>();
		musicAudio = gameObject.AddComponent<AudioSource>();

		sfxAudio.playOnAwake = false;
		musicAudio.playOnAwake = false;
		musicAudio.loop = true;
	}

	/// <summary>
	/// Play a sound clip by name
	/// </summary>
	/// <param name="sfxName">The name of the sound to play</param>
	public static void PlaySfx(string sfxName)
	{
		if(soundMan == null)
		{
			Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
			return;
		}

		soundMan.PlaySound(sfxName, soundMan.sounds, soundMan.sfxAudio);
	}

	/// <summary>
	/// Plays a given sound clip	
	/// </summary>
	/// <param name="clip">The sound clip to play.</param>
	public static void PlaySfx(AudioClip clip)
	{
		soundMan.PlaySound(clip, soundMan.sfxAudio);
	}

	/// <summary>
	/// Start playing a music track from the beginning
	/// </summary>
	/// <param name="trackName">Track name.</param>
	public static void PlayMusic(string trackName)
	{
		if(soundMan == null)
		{
			Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
			return;
		}

		// reset track to beginning
		soundMan.musicAudio.time = 0.0f;
		soundMan.musicAudio.volume = 1.0f;

		soundMan.PlaySound(trackName, soundMan.music, soundMan.musicAudio);
	}

	/// <summary>
	/// Pauses the music.
	/// </summary>
	/// <param name="fadeTime">Fade out time.</param>
	public static void PauseMusic(float fadeTime)
	{
		if(fadeTime > 0.0f)
			soundMan.StartCoroutine(soundMan.FadeMusicOut(fadeTime));
		else
			soundMan.musicAudio.Pause();
	}

	/// <summary>
	/// Unpauses the music.
	/// </summary>
	public static void UnpauseMusic()
	{
		soundMan.musicAudio.volume = 1.0f;
		soundMan.musicAudio.Play();
	}

	/// <summary>
	/// Plays a sound using a given AudioSource
	/// </summary>
	private void PlaySound(string soundName, AudioClip[] pool, AudioSource audioOut)
	{
		// loop through our list of clips until we find the right one.
		foreach(AudioClip clip in pool)
		{
			if(clip.name == soundName)
			{
				PlaySound(clip, audioOut);
				return;
			}
		}

		Debug.LogWarning("No sound clip found with name " + soundName);
	}

	/// <summary>
	/// Plays a sound using a given AudioSource
	/// </summary>
	private void PlaySound(AudioClip clip, AudioSource audioOut)
	{
		audioOut.clip = clip;
		audioOut.Play();
	}

	/// <summary>
	/// Co-Routine for fading out the music
	/// </summary>
	/// <param name="time">Fade time</param>
	IEnumerator FadeMusicOut(float time)
	{
		float startVol = musicAudio.volume;
		float startTime = Time.realtimeSinceStartup;

		while(true)
		{
			// use realtimeSinceStartup because Time.time doesn't increase when the game is paused.
			float t = (Time.realtimeSinceStartup - startTime) / time;
			if(t < 1.0f)
			{
				musicAudio.volume = (1.0f - t) * startVol;
				yield return 0;
			}
			else
			{
				break;
			}
		}

		// once we've fully faded out, pause the track
		musicAudio.Pause();
	}
}
