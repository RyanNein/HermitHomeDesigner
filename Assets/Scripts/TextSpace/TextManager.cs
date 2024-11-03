using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextSpace;

public class TextManager : NeinUtility.PersistentSingleton<TextManager>
{
	protected override void Awake()
	{
		base.Awake();
	}

	public delegate void TextEvents();
	public static event TextEvents
		OnTextStart,
		OnTextEnd;

	[SerializeField] GameObject TextObject;
	private TextSystem textInstance;
	public bool TextIsActive => textInstance != null;

	public void InstantiateText(string _storyId)
	{
		// Get Story
		Story story = StcConversationManager.AllStoriesData[_storyId];

		// Translate Story
		Story TranslatedStory = StoryTranslator.Translate(story, _storyId);

		// Instantiate Text Object
		{
			GameObject objectInstance = Instantiate(TextObject);
			objectInstance.GetComponent<Canvas>().worldCamera = Camera.main;
			textInstance = objectInstance.GetComponent<TextSystem>();
		}

		textInstance.Story = TranslatedStory.pages;

		OnTextStart?.Invoke();
	}


	public void DestroyText(TextSystem textInst)
	{
		Destroy(textInst.gameObject);
		textInstance = null;

		OnTextEnd?.Invoke();
	}

}