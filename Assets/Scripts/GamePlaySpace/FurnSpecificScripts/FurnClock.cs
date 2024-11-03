using UnityEngine;

namespace GameplaySpace.FurnitureSpace
{
    public class FurnClock : FurnAnimated
    {
		protected override void Start()
		{
			base.Start();
			if (Level.instance.CurrentLevelIndex == 5)
				baseSpeed = 70f;
		}
	}
}
