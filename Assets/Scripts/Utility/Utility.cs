using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeinUtility
{
	public static class Utility
	{

		public static bool IsMouseOverUI => UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

		public static void PrintArray<T>(T[] _array, bool _concatanate = false, string _seperator = ", ")
		{
			if (!_concatanate)
			{
				for (int i = 0; i < _array.Length; i++)
					Debug.Log(_array[i]);
			}
			else
			{
				string toPrint = "";

				for (int i = 0; i < _array.Length; i++)
				{
					toPrint += System.Convert.ToString(_array[i]);
					if (i < _array.Length - 1)
						toPrint += _seperator;
				}

				Debug.Log(toPrint);
			}
		}

		public static string[][] ConvertCsvToStringGrid(string _CsvString, int _numberOfColumns, bool _skipHeader)
		{
			// Get String array from file:
			List<string> fields = new List<string>();
			System.IO.StringReader stringReader = new System.IO.StringReader(_CsvString);
			using (var parser = new NotVisualBasic.FileIO.CsvTextFieldParser(stringReader))
			{
				while (!parser.EndOfData)
				{
					var csvLine = parser.ReadFields();

					for (int c = 0; c < _numberOfColumns; c++)
					{
						fields.Add(csvLine[c]);
					}
				}
			}
			string[] cells = fields.ToArray();

			// convert to grid:
			var numberOfCells = cells.Length;
			List<string[]> rowsList = new List<string[]>();
			var startingIndex = _skipHeader ? _numberOfColumns : 0;
			for (int i = startingIndex; i < numberOfCells; i += _numberOfColumns)
			{
				List<string> formattedlist = new List<string>();
				for (int j = i; j < i + _numberOfColumns; j++)
				{
					formattedlist.Add(cells[j]);
				}
				string[] formattedRow = formattedlist.ToArray();
				rowsList.Add(formattedRow);
			}

			string[][] csvGrid = rowsList.ToArray();
			return csvGrid;
		}

	}
}
