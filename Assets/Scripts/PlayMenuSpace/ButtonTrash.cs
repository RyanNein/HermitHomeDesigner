using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GameplaySpace;

namespace PlayMenuSpace
{
	public class ButtonTrash : MonoBehaviour
	{
		[SerializeField] public Button myButton;

		public static ButtonTrash instance;
		private void Awake() => instance = this;

		private void Start()
		{
			myButton.interactable = false;
			myButton.onClick.AddListener(TrashHeldFurn);
		}

		private void TrashHeldFurn()
		{
			PlayerGameplay.instance.DestroyHeldFurn();
		}
	}
}