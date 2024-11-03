using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GameplaySpace;

public class GameManager : NeinUtility.PersistentSingleton<GameManager>
{

	protected override void Awake()
	{
		base.Awake();
		_currentLanguage = Languages.none;
		UnpackSaveFile();
	}

	// important stuff to save:
	Save SaveData;
	public bool hasFinishedGame;
	public SavedFurns[] allLevelSavedFurn = new SavedFurns[6];  // TODO: check for if save data is loaded

	public enum GameModes : short { normal, gallery, freeDesign }
	public GameModes currentMode = GameModes.normal;

	// Language
	public enum Languages : short { none, english, spanish }
	Languages _currentLanguage = Languages.none;
	public Languages CurrentLanguage
	{
		get
		{
			return _currentLanguage;
		}
	}

	public bool LanguageSelected => _currentLanguage != Languages.none;
	public void SetLanguage(Languages _languageEnumeration)
	{
		_currentLanguage = _languageEnumeration;
	}


	#region FURN GRIDS:

	public void StoreActiveFurn(int levelIndex)
	{
		var activeFurn = Furniture.ActiveFurniture;

		var saveFurn = new SavedFurns();
		saveFurn.allFurns = new singleFurn[activeFurn.Count];

		for (int i = 0; i < activeFurn.Count; i++) 
		{
			saveFurn.allFurns[i] = new singleFurn();
			saveFurn.allFurns[i].furntype = activeFurn[i].furnType;
			saveFurn.allFurns[i].position = activeFurn[i].transform.position;
			saveFurn.allFurns[i].depth = activeFurn[i].SortDepth;
		}

		allLevelSavedFurn[levelIndex] = saveFurn;
	}

	[System.Serializable]
	public class SavedFurns
	{
		public singleFurn[] allFurns;
	}

	[System.Serializable]
	public class singleFurn
	{
		public string furntype;
		public Vector2 position;
		public int depth;
	}

	#endregion

	#region SAVE:

	public void SaveFile()
	{
		var newSave = new Save(hasFinishedGame, _currentLanguage, allLevelSavedFurn);
		var json = JsonUtility.ToJson(newSave);
		File.WriteAllText(Application.dataPath + "/SaveFile.json", json);
	}

	private void UnpackSaveFile()
	{
		string json;

		string path = Application.dataPath + "/SaveFile.json";
		if (File.Exists(path))
		{
			json = File.ReadAllText(path);
		}
		else
		{
			TextAsset savedFile = Resources.Load<TextAsset>("DefaultSaveFile");
			json = savedFile.text;
		}

		SaveData = JsonUtility.FromJson<Save>(json);

		hasFinishedGame = SaveData.hasFinishedGame;
		_currentLanguage = SaveData.selectedLanguage;
		allLevelSavedFurn = SaveData.allLevelSavedFurn;
	}

	[System.Serializable]
	private class Save
	{
		public Save(bool _hasFinishedGame, Languages _selectedLanguage, SavedFurns[] _allLevelSavedFurn)
		{
			hasFinishedGame = _hasFinishedGame;
			selectedLanguage = _selectedLanguage;
			allLevelSavedFurn = _allLevelSavedFurn;
		}

		public bool hasFinishedGame;
		public Languages selectedLanguage;
		public SavedFurns[] allLevelSavedFurn;
	}

	#endregion


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			SceneLoader.Instance.ToggleFullscreen();
		}


		///////////////////////////////////////
		if (Input.GetKeyDown(KeyCode.S))
		{
			Screen.SetResolution(1280, 720, false);
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			Screen.SetResolution(1920, 1080, false);
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
	}
	
}
