using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PlayMenuSpace
{
	public class ButtonArrow : MonoBehaviour
	{

		[SerializeField] bool isRight;
		[SerializeField] Button myButton;

		void Awake()
		{
			ButtonFurnitureManager.OnMenuTabChange += HandleIsActive;
		}

		void Start()
		{
			myButton.onClick.AddListener(() => ButtonFurnitureManager.instance.ChangeMenuPage(isRight));
		}

		private void HandleIsActive()
		{
			if (ButtonFurnitureManager.instance.NumberOfPages == 1)
				gameObject.SetActive(false);
			else
				gameObject.SetActive(true);
		}

		private void OnDestroy()
		{
			ButtonFurnitureManager.OnMenuTabChange -= HandleIsActive;
		}
	}
}