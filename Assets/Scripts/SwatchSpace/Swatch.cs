using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwatchSpace
{
    public class Swatch : MonoBehaviour
    {
        [SerializeField] Sprite[] allSwatchSprites;
		[SerializeField] GameObject spriteObject;
		[SerializeField] GameObject shadowObject;
		SpriteRenderer SwatchRenderer;

		private float maxSqrMagnitude = 350;
		float speed = 0.85f;

		private void Start()
		{
			SwatchRenderer = spriteObject.GetComponent<SpriteRenderer>();
			SwatchRenderer.sprite = allSwatchSprites[Random.Range(0, allSwatchSprites.Length)];

			shadowObject.transform.Translate(Vector3.down * 0.1f, Space.World);
		}

		private void Update()
		{
			transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);

			if (transform.position.sqrMagnitude > maxSqrMagnitude)
				Destroy(gameObject);
		}

	}
}