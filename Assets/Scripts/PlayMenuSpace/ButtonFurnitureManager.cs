using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameplaySpace;

namespace PlayMenuSpace
{
	public class ButtonFurnitureManager : MonoBehaviour
	{
		public static ButtonFurnitureManager instance;
		private void Awake() => instance = this;

		[SerializeField] private Transform butStart;
		[SerializeField] private GameObject furnButton;

		private List<GameObject> activeButtons = new List<GameObject>();
		public List<string>[] menuTabs;

		// Button spawn info:
		float buttonStartX, ButtonSpawnY;
		const int BUTTON_SPAWN_Z = 5;
		const float BUTTON_GAP_X = 1.3f;
		const float BUTTON_GAP_Y = 1.3f;
		const int BUTTON_COL_NUMBER = 5;
		const int BUTTONS_PER_PAGE = 10;

		// Menu info:
		private int currentMenuTab = 0;
		private int currentButtonIndex = 0;
		private int currentButtonPage = 0;
		public int NumberOfPages { get; private set; }

		void Start()
		{
			buttonStartX = butStart.position.x;
			ButtonSpawnY = butStart.position.y;

			activeButtons = new List<GameObject>();

			// fill menu tabls from json data in level singleton:
			menuTabs = new List<string>[4];
			{
				menuTabs[0] = new List<string>();
				menuTabs[1] = new List<string>();
				menuTabs[2] = new List<string>();
				menuTabs[3] = new List<string>();

				var localCurrentLevel = Level.instance.CurrentLevelIndex;
				if (GameManager.Instance.currentMode == GameManager.GameModes.freeDesign)
					localCurrentLevel = 5;

				foreach (KeyValuePair<string, Furniture.FurnData> furn in Furniture.AllFurnInfo)
				{
					if (furn.Value.available[localCurrentLevel] == '1')
						menuTabs[furn.Value.menuIndex].Add(furn.Key);
				}
			}

			ChangeMenuTab(0);
		}

		#region EVENTS AND PUBLIC CALLS

		public delegate void MenuEvents();
		public static event MenuEvents OnMenuTabChange;
		public static event MenuEvents OnMenuPageChange;

		public void ChangeMenuTab(int tabIndex)
		{
			DestroyAllButtons();

			currentMenuTab = tabIndex;
			currentButtonIndex = 0;
			currentButtonPage = 0;

			// calculate number of pages:
			{
				float rawPages = (float)menuTabs[currentMenuTab].Count / BUTTONS_PER_PAGE;
				int pages = Mathf.CeilToInt(rawPages);
				NumberOfPages = pages;
			}

			CreateAllButtons();

			OnMenuTabChange?.Invoke();
		}

		public void ChangeMenuPage(bool rightArrow)
		{
			DestroyAllButtons();

			if (rightArrow)
				currentButtonPage++;
			else
				currentButtonPage--;

			if (currentButtonPage > NumberOfPages - 1)
				currentButtonPage = 0;
			else if (currentButtonPage < 0)
				currentButtonPage = NumberOfPages - 1;

			currentButtonIndex = currentButtonPage * BUTTONS_PER_PAGE;

			CreateAllButtons();

			OnMenuPageChange?.Invoke();
		}

		#endregion

		private void CreateAllButtons()
		{
			int numberToSpawn = BUTTONS_PER_PAGE;

			if (currentButtonPage == NumberOfPages - 1)
			{
				int remainder = menuTabs[currentMenuTab].Count % BUTTONS_PER_PAGE;
				if (remainder != 0)
					numberToSpawn = remainder;
			}

			for (int i = 0; i < numberToSpawn; i++)
			{
				var newButton = Instantiate(furnButton,
					new Vector3(
						buttonStartX + (i % BUTTON_COL_NUMBER * BUTTON_GAP_X),
						ButtonSpawnY + (Mathf.FloorToInt((float)i / BUTTON_COL_NUMBER) * -BUTTON_GAP_Y),
						BUTTON_SPAWN_Z
					),
					Quaternion.identity, instance.transform);
				var butScript = newButton.GetComponent<ButtonFurniture>();
				butScript.furnType = menuTabs[currentMenuTab][currentButtonIndex];
				currentButtonIndex++;
				activeButtons.Add(newButton);
			}
		}

		private void DestroyAllButtons()
		{
			if (activeButtons.Count == 0)
				return;

			foreach (GameObject but in activeButtons)
			{
				Destroy(but);
			}
			activeButtons.Clear();
		}
	}
}