using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplaySpace.FurnitureSpace
{
	[RequireComponent(typeof(Animator))]
	public class FurnAnimated : Furniture
	{
		protected float baseSpeed = 1f;

		[SerializeField] private BoxCollider2D _myCollider;
		public override BoxCollider2D MyCollider => _myCollider;


		[SerializeField] private SpriteRenderer _myMainSpriteRenderer;
		public override SpriteRenderer MyMainSpriteRenderer => _myMainSpriteRenderer;

		[SerializeField] private LightObject _light;
		public override LightObject Light => _light;

		protected override void Start()
		{
			base.Start();
			myAnimator = GetComponent<Animator>();
			myAnimator.Play(AllFurnInfo[furnType].animation);
			myAnimator.speed = 0;
		}

		public override void PickUp()
		{
			base.PickUp();
			myAnimator.speed = 0;
		}

		public override void Place()
		{
			base.Place();
			myAnimator.speed = baseSpeed;
		}
	}
}