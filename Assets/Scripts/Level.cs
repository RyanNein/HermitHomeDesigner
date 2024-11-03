using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TextSpace;
using GameplaySpace;

public class Level : MonoBehaviour
{
	public static Level instance;
	private void Awake() => instance = this;

	public delegate void LevelEvents();
	public static event LevelEvents
		OnStartingTextEnd,
		OnEndingTextStart;

	[SerializeField] private GameObject nonGalleryObjects;
	[SerializeField] private GameObject logoObject;

	// SO:
	[SerializeField] LevelSO levelInfo;
	public int CurrentLevelIndex => levelInfo.levelIndex;
	private float MusiVolume => levelInfo.musicVolume;
	public Sprite PanelSprite => levelInfo.panelBackground;
	private AudioClip LevelMusic => levelInfo.levelMusic;
	private TextAsset LevelTextFile => levelInfo.levelStory;
	private int NextScene => levelInfo.nextScene;

	[SerializeField]
	string chapterStartId;
	[SerializeField]
	string chapterEndId;


	private void Start()
	{
		AudioManager.instance.PlayMusic(LevelMusic, MusiVolume);

		var mode = GameManager.Instance.currentMode;
		if (mode == GameManager.GameModes.gallery)
			StartGallery();
		else if (mode == GameManager.GameModes.normal)
			SceneLoader.OnFadeEnterEnd += BeginChapterStartText;
	}

	void StartGallery()
	{
		// Spawn from grid:
		GameManager.singleFurn[] allFurnsThisLevel = GameManager.Instance.allLevelSavedFurn[CurrentLevelIndex].allFurns;
		for(int i = 0; i < allFurnsThisLevel.Length; i++)
		{
			var furn = allFurnsThisLevel[i];
			Furniture.CreateFurniture(furn.furntype, furn.depth, furn.position);
		}

		// deactivate gameplay objects:
		nonGalleryObjects.gameObject.SetActive(false);
		logoObject.gameObject.SetActive(true);
	}


	private void BeginChapterStartText()
	{
		TextManager.Instance.InstantiateText(chapterStartId);
		TextManager.OnTextEnd += InvokeEndText;
	}

	public void BeginChapterEndText()
	{
		TextManager.Instance.InstantiateText(chapterEndId);
		TextManager.OnTextEnd += EndLevel;
		OnEndingTextStart?.Invoke();
	}

	private void InvokeEndText()
	{
		TextManager.OnTextEnd -= InvokeEndText;
		OnStartingTextEnd?.Invoke();
	}

	public void EndLevel()
	{
		GameManager.Instance.StoreActiveFurn(CurrentLevelIndex);

		if(GameManager.Instance.currentMode != GameManager.GameModes.normal)
		{
			GameManager.Instance.SaveFile();
			SceneLoader.Instance.LaodMainMenu();
		}
		else
		{
			if (CurrentLevelIndex == 5)
			{
				GameManager.Instance.hasFinishedGame = true;
				GameManager.Instance.SaveFile();
			}

			SceneLoader.Instance.LoadNewScene(NextScene);
		}
	}

	private void OnDestroy()
	{
		TextManager.OnTextEnd -= EndLevel;
		TextManager.OnTextEnd -= InvokeEndText;
		SceneLoader.OnFadeEnterEnd -= BeginChapterStartText;
	}
}