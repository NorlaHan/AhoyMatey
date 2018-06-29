using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip startMenuClip, endingClip;
	public AudioClip[] sceneClips;
	public int index = 0;

	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		ChangeAndPlay (startMenuClip);
	}

	public void OnStartSceneBGM (){
        Debug.Log("Starting sceneClips["+ index + "].");
		ChangeAndPlay (sceneClips[index]);
        Invoke("OnStartSceneBGM", sceneClips[index].length);
        if (index < sceneClips.Length - 1){
            index++;
        }
        else { index = 0; }
        Debug.Log("Next Clips is sceneClips[" + index + "].");
    }

	public void OnGameSettle () {
		CancelInvoke ();
        ChangeAndPlay(endingClip);
	}

	void ChangeAndPlay (AudioClip clip)
	{
		audioSource.clip = clip;
		audioSource.Play ();
	}

}
