using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PlayMenuSpace
{
	[RequireComponent(typeof(EventTrigger))]
	public class ButtonHoverScaler : MonoBehaviour
	{
		private Button myButton;

		[SerializeField] float scaleFactor = 1.1f;
		Vector3 defaultScale;

		[SerializeField] AudioClip hoverClip;
		[SerializeField] AudioClip clickClip;

		private void Awake()
		{
			myButton = GetComponent<Button>();
			defaultScale = myButton.transform.localScale;
		}

		private void OnEnable()
		{
			myButton.onClick.AddListener(SelectionPlay);
		}

		private void OnDisable()
		{
			myButton.onClick.RemoveListener(SelectionPlay);
		}



		public void OnHoverScale()
		{
			if (myButton.interactable)
			{
				transform.localScale = defaultScale * scaleFactor;
				AudioManager.instance.PlaySFXOneShot(hoverClip, AudioManager.instance.HoverGroup);
			}
		}

		public void OnhoverExitScale()
		{
			transform.localScale = defaultScale;
		}

		private void SelectionPlay()
		{
			AudioManager.instance.PlaySFXOneShot(clickClip, AudioManager.instance.ClickGroup);
		}
	}
}