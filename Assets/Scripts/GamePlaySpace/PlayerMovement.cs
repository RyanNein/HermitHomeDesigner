using UnityEngine;

namespace GameplaySpace
{
	public class PlayerMovement : MonoBehaviour
	{
		public static PlayerMovement instance;
		private void Awake() => instance = this;

		public Vector3 MousePosition, MouseWorldPosition;

		private void Start()
		{
			// Cursor.visible = false;
		}

		private void Update()
		{
			MousePosition = Input.mousePosition;
			MouseWorldPosition = Camera.main.ScreenToWorldPoint(MousePosition);
			MouseWorldPosition.z = Camera.main.nearClipPlane;

			// Clamp:
			var bottomCorner = Camera.main.ViewportToWorldPoint(Vector3.zero);
			var topCorner = Camera.main.ViewportToWorldPoint(Vector3.one);

			if(MouseWorldPosition.x < bottomCorner.x || MouseWorldPosition.x > topCorner.x || MouseWorldPosition.y > topCorner.y || MouseWorldPosition.y < bottomCorner.y)
			{
				MouseWorldPosition.x = Mathf.Clamp(MouseWorldPosition.x, bottomCorner.x, topCorner.x);
				MouseWorldPosition.y = Mathf.Clamp(MouseWorldPosition.y, bottomCorner.y, topCorner.y);
				Cursor.visible = true;
			}
			else
			{
				// Cursor.visible = false;
			}

			// Apply:
			transform.position = MouseWorldPosition;
		}
	}
}