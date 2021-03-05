using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrutchForWinMenu : MonoBehaviour {
    public GameObject btnNext;
	// Use this for initialization
	void Start () {
        int p = PlayerPrefs.GetInt(ControlOfPl.GLOBAL_MODE, 1);
        if (p == ControlOfPl.TRAINING_MODE) {
            btnNext.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
