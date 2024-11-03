using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameplaySpace
{
	public class PlayerGameplay : MonoBehaviour
	{
		public static PlayerGameplay instance;
		private void Awake() => instance = this;

		public enum states { holding, resting };
		public states state { get; private set; }
		
		bool RightClick, LeftClick;

		[HideInInspector] public Furniture heldFurn = null;

		public delegate void PlayerStateChange(states state);
		public static event PlayerStateChange OnStateChange;

		void Start()
		{
			ChangeState(states.resting);

			TextManager.OnTextStart += () => ChangeState(states.resting);
			MenuUtilitySpace.PauseMenu.OnPauseChange += ToggleEnable;
			SceneManager.sceneLoaded += ResetOnScene;

			Furniture.OnCreatedFurniture += HoldFurn;
		}

		void Update()
		{
			LeftClick = Input.GetMouseButtonDown(0);
			RightClick = Input.GetMouseButtonDown(1);

			updateFunction();
		}


		private delegate void StateDel();
		private StateDel updateFunction;

		public void ChangeState(states _state, Furniture furnToHold = null)
		{
			if (_state == state)
				return;

			state = _state;

			switch (_state)
			{
				case states.resting:
					updateFunction = RestingUpdate;
					heldFurn = null;
					break;

				case states.holding:
					updateFunction = HoldingUpdate;
					break;
			}

			OnStateChange?.Invoke(_state);
		}

		private void HoldFurn(Furniture furn)
		{
			heldFurn = furn;
			ChangeState(states.holding);
		}

		#region STATE:
		private void RestingUpdate()
		{
			if (LeftClick)
			{
				Collider2D[] col = Physics2D.OverlapPointAll(PlayerMovement.instance.MouseWorldPosition, LayerMask.GetMask("Furniture"));
				if (col.Length > 0)
				{
					Furniture clickedFurn = null;
					var topValue = -Mathf.Infinity;
					for (int i = 0; i < col.Length; i++)
					{
						var FurnInt = col[i].GetComponent<Furniture>();
						var sortValue = FurnInt.SortDepth;
						if (sortValue > topValue)
						{
							topValue = sortValue;
							clickedFurn = FurnInt;
						}
					}

					if (clickedFurn != null)
					{
						if (clickedFurn.CanBePickedUp)
						{
							clickedFurn.PickUp();
							heldFurn = clickedFurn;
							ChangeState(states.holding);
						}
					}
				}
			}
		}

		private void HoldingUpdate()
		{
			if (LeftClick && heldFurn != null)
			{
				if (heldFurn.CanBePlaced)
				{
					heldFurn.Place();
					ChangeState(states.resting);
				}
			}
			else if (RightClick && heldFurn != null)
			{
				// heldFurn.TryDoRotate();
			}
		}

		#endregion


		public void DestroyHeldFurn()
		{
			heldFurn.DestroyFurniture();
			ChangeState(states.resting);
		}

		private void ResetOnScene(Scene scene, LoadSceneMode mode)
		{
			ChangeState(states.resting);
		}

		private void ToggleEnable()
		{
			enabled = !enabled;
		}

		private void OnDestroy()
		{
			TextManager.OnTextStart -= () => ChangeState(states.resting);
			MenuUtilitySpace.PauseMenu.OnPauseChange -= ToggleEnable;
			SceneManager.sceneLoaded -= ResetOnScene;
			Furniture.OnCreatedFurniture -= HoldFurn;
		}
	}
}