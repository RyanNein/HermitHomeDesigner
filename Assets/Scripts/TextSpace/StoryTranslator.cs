using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextSpace
{
	public static class StoryTranslator
	{
		private static TextAsset _translationsCsv;
		private static TextAsset TranslationsCsv
		{
			get
			{
				if (_translationsCsv == null)
				{
					_translationsCsv = Resources.Load<TextAsset>("DialoguesTranslationCsv");
				}

				return _translationsCsv;
			}
		}

		private static Dictionary<string, TranslationTable> _allTranslations;
		public static Dictionary<string, TranslationTable> AllTranslations
		{
			get
			{
				if (_allTranslations == null)
				{
					Dictionary<string, TranslationTable> newDictionary = new Dictionary<string, TranslationTable>();

					var rawCsv = TranslationsCsv.text;
					string[][] formattedCsv = NeinUtility.Utility.ConvertCsvToStringGrid(rawCsv, 3, false);

					var numberOfRows = formattedCsv.Length;
					for (int i = 1; i < numberOfRows; i++)
					{
						var currentRow = formattedCsv[i];

						// Found Id, start a new section
						if (currentRow[0] != string.Empty)
						{
							TranslationTable translationTable = new TranslationTable();
							List<string> english = new List<string>();
							List<string> spanish = new List<string>();

							var startRow = i + 1;
							for (int j = startRow; j < numberOfRows; j++)
							{
								var jRow = formattedCsv[j];

								//check for new convo
								if (jRow[1] == string.Empty)
								{
									break;
								}
								else
								{
									english.Add(jRow[1]);
									spanish.Add(jRow[2]);
								}
							}
							translationTable.translations = new string[][] { english.ToArray(), spanish.ToArray() };
							newDictionary.Add(currentRow[0], translationTable);
						}
					}

					_allTranslations = newDictionary;
				}

				return _allTranslations;
			}
		}


		// Public Calls
		public static Story Translate(Story _storyToTranslate, string _id)
		{
			TranslationTable table = AllTranslations[_id];
			int languageIndex = (int)GameManager.Instance.CurrentLanguage-1;
			Story translationStory = _storyToTranslate;

			var numberOfPages = translationStory.pages.Length;
			for (int i = 0; i < numberOfPages; i++)
			{
				translationStory.pages[i].text = table.translations[languageIndex][i];
			}

			return translationStory;
		}


		[System.Serializable]
		public class TranslationTable
		{
			public string[][] translations;
		}
	}
}