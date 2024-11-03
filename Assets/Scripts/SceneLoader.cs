using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : NeinUtility.PersistentSingleton<SceneLoader>
{

	protected override void Awake()
	{
		base.Awake();
	}

	[SerializeField] Animator fadeAnimator;
	[SerializeField] Canvas myCanvas;

	private void Start()
	{
		GetActiveCamera();
		StartCoroutine(EnterFadeWait());

		SceneManager.sceneLoaded += Instance.OnSceneLoaded;
	}

	#region UTILITY:

	public void LaodMainMenu()
	{
		LoadNewScene(0);
	}

	public void ToggleFullscreen()
	{
		var toFull = !Screen.fullScreen;
		// Screen.fullScreen = full;

		if (!toFull)
		{
			Screen.SetResolution(1280, 720, false);
		}
		else
		{
			Screen.SetResolution(1920, 1080, true);

			/*
			StartCoroutine(SetOverTime());

			IEnumerator SetOverTime()
			{
				Screen.fullScreen = true;
				yield return new WaitForEndOfFrame();
				Screen.SetResolution(1920, 1080, true);
			}
			*/

			/*
			var res = Screen.currentResolution;
			print(res.height);
			Screen.SetResolution(res.width, res.height, true);
			*/
		}

		/*
		if (!full)
		{
			Screen.SetResolution(1280, 720, false);
		}
		else
		{
			var res = Screen.currentResolution;
			Screen.SetResolution(res.width, res.height, true);
		}
		*/
	}

	public void LoadNewScene(int sceneIndex)
	{
		StartCoroutine(ExitSceneTransition(sceneIndex));
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	#endregion

	// ========= Fade ===========\\

	public delegate void SceneEvents();
	public static event SceneEvents
		OnSceneStart,
		OnFadeEnterEnd,
		OnFadeExitStart;

	private bool _isFading;
	public bool IsFading
	{
		get { return _isFading; }
		private set { _isFading = value; }
	}

	IEnumerator ExitSceneTransition(int sceneIndex)
	{
		fadeAnimator.Play("CrossfadeExit");
		IsFading = true;
		OnFadeExitStart?.Invoke();
		yield return new WaitForSeconds(1f);

		SceneManager.LoadScene(sceneIndex);
	}

	IEnumerator EnterFadeWait()
	{
		yield return new WaitForSeconds(1f);
		IsFading = false;
		OnFadeEnterEnd?.Invoke();
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		GetActiveCamera();
		fadeAnimator.Play("CrossfadeEnter");
		IsFading = true;
		OnSceneStart?.Invoke();
		StartCoroutine(EnterFadeWait());
	}

	void GetActiveCamera()
	{
		myCanvas.worldCamera = Camera.main;
	}
}
