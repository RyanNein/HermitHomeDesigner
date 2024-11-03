using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PlayMenuSpace
{
	public class ButtonTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] [Range(0, 3)] public int tabIndex;

		[SerializeField] Button myButton;
		[SerializeField] RectTransform myTransform;

		[SerializeField] AudioClip hoverClip;
		[SerializeField] AudioClip clickClip;

		GameplaySpace.PlayerGameplay.states activeState = GameplaySpace.PlayerGameplay.states.resting;

		[SerializeField] bool isCurrentlyActiveTab;
		[SerializeField] Vector3 defaultScale;
		[SerializeField] float scaleMult = 1.25f;

		void Awake()
		{
			defaultScale = transform.localScale;
			myButton.onClick.AddListener(ActivateButton);
		}

		private void Start()
		{
			GameplaySpace.PlayerGameplay.OnStateChange += HandleInteractive;
			ButtonFurnitureManager.OnMenuTabChange += ResetScale;
			
			if (tabIndex == 0)
				SetCurrentlyActive();
		}

		private void ActivateButton()
		{
			AudioManager.instance.PlaySFXOneShot(clickClip, AudioManager.instance.ClickGroup);
			ButtonFurnitureManager.instance.ChangeMenuTab(tabIndex);
			foreach (Transform tabChild in transform.parent)
			{
				tabChild.GetComponent<ButtonTab>().isCurrentlyActiveTab = false;
			}
			SetCurrentlyActive();
			foreach(Transform tabChild in transform.parent)
			{
				tabChild.GetComponent<ButtonTab>().ResetScale();
			}
		}

		private void SetCurrentlyActive()
		{
			myTransform.localScale = defaultScale * scaleMult;
			myButton.interactable = false;
			isCurrentlyActiveTab = true;
		}

		private void ResetScale()
		{
			if (isCurrentlyActiveTab)
				return;

			myTransform.localScale = defaultScale;
			HandleInteractive(GameplaySpace.PlayerGameplay.instance.state);
			isCurrentlyActiveTab = false;
		}

		private void HandleInteractive(GameplaySpace.PlayerGameplay.states newState)
		{
			if (isCurrentlyActiveTab)
			{
				myButton.interactable = false;
			}
			else
			{
				myButton.interactable = activeState == newState;
			}
		}

		void OnDestroy()
		{
			ButtonFurnitureManager.OnMenuTabChange -= ResetScale;
			GameplaySpace.PlayerGameplay.OnStateChange -= HandleInteractive;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (myButton.interactable && !isCurrentlyActiveTab)
			{
				transform.localScale = defaultScale * 1.1f;
				AudioManager.instance.PlaySFXOneShot(hoverClip, AudioManager.instance.HoverGroup);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!isCurrentlyActiveTab)
			{
				ResetScale();
			}
		}
	}
}