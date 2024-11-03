using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MenuTranslator : MonoBehaviour
{

	// csv parsing values
	static int rowsToSkip = 2;
	static int englishColumn = 1;
	static int spanishColumn = 2;

	// import csv from resources
	static TextAsset menuCsv;
	public static TextAsset MenuCsv
	{
		get
		{
			if (menuCsv == null)
			{
				menuCsv = Resources.Load<TextAsset>("MenuStrings");
			}
			return menuCsv;
		}
	}


	// english to spanish dictionary from csv
	static Dictionary<string, string> menuStrings;
	static Dictionary<string, string> MenuStrings
	{
		get
		{
			if (menuStrings == null)
			{
				var parsedMenu = new Dictionary<string, string>();
				var csv = MenuCsv.text;

				string[] rowStrings = csv.Split(new char[] { '\n' });
				for (int i = rowsToSkip; i < rowStrings.Length-1; i++)
				{
					var currentRow = rowStrings[i];
					var colums = currentRow.Split(new char[] { ',' });

					parsedMenu.Add(colums[englishColumn].Trim(), colums[spanishColumn].Trim());
				}

				menuStrings = parsedMenu;
			}

			return menuStrings;
		}
	
	}


	// -------------------------------- INSTANCE ----------------- \\

	[SerializeField]
	string defaultButtonString;

	TextMeshProUGUI buttonText;

	private void OnValidate()
	{
		defaultButtonString = GetComponent<TextMeshProUGUI>().text.Trim();
		GetComponent<TextMeshProUGUI>().text = defaultButtonString;
	}


	private void Awake()
	{
		buttonText = GetComponent<TextMeshProUGUI>();
	}

	private void OnEnable()
	{
		var language = GameManager.Instance.CurrentLanguage;

		if (language == GameManager.Languages.spanish)
		{
			if (!MenuStrings.ContainsKey(defaultButtonString))
			{
				Debug.LogWarning($"Menu button name key \"{defaultButtonString}\" not found");
				return;
			}
			else
			{
				buttonText.text = MenuStrings[defaultButtonString];
			}
		}
		else if (language == GameManager.Languages.english)
		{
			buttonText.text = defaultButtonString;
		}
		else
		{
			Debug.LogWarning("no language");
		}
	}

}
