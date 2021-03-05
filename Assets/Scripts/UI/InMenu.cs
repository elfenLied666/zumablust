using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InMenu : MonoBehaviour {

	public bool ThisGlobal = false;
	public bool CleanPlayerPrefs = false;
	public Text ScoreText;
	public Text SoundText;
	public string[] ListOfLevels;
	public Image[] SetOfPlates;
	public Text[] SetOfLevelText;
	public Image ButtonNext;
	public Image ButtonBack;
	public Sprite[] Stars; // 0 , 1 , 2 , 3 , (4) block

	// calculate
	private int CountOfPages;
	private int CurrentPage = 0;
	private int threeActive = 3;

	//[HideInInspector]
	public int Clicked;

	[Header("Audios")]
	public AudioClip Tap_to_play;
	public AudioClip Tap_to_level;
	public AudioClip Tap_to_next;
	public AudioClip Tap_to_back;

	void Start(){
		if (CleanPlayerPrefs) {
			PlayerPrefs.DeleteAll ();
		}
		if (ThisGlobal) {
			// About score
			float GlobalScore = 0;
			int CountLvl = 0;
			int Point = 0;
            /*while (Point == 0) {
				if (CountLvl < ListOfLevels.Length) {
					if (PlayerPrefs.HasKey ("0win" + ListOfLevels [CountLvl])) {
						GlobalScore += PlayerPrefs.GetInt ("0score" + ListOfLevels [CountLvl]);
					}
					CountLvl++;
				} else {
					Point = 1;
				}
			}*/
            GlobalScore = PlayerPrefs.GetFloat(ControlOfPl.GLOBAL_SCORE_KEY, 0);
			/*if (GlobalScore > 1000) {
				GlobalScore = GlobalScore / 1000;
				ScoreText.text = "SCORE: " + GlobalScore.ToString () + "k";
			} else {
				ScoreText.text = "SCORE: " + GlobalScore.ToString ();
			}*/
            ScoreText.text = "SCORE: " + GlobalScore.ToString();
            //About sound
            if (PlayerPrefs.HasKey ("Sound")) {
				if (PlayerPrefs.GetInt ("Sound") == 1) {
					AudioListener.pause = false;
					SoundText.text = "SOUND: ON";
				} else {
					AudioListener.pause = true;
					SoundText.text = "SOUND: OFF";
				}
			} else {
				PlayerPrefs.SetInt ("Sound", 1);
				AudioListener.pause = false;
				SoundText.text = "SOUND: ON";
			}
			CalculateMain ();
		}
        //SimpleAds.GetInstance().ShowAd(SimpleAds.START_MENU);
	}

	public void Exit(){
        //GameObject.Find("Main Camera").GetComponent<GoogleMobileAdsDemoScript>().ShowGoogleAd(GoogleMobileAdsDemoScript.EXIT_GAME);
        Application.Quit ();
	}

	public void Sound(){
		if (PlayerPrefs.GetInt ("Sound") == 0) {
			AudioListener.pause = false;
			SoundText.text = "SOUND: ON";
			PlayerPrefs.SetInt ("Sound",1);
		} else {
			AudioListener.pause = true;
			SoundText.text = "SOUND: OFF";
			PlayerPrefs.SetInt ("Sound",0);
		}
	}


	void CalculateMain (){
		CountOfPages = ListOfLevels.Length / 10;
	}

	public void TapToPlay(){
		GameObject.Find ("Canvas").GetComponent<Animation> ().Play ("TapToPlay");
		GetComponent<AudioSource> ().pitch = 0.3f;
		if (Tap_to_play != null) {
			GetComponent<AudioSource> ().PlayOneShot (Tap_to_play);
		}
        PlayerPrefs.SetInt(ControlOfPl.GLOBAL_MODE, 1);
        ReSet ();
	}

    public void TapToPlay2()
    {
        GameObject.Find("Canvas").GetComponent<Animation>().Play("TapToPlay");
        GetComponent<AudioSource>().pitch = 0.3f;
        if (Tap_to_play != null)
        {
            GetComponent<AudioSource>().PlayOneShot(Tap_to_play);
        }
        PlayerPrefs.SetInt(ControlOfPl.GLOBAL_MODE, 2);
        ReSet();
    }

    void ReSet(){
		if (CurrentPage < CountOfPages) {
			ButtonNext.gameObject.SetActive (true);
		} else {
			ButtonNext.gameObject.SetActive (false);
		}
		if (CurrentPage > 0) {
			ButtonBack.gameObject.SetActive (true);
		} else {
			ButtonBack.gameObject.SetActive (false);
		}
		int Point = 0;
		int CountActivate = 0;
		while (Point == 0){
			if (CountActivate < 10 && CountActivate + CurrentPage * 10 < ListOfLevels.Length) {
				SetOfPlates [CountActivate].sprite = Stars [4];
				SetOfLevelText [CountActivate].enabled = false;
				SetOfPlates [CountActivate].enabled = false;
				if (ListOfLevels [CountActivate + CurrentPage * 10] != null) {
					SetOfPlates [CountActivate].enabled = true;
					if (PlayerPrefs.HasKey ("0win" + ListOfLevels [CountActivate + CurrentPage * 10])) {
						SetOfLevelText [CountActivate].enabled = true;
						SetOfLevelText [CountActivate].text = (CountActivate + 1 + CurrentPage * 10).ToString ();
						int st = PlayerPrefs.GetInt ("0stars" + ListOfLevels [CountActivate + CurrentPage * 10]);
						SetOfPlates [CountActivate].sprite = Stars [st];
					} else {
						if (threeActive > 0) {
							threeActive--;
							SetOfLevelText [CountActivate].enabled = true;
							SetOfLevelText [CountActivate].text = (CountActivate + 1 + CurrentPage * 10).ToString ();
							SetOfPlates [CountActivate].sprite = Stars [0];
						}
					}
				}
				CountActivate++;
			} else {
				if (CountActivate < 10) {
					SetOfLevelText [CountActivate].enabled = false;
					SetOfPlates [CountActivate].enabled = false;
					CountActivate++;
				} else {
					Point = 1;
				}
			}

		}
	}


	public void LocalClickToLevel(){
		GameObject.Find ("Main Camera").GetComponent<InMenu> ().Clicked = Clicked;
	}

	public void ClickToLevel(){
		GameObject.Find ("Canvas").GetComponent<Animation> ().Play ("GoInLevel");
		GetComponent<AudioSource> ().pitch = 1f;
		if (Tap_to_level != null) {
			GetComponent<AudioSource> ().PlayOneShot (Tap_to_level);
		}
		StartCoroutine (Load());
	}
	IEnumerator Load(){
		yield return new WaitForSeconds (0.4f);
		string GetIntOfLevel = ListOfLevels [Clicked + CurrentPage * 10];
        Debug.Log(GetIntOfLevel);
		SceneManager.LoadScene (GetIntOfLevel);
	}

	public void ClickToNextPage(){
		GameObject.Find ("Canvas").GetComponent<Animation> ().Play ("GoNextPage");
		GetComponent<AudioSource> ().pitch = 0.5f;
		if (Tap_to_next != null) {
			GetComponent<AudioSource> ().PlayOneShot (Tap_to_next);
		}
		StartCoroutine (waitNextPage());
	}

	IEnumerator waitNextPage(){
		yield return new WaitForSeconds (0.7f);
		CurrentPage++;
		ReSet ();
	}

	public void ClickToBackPage(){
		GameObject.Find ("Canvas").GetComponent<Animation> ().Play ("GoBackPage");
		GetComponent<AudioSource> ().pitch = 0.5f;
		if (Tap_to_back != null) {
			GetComponent<AudioSource> ().PlayOneShot (Tap_to_back);
		}
		StartCoroutine (waitBackPage());
	}

	IEnumerator waitBackPage(){
		yield return new WaitForSeconds (0.7f);
		CurrentPage--;
		threeActive = 3;
		int startPage = 0;
		int Point = 0;
		int ten = 0;
		while(Point == 0){
			if (threeActive > 0) {
				if (startPage < CurrentPage) {
					if (ten <= 10) {
						if (!PlayerPrefs.HasKey ("0win" + ListOfLevels [ten + startPage * 10])) {
							threeActive--;
						} else {
							ten++;
						}
					} else {
						ten = 0;
						startPage++;
					}
				} else {
					Point = 1;
				}
			} else {
				Point = 1;
			}
		}



		ReSet ();
	}


}
