using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayMenuSpace;

public class PlayMenu : MonoBehaviour
{
	[SerializeField] GameObject finishButton;
	[SerializeField] GameObject notGalleryFolder;
	[SerializeField] GameObject logoObject;
	[SerializeField] ButtonFurnitureManager buttonManager;

	private void Awake()
	{
		DisableLevelObjects();
	}

	private void Start()
	{
		var mode = GameManager.Instance.currentMode;
		if (mode == GameManager.GameModes.gallery)
		{
			finishButton.SetActive(true);
			logoObject.SetActive(true);
		}
		else if (mode == GameManager.GameModes.freeDesign)
		{
			EnableLevelObjects();
		}
		else
		{
			Level.OnStartingTextEnd += EnableLevelObjects;
			Level.OnEndingTextStart += DisableLevelObjects;
		}
	}

	void EnableLevelObjects()
	{
		finishButton.SetActive(true);
		notGalleryFolder.SetActive(true);
	}

	void DisableLevelObjects()
	{
		finishButton.SetActive(false);
		notGalleryFolder.SetActive(false);
		logoObject.SetActive(false);
	}

	private void OnDestroy()
	{
		Level.OnStartingTextEnd -= EnableLevelObjects;
		Level.OnEndingTextStart -= DisableLevelObjects;
	}
}
