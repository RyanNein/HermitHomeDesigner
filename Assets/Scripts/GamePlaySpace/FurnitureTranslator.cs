using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplaySpace;

public static class FurnitureTranslator
{

	static TextAsset _furnatureCsv;

	public static TextAsset FurnatureCsv
	{
		get
		{
			if (_furnatureCsv == null)
				_furnatureCsv = Resources.Load<TextAsset>("FurnitureInfoCsv");
			return _furnatureCsv;
		}
	}

	static Dictionary<string, TranslationTable> _furnitureTranslations;
	public static Dictionary<string, TranslationTable> FurnitureTranslations
	{
		get
		{
			if (_furnitureTranslations == null)
			{
				Dictionary<string, TranslationTable> newDictionary = new Dictionary<string, TranslationTable>();

				// Get Grid Csv
				var rawCsv = FurnatureCsv.text;
				string[][] furnCsvGrid = NeinUtility.Utility.ConvertCsvToStringGrid(rawCsv, 3, false);

				// Convert To Dictionary:
				var numberOfRows = furnCsvGrid.Length;
				for (int i = 1; i < numberOfRows; i++)
				{
					var currentRow = furnCsvGrid[i];

					// Found Id, Create new table entry
					if (currentRow[0] != string.Empty)
					{
						TranslationTable translationTable = new TranslationTable();
						
						List<string> nameTranslations = new List<string>();
						List<string> descriptionTranslations = new List<string>();

						var nameRow = furnCsvGrid[i + 1];
						var descriptioinRow = furnCsvGrid[i + 2];

						nameTranslations.Add(nameRow[1]);
						nameTranslations.Add(nameRow[2]);
						descriptionTranslations.Add(descriptioinRow[1]);
						descriptionTranslations.Add(descriptioinRow[2]);

						translationTable.name = nameTranslations.ToArray();
						translationTable.description = descriptionTranslations.ToArray();

						newDictionary.Add(currentRow[0], translationTable);
					}
				}
				_furnitureTranslations = newDictionary;
			}

			return _furnitureTranslations;
		}
	}


	// Public Calls
	public static Furniture.FurnData Translate(Furniture.FurnData _data, string _furnId)
	{
		TranslationTable table = FurnitureTranslations[_furnId];
		int languageIndex = (int)GameManager.Instance.CurrentLanguage - 1;

		Furniture.FurnData translationFurnData = _data;

		translationFurnData.title = table.name[languageIndex];
		translationFurnData.description = table.description[languageIndex];

		return translationFurnData;
	}

	[System.Serializable]
	public class TranslationTable
	{
		public string[] name;
		public string[] description;
	}


}
