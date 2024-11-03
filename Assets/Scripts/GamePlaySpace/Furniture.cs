using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;
using GameplaySpace.FurnitureSpace;

namespace GameplaySpace
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(SortingGroup))]
	public abstract class Furniture : MonoBehaviour
	{
		public delegate void FurnitureEvents(Furniture _furn);
		public static event FurnitureEvents
			OnCreatedFurniture,
			OnFurnitureDestroyed,
			OnFurniturePlacement,
			OnFurniturePickup;


		private static List<Furniture> _activeFurniture;
		public static List<Furniture> ActiveFurniture
		{
			get
			{
				if (_activeFurniture == null)
					_activeFurniture = new List<Furniture>();
				return _activeFurniture;
			}
			private set => _activeFurniture = value;
		}


		private static GameObject _furnPrefab;
		public static GameObject FurnPrefab
		{
			get
			{
				if (_furnPrefab == null)
					_furnPrefab = Resources.Load<GameObject>("FurniturePrefabs/Obj_Furniture_General");
				return _furnPrefab;
			}
		}


		#region PUBLIC CALLS:
		public static void CreateFurniture(string _furnType)
		{
			var prefab = FurnPrefab;
			if (AllFurnInfo[_furnType].prefab)
				prefab = Resources.Load<GameObject>("FurniturePrefabs/" + _furnType);

			// spawn:
			Vector2 spawnLocation = PlayerMovement.instance.transform.position;
			GameObject inst = Instantiate(prefab, spawnLocation, Quaternion.identity);
			Furniture instFurnScript = inst.GetComponent<Furniture>();
			instFurnScript.furnType = _furnType;

			//after spawn:
			ActiveFurniture.Add(instFurnScript);
			UpdateAllSortingOrder(instFurnScript);
			OnCreatedFurniture?.Invoke(instFurnScript);
		}

		public static void CreateFurniture(string _furnType, int _depth, Vector2 _position)
		{
			var prefab = FurnPrefab;
			if (AllFurnInfo[_furnType].prefab)
				prefab = Resources.Load<GameObject>("FurniturePrefabs/" + _furnType);

			// spawn:
			GameObject inst = Instantiate(prefab, _position, Quaternion.identity);
			Furniture instFurnScript = inst.GetComponent<Furniture>();
			instFurnScript.furnType = _furnType;
			instFurnScript.mySortGroup.sortingOrder = _depth;
			instFurnScript.state = States.placed;

			//after spawn:
			ActiveFurniture.Add(instFurnScript);
		}


		#endregion


		private static void UpdateAllSortingOrder(Furniture _topFurn)
		{
			foreach (Furniture furn in ActiveFurniture)
				furn.mySortGroup.sortingOrder--;

			_topFurn.mySortGroup.sortingOrder = 0;
		}

		public static void ResetActiveFurn()
		{
			ActiveFurniture.Clear();
		}


		#region	JSON FURN INFO:


		[System.Serializable]
		public class FurnData
		{
			public string title;
			public string description;
			public string sprite;
			public string alt_sprite;
			public int menuIndex;
			public string available;
			public string animation;
			public bool prefab;
		}

		private static Dictionary<string, FurnData> _allFurnInfo;
		public static Dictionary<string, FurnData> AllFurnInfo
		{
			get
			{
				if (_allFurnInfo == null)
				{
					TextAsset jsonFile = Resources.Load<TextAsset>("FurnitureData");
					_allFurnInfo = JsonConvert.DeserializeObject<Dictionary<string, FurnData>>(jsonFile.text);
				}
				return _allFurnInfo;
			}
		}


		#endregion



		#region INSTANCE:


		public enum States { held, placed }
		public States state;


		public int SortDepth => mySortGroup.sortingOrder;
		public bool CanBePlaced => transform.position.y - spriteBounds.size.y / 2 > playArea.position.y;
		private bool _canBePickedUp;
		public bool CanBePickedUp
		{
			get
			{
				if (GameManager.Instance.currentMode == GameManager.GameModes.gallery)
					_canBePickedUp = false;
				else if (TextManager.Instance.TextIsActive)
					_canBePickedUp = false;
				else if (SceneLoader.Instance.IsFading)
					_canBePickedUp = false;
				else
					_canBePickedUp = true;

				return _canBePickedUp;
			}
		}


		Sprite[] spriteArray;
		int rotationIndex = 0;


		protected SortingGroup mySortGroup;

		protected Animator myAnimator;

		public abstract SpriteRenderer MyMainSpriteRenderer { get; }
		public abstract BoxCollider2D MyCollider { get; }

		public abstract LightObject Light { get; }

		private SpriteRenderer[] allRenderers;

		public string furnType;
		protected Bounds spriteBounds;

		protected Transform playArea;

		Color col = Color.white;

		protected virtual void Awake()
		{
			mySortGroup = GetComponent<SortingGroup>();
		}

		protected virtual void Start()
		{
			allRenderers = GetComponentsInChildren<SpriteRenderer>();
			playArea = GameObject.FindGameObjectWithTag("PlayArea").transform;
		}

		protected virtual void Update()
		{
			if (state == States.held)
			{
				if (MenuUtilitySpace.PauseMenu.ActivePause)
					return;

				Vector3 position = PlayerGameplay.instance.transform.position;

				// Clamp:
				var bottomCorner = Camera.main.ViewportToWorldPoint(Vector3.zero);
				var topCorner = Camera.main.ViewportToWorldPoint(Vector3.one);

				position.x = Mathf.Clamp(position.x,
					bottomCorner.x + spriteBounds.size.x / 2,
					topCorner.x - spriteBounds.size.x / 2);
				position.y = Mathf.Clamp(position.y,
					bottomCorner.y + spriteBounds.size.y / 2,
					topCorner.y - spriteBounds.size.y / 2);

				// apply:
				transform.position = position;
			}

			#region VOID FADE:
			if (Level.instance.CurrentLevelIndex == 5 && GameManager.Instance.currentMode == GameManager.GameModes.normal)
			{
				if (state != States.held)
				{
					col.a -= 0.05f * Time.deltaTime;
					if (col.a <= 0)
					{
						DestroyFurniture();
					}
					// MyMainSpriteRenderer.color = col;
					foreach (SpriteRenderer renderer in allRenderers)
					{
						renderer.color = col;
					}
				}
			}
			#endregion
		}

		protected virtual void OnDestroy()
		{
			ActiveFurniture.Remove(this);
		}

		public virtual void PickUp()
		{
			state = States.held;
			UpdateAllSortingOrder(this);

			OnFurniturePickup?.Invoke(this);
		}

		public void DestroyFurniture()
		{
			OnFurnitureDestroyed?.Invoke(this);
			Destroy(gameObject);
		}

		public virtual void Place()
		{
			state = States.placed;

			OnFurniturePlacement?.Invoke(this);
		}

		public void ChangeSprite(bool alt = false)
		{
			string stringAdress = alt == true ? AllFurnInfo[furnType].alt_sprite : AllFurnInfo[furnType].sprite;

			// MyMainSpriteRenderer.sprite = Resources.Load<Sprite>("FurnitureSprites/" + stringAdress);

			spriteArray = Resources.LoadAll<Sprite>("FurnitureSprites/" + stringAdress);
			rotationIndex = 0;
			MyMainSpriteRenderer.sprite = spriteArray[0];

		}

		public void TryDoRotate()
		{
			rotationIndex++;
			if (rotationIndex >= spriteArray.Length)
				rotationIndex = 0;

			MyMainSpriteRenderer.sprite = spriteArray[rotationIndex];
		}

		#endregion
	}
}