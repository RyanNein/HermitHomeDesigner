using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplaySpace.FurnitureSpace
{
	public class FurnitureGeneral : Furniture
	{

		[SerializeField] private BoxCollider2D _myCollider;
		public override BoxCollider2D MyCollider => _myCollider;


		[SerializeField] private SpriteRenderer _myMainSpriteRenderer;
		public override SpriteRenderer MyMainSpriteRenderer => _myMainSpriteRenderer;


		[SerializeField] private LightObject _light;
		public override LightObject Light => _light;

		protected override void Start()
		{
			base.Start();
			
			ChangeSprite();
			
			// Animation:
			if (!string.IsNullOrEmpty(AllFurnInfo[furnType].animation))
			{
				TryGetComponent(out myAnimator);
				if (myAnimator == null)
				{
					myAnimator = gameObject.AddComponent<Animator>();
					myAnimator.runtimeAnimatorController = Resources.Load("Animation/FurnitureAnimator") as RuntimeAnimatorController;
					myAnimator.applyRootMotion = true;
				}
				myAnimator.Play(AllFurnInfo[furnType].animation);
				myAnimator.speed = 0;
			}

			// Change Collider Size:
			spriteBounds = MyMainSpriteRenderer.sprite.bounds;
			MyCollider.size = (Vector2)spriteBounds.size - (Vector2.one * 0.1f);

		}

		public override void PickUp()
		{
			base.PickUp();
			if (Light != null)
				Light.OnPickup();

			if (myAnimator != null)
				myAnimator.speed = 0;
		}

		public override void Place()
		{
			base.Place();
			
			if (Light != null)
				Light.OnPlacement();

			if (myAnimator != null)
				myAnimator.speed = 1;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}