using UnityEngine;

namespace GameplaySpace.FurnitureSpace
{
	public class FurnDaruma : FurnitureGeneral
	{
		protected override void Start()
		{
			base.Start();
			if (Level.instance.CurrentLevelIndex == 5)
				ChangeSprite(true);
		}
	}
}