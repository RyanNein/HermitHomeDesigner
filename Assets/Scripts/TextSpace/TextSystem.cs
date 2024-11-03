using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TextSpace
{
	[RequireComponent(typeof(AudioSource))]
	public class TextSystem : MonoBehaviour
	{
		[SerializeField] AudioClip TypeSound;

		private WaitForSeconds typeWait = new WaitForSeconds(0.039f);
		private WaitForSeconds typeMidWait = new WaitForSeconds(.15f);
		private WaitForSeconds typeLongWait = new WaitForSeconds(0.28f);

		bool isDisplyingText { get { return DisplayRoutine != null; } }
		Coroutine DisplayRoutine = null;

		bool finishedTyping = false;
		bool click;

		public Page[] Story; // assign on instantion

		private void Start()
		{
			MenuUtilitySpace.PauseMenu.OnPauseStart += SetDisabled;
			MenuUtilitySpace.PauseMenu.OnPauseEnd += SetEnabled;

			DisplayRoutine = StartCoroutine(DisplayText(Story));
		}

		private void EndText()
		{
			TextManager.Instance.DestroyText(this);
		}

		IEnumerator DisplayText(Page[] story)
		{
			int currentPage = 0;
			int lastPage = story.Length - 1;
			finishedTyping = false;

			string targetSpeech;
			Coroutine typeRoutine;

			void StartPage()
			{
				textBox.text = "";
				targetSpeech = story[currentPage].text;
				targetSpeech = RemoveUnderscores(targetSpeech);
				typeRoutine = StartCoroutine(TypePage(targetSpeech));

				// name:
				speechName.enabled = story[currentPage].name;

				// sprite:
				var spr = story[currentPage].spr;
				if (spr != null)
					portrait.sprite = Resources.Load<Sprite>("Portraits/" + spr);

				// sfx
				var sfx = story[currentPage].sfx;
				if (sfx != null)
					AudioManager.instance.PlaySFXOneShot(Resources.Load<AudioClip>("SFX/" + sfx), AudioManager.instance.GeneralSfxgroup);
			}

			// Handle pages:
			bool finishedStory = false;
			StartPage();
			while (!finishedStory)
			{
				if (click)
				{
					if (typeRoutine != null)
						StopCoroutine(typeRoutine);
						
					// skip text:
					if (!finishedTyping)
					{
						FinishTyping();
					}

					// next page
					else
					{
						currentPage++;
						// Finish text:
						if(currentPage > lastPage)
							finishedStory = true;
						else
							StartPage();
					}
				}

				yield return new WaitForEndOfFrame();
			}

			StopDisplayingText();

			yield return null;
		}

		private void StopDisplayingText()
		{
			StopAllCoroutines();
			DisplayRoutine = null;

			EndText();
		}

		IEnumerator TypePage(string targetSpeech)
		{
			finishedTyping = false;
			dot.gameObject.SetActive(false);

			textBox.text = targetSpeech;

			int typeIndex = 0;
			int speechLength = targetSpeech.Length;

			var waited = false;
			
			while (typeIndex < speechLength)
			{
				typeIndex++;
				textBox.maxVisibleCharacters = typeIndex;
				var newChar = targetSpeech[typeIndex-1];

				if ((typeIndex-1) % 2 == 0 || waited)
					AudioManager.instance.PlaySFXOneShot(TypeSound, AudioManager.instance.TypeGroup);

				var wait = typeWait;
				if (newChar == ',')
				{
					wait = typeMidWait;
					waited = true;
				}
				else if (newChar == '.' || newChar == '?' || newChar == '!')
				{
					wait = typeLongWait;
					waited = true;
				}
				else
				{
					waited = false;
				}

				yield return wait;
			}

			FinishTyping();
		}

		private void FinishTyping()
		{
			textBox.maxVisibleCharacters = textBox.text.Length;
			finishedTyping = true;
			dot.gameObject.SetActive(true);
		}

		private string RemoveUnderscores(string _pageText)
		{
			string text = _pageText;

			if (text.Contains("_"))
				text = "";

			return text;
		}

		private void Update()
		{
			click = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
		}

		private void SetEnabled()
		{
			enabled = true;
		}

		private void SetDisabled()
		{
			enabled = false;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();

			MenuUtilitySpace.PauseMenu.OnPauseStart -= SetDisabled;
			MenuUtilitySpace.PauseMenu.OnPauseEnd -= SetEnabled;
		}

		#region ELEMENTS:

		[System.Serializable]
		class ELEMENTS
		{
			public GameObject speachPanel;
			public TextMeshProUGUI speechText;
			public Image speechName;
			public Image portrait;
			public GameObject dot;
		}
		[SerializeField] private ELEMENTS Elements;
		private GameObject speechPanel { get { return Elements.speachPanel; } }
		private TextMeshProUGUI textBox { get { return Elements.speechText; } }
		private Image speechName { get { return Elements.speechName; } }
		private Image portrait { get { return Elements.portrait; } }
		private GameObject dot { get { return Elements.dot; } }

		#endregion
	}
}