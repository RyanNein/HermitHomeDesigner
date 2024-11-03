using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	private void Awake()
	{
		if (instance != null)
			Destroy(gameObject);
		else
			instance = this;

		DontDestroyOnLoad(gameObject);

		musicSource = gameObject.AddComponent<AudioSource>();
	}

	AudioSource musicSource;
	List<AudioSource> sources = new List<AudioSource>();

	// Groups:
	[SerializeField] private AudioMixerGroup _musicGroup;
	public AudioMixerGroup MusicGroup => _musicGroup;

	[SerializeField] private AudioMixerGroup _hoverGroup;
	public AudioMixerGroup HoverGroup => _hoverGroup;

	[SerializeField] public AudioMixerGroup _clickGroup;
	public AudioMixerGroup ClickGroup => _clickGroup;

	[SerializeField] private AudioMixerGroup _typeGroup;
	public AudioMixerGroup TypeGroup => _typeGroup;

	[SerializeField] private AudioMixerGroup _generalSfxGroup;
	public AudioMixerGroup GeneralSfxgroup => _generalSfxGroup;

	const int MAX_SOURCES = 10;

	float volumeOnPause;

	Coroutine fadeRoutine;

	private void Start()
	{
		MenuUtilitySpace.PauseMenu.OnPauseStart += LowerVolume;
		MenuUtilitySpace.PauseMenu.OnPauseEnd += ResetVolume;

		SceneLoader.OnFadeExitStart += StartMusicFadeOut;
	}


	public void PlayMusic(AudioClip clip, float volume = 1f)
	{
		if (fadeRoutine != null)
			StopCoroutine(fadeRoutine);
		musicSource.clip = clip;
		musicSource.outputAudioMixerGroup = MusicGroup;
		musicSource.volume = volume;
		musicSource.loop = true;
		musicSource.Play();
	}

	public void StartMusicFadeOut()
	{
		fadeRoutine = StartCoroutine(FadeOut());
	}

	IEnumerator FadeOut()
	{
		var volume = musicSource.volume;
		while (musicSource.volume >= 0f)
		{
			musicSource.volume -= volume / 60f;
			yield return new WaitForSeconds(1f / 60f);
		}
		musicSource.volume = 0f;
	}

	public void PlaySFXOneShot(AudioClip clip, AudioMixerGroup mixGroup, float volume = 1f)
	{
		bool played = false;

		// Find unused source:
		for (int i = 0; i < sources.Count; i++)
		{
			var source = sources[i];
			if (!source.isPlaying)
			{
				source.outputAudioMixerGroup = mixGroup;
				source.PlayOneShot(clip, volume);
				played = true;
				break;
			}
		}

		// Make new source:
		if (!played && sources.Count < MAX_SOURCES)
		{
			var newSource = gameObject.AddComponent<AudioSource>();
			newSource.outputAudioMixerGroup = mixGroup;
			newSource.PlayOneShot(clip, volume);
			sources.Add(newSource);
		}
	}


	private void LowerVolume()
	{
		volumeOnPause = musicSource.volume;
		musicSource.volume = 0.25f;
	}

	private void ResetVolume()
	{
		musicSource.volume = volumeOnPause;
	}

	private void OnDestroy()
	{
		MenuUtilitySpace.PauseMenu.OnPauseStart -= LowerVolume;
		MenuUtilitySpace.PauseMenu.OnPauseEnd -= ResetVolume;

		SceneLoader.OnFadeExitStart -= StartMusicFadeOut;
	}
}
