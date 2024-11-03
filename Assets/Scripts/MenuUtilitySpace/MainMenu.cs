using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuUtilitySpace
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField] private GameObject GalleryMenu;
		[SerializeField] private GameObject FreeDesignMenu;
		[SerializeField] private GameObject FirstMenu;
		[SerializeField] private GameObject LanguageMenu;


		[SerializeField] private GameObject GalleryButton;
		[SerializeField] private GameObject FreeDesignButton;

		[SerializeField] private AudioClip MenuMusic;

		private void Start()
		{
			AudioManager.instance.PlayMusic(MenuMusic);

			if (!GameManager.Instance.LanguageSelected)
			{
				LanguageMenu.SetActive(true);
				FirstMenu.SetActive(false);
			}
			else
			{
				LanguageMenu.SetActive(false);
				FirstMenu.SetActive(true);
			}

			if (!GameManager.Instance.hasFinishedGame)
			{
				GalleryButton.GetComponent<Button>().interactable = false;
				FreeDesignButton.GetComponent<Button>().interactable = false;
			}
		}

		public void StartGame()
		{
			GameManager.Instance.currentMode = GameManager.GameModes.normal;
			SceneLoader.Instance.LoadNewScene(1);
			foreach (Transform child in FirstMenu.transform)
				child.GetComponent<Button>().interactable = false;
		}

		public void EnterGalleryLevel(int levelIndex)
		{
			GameManager.Instance.currentMode = GameManager.GameModes.gallery;
			SceneLoader.Instance.LoadNewScene(levelIndex);
			foreach (Transform child in GalleryMenu.transform)
				child.GetComponent<Button>().interactable = false;
		}

		public void EnterFreeDesignLevel(int levelIndex)
		{
			GameManager.Instance.currentMode = GameManager.GameModes.freeDesign;
			SceneLoader.Instance.LoadNewScene(levelIndex);
			foreach (Transform child in FreeDesignMenu.transform)
				child.GetComponent<Button>().interactable = false;
		}

		public void EndGame()
		{
			SceneLoader.Instance.ExitGame();
		}

		public void Fullscreen()
		{
			SceneLoader.Instance.ToggleFullscreen();
		}

		public void SelectLanguage(int _languageEnumeration)
		{
			GameManager.Instance.SetLanguage((GameManager.Languages)_languageEnumeration);
		}

		public void LaunchCreditsLink()
		{
			Application.OpenURL("https://twitter.com/Ryan_Nein");
		}
	}
}