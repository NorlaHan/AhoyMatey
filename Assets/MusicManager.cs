using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public AudioClip[] audioClips;
	public int index = 0;

	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		ChangeAndPlay (index);
	}

	public void StartSceneBGM (){
		if (index == 0) {
			index = 1;
		}else if (index == 1) {
			index = 2;
		}else if (index == 2) {
			index = 1;
		}	
		ChangeAndPlay (index);
		Invoke ("StartSceneBGM", audioClips [index].length);
	}

	public void OnGameSettle () {
		CancelInvoke ();
		index = 3;
		audioSource.clip = audioClips [index];
		audioSource.Play ();

	}

	void ChangeAndPlay (int input)
	{
		audioSource.clip = audioClips [input];
		audioSource.Play ();
	}

}
