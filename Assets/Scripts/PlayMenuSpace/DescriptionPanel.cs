using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameplaySpace;

namespace PlayMenuSpace
{
	public class DescriptionPanel : MonoBehaviour
	{
		public static DescriptionPanel instance;
		private void Awake() => instance = this;

		[SerializeField] private TextMeshProUGUI titleTextBox;
		[SerializeField] private TextMeshProUGUI descriptionTextBox;

		private static float typeSpeed = 0.05f;
		private WaitForSeconds typeWait = new WaitForSeconds(typeSpeed);

		Coroutine typingCoroutine = null;

		private void Start()
		{
			StopDescription();
		}

		public void InitiateDescription(string _furnId)
		{
			// get furn
			Furniture.FurnData furnData = Furniture.AllFurnInfo[_furnId];

			// translate furn
			furnData = FurnitureTranslator.Translate(furnData, _furnId);
			var translatedName = furnData.title;
			var translatedDescription = furnData.description;

			YamasenPanel.instance.StartTalking();
			StopAllCoroutines();
			typingCoroutine = StartCoroutine(TypeDescription(translatedName, translatedDescription));
		}

		public void StopDescription()
		{
			YamasenPanel.instance.EndTalking();

			titleTextBox.text = null;

			if (typingCoroutine != null)
				StopCoroutine(typingCoroutine);
			descriptionTextBox.text = string.Empty;
		}

		IEnumerator TypeDescription(string titleText, string targetSpeech)
		{
			titleTextBox.text = titleText;

			if (string.IsNullOrEmpty(targetSpeech))
			{
				YamasenPanel.instance.EndTalking();
				yield return null;
			}
			else
			{
				int visIndex = 0;
				int length = targetSpeech.Length;

				descriptionTextBox.text = targetSpeech;

				while (visIndex < length)
				{
					visIndex++;
					descriptionTextBox.maxVisibleCharacters = visIndex;
					yield return typeWait;
				}
			}

			YamasenPanel.instance.EndTalking();
			yield return null;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}
	}
}