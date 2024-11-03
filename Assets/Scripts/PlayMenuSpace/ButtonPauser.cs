using UnityEngine;
using UnityEngine.UI;

using GameplaySpace;

namespace PlayMenuSpace
{
	public class ButtonPauser : MonoBehaviour
	{
		[SerializeField] PlayerGameplay.states activeState;
		[SerializeField] Button myButton;

		private void OnEnable()
		{
			PlayerGameplay.OnStateChange += HandleInteractive;
		}
		private void OnDisable()
		{
			PlayerGameplay.OnStateChange -= HandleInteractive;
		}

		private void HandleInteractive(PlayerGameplay.states newState)
		{
			myButton.interactable = activeState == newState;
		}
	}
}