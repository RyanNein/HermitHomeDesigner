using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuUtilitySpace
{
	public class PauseMenu : MonoBehaviour
	{
		public static PauseMenu instance;
		private void Awake() => instance = this;

		private static bool _activePause;
		public static bool ActivePause
		{
			get => _activePause;
			private set => _activePause = value;
		}

		private void Start()
		{
			DeactivateChildren();
		}

		public delegate void PauseEvents();
		public static event PauseEvents 
			OnPauseChange, 
			OnPauseEnd, 
			OnPauseStart;

		[SerializeField] GameObject firstMenu;
		[SerializeField] GameObject elements;

		private void Update()
		{
			if (SceneLoader.Instance.IsFading)
				return;

			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
			{
				if (!elements.gameObject.activeSelf)
					CreatePauseMenu();
				else
					DestroyPauseMenu();
			}
		}

		public void CreatePauseMenu()
		{
			firstMenu.gameObject.SetActive(true);
			elements.gameObject.SetActive(true);

			ActivePause = true;

			OnPauseChange?.Invoke();
			OnPauseStart?.Invoke();
			Time.timeScale = 0f;
		}

		public void DestroyPauseMenu()
		{
			DeactivateChildren();

			ActivePause = false;

			OnPauseChange?.Invoke();
			OnPauseEnd?.Invoke();
			Time.timeScale = 1f;
		}

		void OnDestroy()
		{
			ActivePause = false;
		}

		#region UTILITY:
		public void Fullscreen()
		{
			SceneLoader.Instance.ToggleFullscreen();
		}

		public void ReturnToMenu()
		{
			DestroyPauseMenu();
			SceneLoader.Instance.LaodMainMenu();
		}

		private void DeactivateChildren()
		{
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(false);
			}
		}
		#endregion
	}
}
