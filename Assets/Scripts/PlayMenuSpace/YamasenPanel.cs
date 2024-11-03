using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayMenuSpace
{
	public class YamasenPanel : MonoBehaviour
	{
		public static YamasenPanel instance;
		private void Awake() => instance = this;

		[SerializeField] Image backgroundRenderer;
		[SerializeField] Animator PortAnimator;

		private void Start()
		{
			backgroundRenderer.sprite = Level.instance.PanelSprite;
		}

		public void StartTalking()
		{
			PortAnimator.SetBool("isTalking", true);
		}

		public void EndTalking()
		{
			PortAnimator.SetBool("isTalking", false);
		}
	}
}