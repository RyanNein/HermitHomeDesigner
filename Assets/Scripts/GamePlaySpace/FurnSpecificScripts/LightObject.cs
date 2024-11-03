using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplaySpace.FurnitureSpace
{
	public class LightObject : MonoBehaviour
	{

		[SerializeField] Furniture myFurniture;
		[SerializeField] SpriteRenderer lightRenderer;
		[SerializeField] bool requiresPower;

		void Start()
		{
			FurnPowerGenerator.OnGeneratorPlaced += CheckLightEnabled;
			FurnPowerGenerator.OnGeneratorRemoved += CheckLightEnabled;
			TurnOnOff(false);
		}

		void OnDestroy()
		{
			FurnPowerGenerator.OnGeneratorPlaced -= CheckLightEnabled;
			FurnPowerGenerator.OnGeneratorRemoved -= CheckLightEnabled;
		}

		public void OnPlacement()
		{
			CheckLightEnabled();
		}

		public void OnPickup()
		{
			print("up");
			TurnOnOff(false);
		}

		void CheckLightEnabled()
		{
			if (Level.instance.CurrentLevelIndex >= 3 && requiresPower)
			{
				var turnOn = FurnPowerGenerator.ActiveGeneratorExists;
				TurnOnOff(turnOn);
			}
			else
			{
				TurnOnOff(true);
			}
		}

		void TurnOnOff(bool on)
		{
			lightRenderer.enabled = on;

			if (Furniture.AllFurnInfo[myFurniture.furnType].alt_sprite != null)
				myFurniture.ChangeSprite(on);
		}
	}
}