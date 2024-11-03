using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplaySpace.FurnitureSpace
{
	public class FurnPowerGenerator : FurnitureGeneral
	{
		public delegate void GeneratorEvents();
		public static event GeneratorEvents
			OnGeneratorPlaced,
			OnGeneratorRemoved;

		private static List<FurnPowerGenerator> _activeGenerators;
		private static List<FurnPowerGenerator> ActiveGenerators
		{
			get
			{
				if (_activeGenerators == null)
					_activeGenerators = new List<FurnPowerGenerator>();
				return _activeGenerators;
			}
			set => _activeGenerators = value;
		}
		public static bool ActiveGeneratorExists => ActiveGenerators.Count > 0;
		
		
		public override void Place()
		{
			base.Place();
			ActiveGenerators.Add(this);
			OnGeneratorPlaced?.Invoke();
		}

		public override void PickUp()
		{
			base.PickUp();
			ActiveGenerators.Remove(this);
			OnGeneratorRemoved?.Invoke();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if(ActiveGenerators.Contains(this))
				ActiveGenerators.Remove(this);
			OnGeneratorRemoved?.Invoke();
		}
	}
}