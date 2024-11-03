using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwatchSpace
{
	public class SwatchSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject swatchPrefab;

		private float spawnWaitTime = 6.2f;

		private void Start()
		{
			StartCoroutine(ContiuousSpawn());
		}

		IEnumerator ContiuousSpawn()
		{
			while (true)
			{
				Instantiate(swatchPrefab, transform, false);
				yield return new WaitForSeconds(spawnWaitTime);
			}
		}
	}
}