using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TextSpace;
using GameplaySpace;

namespace PlayMenuSpace
{
	public class ButtonConvo : MonoBehaviour
	{
		public static ButtonConvo instance;
		private void Awake() => instance = this;

		[SerializeField] Button myButton;
		[SerializeField] Animator myAnimator;
		[SerializeField] GameObject convoObject;
		[SerializeField] AudioClip activationSound;

		const float waitMin = 30f;
		const float waitMid = 60f;
		const float waitMax = 120f;

		Coroutine waitingRoutine;

		private void Start()
		{
			if (GameManager.Instance.currentMode == GameManager.GameModes.freeDesign)
			{
				gameObject.SetActive(false);
				return;
			}
			
			if (Level.instance.CurrentLevelIndex == 0)
				StcConversationManager.ResetConversationData();

			DisableConvoAndStartWait();

			myButton.onClick.AddListener(ActivateButton);

			Furniture.OnCreatedFurniture += StcConversationManager.CheckAllAvailableConvosOnFurnCreated;
			Furniture.OnFurnitureDestroyed += StcConversationManager.CheckAllActiveOnDestroy;
			PlayerGameplay.OnStateChange += HandleInteractive;
		}

		private void HandleInteractive(PlayerGameplay.states newState)
		{
			bool setActive = PlayerGameplay.states.resting == newState;
			myButton.interactable = setActive;

			if (setActive)
				myAnimator.speed = 1f;
			else
				myAnimator.speed = 0f;
		}

		public void ActivateButton()
		{
			StcConversationManager.StartMidConversation();
			DisableConvoAndStartWait();
			StartCoroutine(DelayEnable());
		}

		IEnumerator DelayEnable()
		{
			var waitTime = Random.Range(waitMin, waitMax);
			yield return new WaitForSeconds(waitTime);

			while(StcConversationManager.AvailableConvosByName.Count <= 0)
			{
				waitTime = Random.Range(waitMin, waitMid);
				yield return new WaitForSeconds(waitTime);
			}
			
			EnableConvo();
		}

		private void EnableConvo()
		{
			if (waitingRoutine != null)
				StopCoroutine(waitingRoutine);

			if (convoObject.activeSelf)
				return;

			convoObject.SetActive(true);
			AudioManager.instance.PlaySFXOneShot(activationSound, AudioManager.instance.ClickGroup);
			myAnimator.Play("ScaleTween");
			myAnimator.speed = 1f;
		}

		public void DisableConvoAndStartWait()
		{
			convoObject.SetActive(false);
			if (waitingRoutine == null)
				StartCoroutine(DelayEnable());
		}

		public void HoverEnter()
		{
			if (myButton.interactable)
			{
				myAnimator.Play("ScaleTween");
				myAnimator.speed = 0;
			}
		}

		public void HoverExit()
		{
			if (myButton.interactable)
			{
				myAnimator.Play("ScaleTween");
				myAnimator.speed = 1f;
			}
		}

		private void OnDestroy()
		{
			Furniture.OnCreatedFurniture -= StcConversationManager.CheckAllAvailableConvosOnFurnCreated;
			Furniture.OnFurnitureDestroyed -= StcConversationManager.CheckAllActiveOnDestroy;
			PlayerGameplay.OnStateChange -= HandleInteractive;
		}

	}
}