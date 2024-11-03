using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelSO : ScriptableObject
{
	public AudioClip levelMusic;
	public float musicVolume;
	public int nextScene;
	public TextAsset levelStory;
	public Sprite panelBackground;
	public int levelIndex;
}