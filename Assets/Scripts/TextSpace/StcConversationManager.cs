using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using GameplaySpace;

namespace TextSpace
{
	public static class StcConversationManager
	{

		// Files:
		private static TextAsset _chapterDialogueJson;
		private static TextAsset ChapterDialogueJson
		{
			get
			{
				if (_chapterDialogueJson == null)
					_chapterDialogueJson = Resources.Load<TextAsset>("AllChapters");
				return _chapterDialogueJson;
			}
		}

		private static TextAsset _midLevelConvoJsonFile;
		private static TextAsset MidLevelConvoJsonFile
		{
			get
			{
				if (_midLevelConvoJsonFile == null)
					_midLevelConvoJsonFile = Resources.Load<TextAsset>("MidLevelText");
				return _midLevelConvoJsonFile;
			}
		}


		// All Stories
		private static Dictionary<string, Story> _allStories;
		public static Dictionary<string, Story> AllStoriesData
		{
			get
			{
				_allStories ??= PopulateAllStories();
				return _allStories;
			}
		}


		// All Mid Level Conversations:
		private static Dictionary<string, MidLevelConversation> _allConversations;
		public static Dictionary<string, MidLevelConversation> AllConversations
		{
			get
			{
				if (_allConversations == null)
				{
					_allConversations = new Dictionary<string, MidLevelConversation>();
					ResetConversationData();
				}
				return _allConversations;
			}
		}

		private static List<string> _availableConvosByName;
		public static List<string> AvailableConvosByName
		{
			get
			{
				if (_availableConvosByName == null)
				{
					_availableConvosByName = new List<string>();
				}
				return _availableConvosByName;
			}
		}


		#region PUBLIC CALLS:

		public static void StartMidConversation()
		{
			var allConvos = AllConversations;
			var availConvos = AvailableConvosByName;
			var length = availConvos.Count;

			var index = Random.Range(0, length);

			TextManager.Instance.InstantiateText(allConvos[availConvos[index]].id);

			allConvos[availConvos[index]].used = true;
			availConvos.RemoveAt(index);
		}

		public static void ResetAvailableConvos()
		{
			AvailableConvosByName.Clear();
		}
		
		public static void ResetConversationData()
		{
			var allconvos =	JsonConvert.DeserializeObject<Dictionary<string, MidLevelConversation>>(MidLevelConvoJsonFile.text);
			
			// Populate With Ids
			foreach (KeyValuePair<string, MidLevelConversation> convo in allconvos)
				convo.Value.id = convo.Key;

			_allConversations = allconvos;
		}

		public static void CheckAllAvailableConvosOnFurnCreated(Furniture furn)
		{
			var matchingList = CheckConditionsSingleFurn(furn);

			for(int i = 0; i < matchingList.Count; i++)
			{
				var currentConvo = matchingList[i];
				if (!AvailableConvosByName.Contains(currentConvo))
				{
					AvailableConvosByName.Add(currentConvo);
				}
			}
		}

		public static void CheckAllActiveOnDestroy(Furniture furn)
		{
			CheckAvailableConvoAllActiveFurn();

			if (AvailableConvosByName.Count <= 0)
			{
				PlayMenuSpace.ButtonConvo.instance.DisableConvoAndStartWait();
			}
		}

		#endregion

		private static Dictionary<string, Story> PopulateAllStories()
		{
			var stories = JsonConvert.DeserializeObject<Dictionary<string, Story>>(ChapterDialogueJson.text);
			foreach (var convo in AllConversations)
			{
				stories.Add(convo.Key, convo.Value.story);
			}
			
			return stories;
		}

		private static void CheckAvailableConvoAllActiveFurn()
		{
			var availConvos = AvailableConvosByName;
			var activeFurn = Furniture.ActiveFurniture;
			var furnCount = activeFurn.Count;

			// Check each convo in list
			for (int i = 0; i < availConvos.Count; i++)
			{
				var currentConvo = availConvos[i];
				bool convoConditionsMet = false;

				// Check each furn
				for (int j = 0; j < furnCount; j++)
				{
					var matchingList = CheckConditionsSingleFurn(activeFurn[j]);
					if (matchingList.Contains(currentConvo))
					{
						convoConditionsMet = true;
						continue;
					}
				}

				if (!convoConditionsMet)
				{
					availConvos.Remove(currentConvo);
				}
			}
		}

		private static List<string> CheckConditionsSingleFurn(Furniture furn)
		{
			var newFurnType = furn.furnType;
			var levelIndex = Level.instance.CurrentLevelIndex;

			var matchingConvos = new List<string>();

			foreach (KeyValuePair<string, MidLevelConversation> con in AllConversations)
			{
				if (con.Value.used == true)
					continue;

				bool roomConditionsMet, ItemConditionsMet;

				// Room check:
				roomConditionsMet = true;
				int[] rooms = con.Value.targetRooms;
				if (rooms != null)
				{
					roomConditionsMet = false;

					for (int i = 0; i < rooms.Length; i++)
					{
						if (rooms[i] == levelIndex)
						{
							roomConditionsMet = true;
							break;
						}
					}
				}

				// Furn Check:
				ItemConditionsMet = true;
				string[] items = con.Value.targetItems;
				if (items != null)
				{
					ItemConditionsMet = false;

					for (int i = 0; i < items.Length; i++)
					{
						if (items[i] == newFurnType)
						{
							ItemConditionsMet = true;
							break;
						}
					}
				}

				// Add to list:
				if (roomConditionsMet && ItemConditionsMet)
					matchingConvos.Add(con.Key);
			}

			return matchingConvos;
		}


		[System.Serializable]
		public class MidLevelConversation
		{
			public string id;
			public Story story;
			public int[] targetRooms;
			public string[] targetItems;
			public bool used;
		}

	}
}