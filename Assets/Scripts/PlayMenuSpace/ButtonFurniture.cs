using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GameplaySpace;

namespace PlayMenuSpace
{
	public class ButtonFurniture : MonoBehaviour
	{
		public string furnType;

		[SerializeField] private Button myButton;
		[SerializeField] private Image myImage;

		void Start()
		{
			myButton.onClick.AddListener(ActivateButton);

			var spr = Resources.Load<Sprite>("FurnitureSprites/" + Furniture.AllFurnInfo[furnType].sprite);
			myImage.sprite = spr;

			// Set Sprite and Size:
			{
				var rect = spr.rect;

				var smallest = rect.width;
				var largest = rect.height;

				if (smallest > largest)
				{
					smallest = rect.height;
					largest = rect.width;
				}

				if (smallest < 36 && largest < 50)
					myImage.rectTransform.sizeDelta = new Vector2(50f, 50f);
				else if (smallest < 80)
					myImage.rectTransform.sizeDelta = new Vector2(80f, 80f);
			}
		}

		public void ActivateButton()
		{
			Furniture.CreateFurniture(furnType);
		}

		public void OnHoverEnter()
		{
			if (myButton.interactable)
			{
				DescriptionPanel.instance.InitiateDescription(furnType);
			}
		}

		public void OnHoverExit()
		{
			DescriptionPanel.instance.StopDescription();
		}
	}
}