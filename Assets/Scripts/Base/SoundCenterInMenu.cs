using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCenterInMenu : MonoBehaviour {

    public AudioClip Music; // this

    void Start()
    {
        if (Music != null)
        {
            GetComponent<AudioSource>().clip = Music;
            GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
