using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GameplaySpace;

namespace PlayMenuSpace
{
	public class ButtonFinish : MonoBehaviour
	{
		[SerializeField] public Button myButton;

		public static ButtonFinish instance;
		private void Awake() => instance = this;

		private void Start()
		{
			myButton.interactable = false;

			if (GameManager.Instance.currentMode == GameManager.GameModes.gallery)
			{
				myButton.onClick.AddListener(Level.instance.EndLevel);
				myButton.interactable = true;
			}
			else if (GameManager.Instance.currentMode == GameManager.GameModes.freeDesign)
			{
				myButton.onClick.AddListener(Level.instance.EndLevel);
				PlayerGameplay.OnStateChange += HandleInteractive;
			}
			else
			{
				myButton.onClick.AddListener(Level.instance.BeginChapterEndText);
				PlayerGameplay.OnStateChange += HandleInteractive;
				Furniture.OnFurnitureDestroyed += CheckFurnExist;
			}
		}

		private void HandleInteractive(PlayerGameplay.states newState)
		{
			bool active = newState == PlayerGameplay.states.resting;

			if (Furniture.ActiveFurniture.Count == 0)
				active = false;

			myButton.interactable = active;
		}

		private void CheckFurnExist(Furniture furn)
		{
			if (Furniture.ActiveFurniture.Count == 0)
				myButton.interactable = false;
		}

		private void OnDestroy()
		{
			PlayerGameplay.OnStateChange -= HandleInteractive;
			Furniture.OnFurnitureDestroyed -= CheckFurnExist;
		}
	}
}