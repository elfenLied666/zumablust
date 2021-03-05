using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour {

    public Text infoLvl;
    public int globalMode;
    public GameObject main0;
	public string NameOfScene;
	public string NameOfNext;
	public string NameOfMenu;
	public int ScoreFor3Stars;
	public int ScoreFor2Stars;
	[HideInInspector]
	public int onceClick = 0;
	[Header("Other Objects")]
	public Text TextOfProgress;
	public Text TextOfScore;
	public Image Stars;
	public Sprite _3Stars;
	public Sprite _2Stars;

    //Panel
    public GameObject BtnPizdaa;
   

    void Start()
    {
       // infoLvl.text = NameOfScene;
       globalMode = PlayerPrefs.GetInt(ControlOfPl.GLOBAL_MODE, 1);
    }

        public void ClickToPause(){

		if (onceClick == 0) {
			StartCoroutine (iClickToPause ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToPause(){
      /*  GoogleMobileAdsDemoScript gms = GameObject.Find("Main Camera").GetComponent<GoogleMobileAdsDemoScript>();
        if (gms != null)
            gms.ShowGoogleAd(GoogleMobileAdsDemoScript.MENU);*/
        if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toPause");
		yield return new WaitForSeconds (0.4f);
		Time.timeScale = 0;
		onceClick = 0;
	}

	public void ClickToContinue(){
		if (onceClick == 0) {
			StartCoroutine (iClickToContinue ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToContinue(){
		Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toContinue");
		yield return new WaitForSeconds (0.4f);
		onceClick = 0;
	}

	public void ClickToReset(){
		if (onceClick == 0) {
			StartCoroutine (iClickToReset ());
            //GoogleMobileAdsDemoScript.GetInstance().ShowGoogleAd(GoogleMobileAdsDemoScript.RESET);
            onceClick = 1;
		}
	}
	IEnumerator iClickToReset(){
		Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toReset");
		yield return new WaitForSeconds (0.6f);
		SceneManager.LoadScene (NameOfScene);
	}

	public void ClickToMenu(){
		if (onceClick == 0) {
			StartCoroutine (iClickToMenu ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToMenu(){
        Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toReset");
		yield return new WaitForSeconds (0.6f);
		SceneManager.LoadScene (NameOfMenu);
	}


	public void ToLose(int Progress){
       /* GoogleMobileAdsDemoScript gms = GameObject.Find("Main Camera").GetComponent<GoogleMobileAdsDemoScript>();
        if(gms != null)
            gms.ShowGoogleAd(GoogleMobileAdsDemoScript.LEVEL_LOSE);*/
       // GoogleMobileAdsDemoScript.GetInstance().ShowGoogleAd(GoogleMobileAdsDemoScript.LEVEL_LOSE);
        StartCoroutine (iToLose (Progress));
    }
	IEnumerator iToLose(int Progress){
		onceClick = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toLose");
		TextOfProgress.text = "Progress: " + Progress.ToString () + "%";
		yield return new WaitForSeconds (0.6f);
		onceClick = 0;
        
    }

	public void ClickToLoseReset(){
		if (onceClick == 0) {
			StartCoroutine (iClickToLoseReset ());

			onceClick = 1;
		}
	}
	IEnumerator iClickToLoseReset(){
		Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toResetLose");
		yield return new WaitForSeconds (0.6f);
        Debug.Log("NameOfScene="+NameOfScene);
		SceneManager.LoadScene (NameOfScene);
	}

	public void ClickToLoseMenu(){
		if (onceClick == 0) {
			StartCoroutine (iClickToLoseMenu ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToLoseMenu(){
		Time.timeScale = 1;
        //Debug.Log("iClickToLoseMenu()"); 
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toResetLose");
		yield return new WaitForSeconds (0.6f);
		SceneManager.LoadScene (NameOfMenu);
	}

    public void ToWin(int Score){
        //GameObject.Find("Main Camera").GetComponent<GoogleMobileAdsDemoScript>().ShowGoogleAd(GoogleMobileAdsDemoScript.LEVEL_WIN);
        /*GoogleMobileAdsDemoScript gms = GameObject.Find("Main Camera").GetComponent<GoogleMobileAdsDemoScript>();
        if (gms != null)
            gms.ShowGoogleAd(GoogleMobileAdsDemoScript.LEVEL_WIN);*/
        //GoogleMobileAdsDemoScript.GetInstance().ShowGoogleAd(GoogleMobileAdsDemoScript.LEVEL_WIN);
        float globalScore = PlayerPrefs.GetFloat(ControlOfPl.GLOBAL_SCORE_KEY, 0);
        if (globalMode == 2)
        {
            globalScore = globalScore + Score;
            PlayerPrefs.SetFloat(ControlOfPl.GLOBAL_SCORE_KEY, globalScore);
        }
        StartCoroutine (iToWin (Score));
	}
	IEnumerator iToWin(int Score)
    {
		onceClick = 1;
		int stars_num = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
 
		    GetComponent<Animation> ().Play ("UI_toWin");
		TextOfScore.text = Score.ToString ();
		if (Score >= ScoreFor2Stars) {
			Stars.sprite = _2Stars;
			stars_num = 2;
		}
		if (Score >= ScoreFor3Stars) {
			Stars.sprite = _3Stars;
			stars_num = 3;
		}
		//Save score
		if (PlayerPrefs.HasKey ("0score" + NameOfScene.ToString ())) {
			if (PlayerPrefs.GetInt ("0score" + NameOfScene.ToString ()) < Score) {
				PlayerPrefs.SetInt ("0score" + NameOfScene.ToString (), Score);
			}
		} else {
			PlayerPrefs.SetInt ("0score" + NameOfScene.ToString (), Score);
		}
		//Save stars
		if (PlayerPrefs.HasKey ("0stars" + NameOfScene.ToString ())) {
			if (PlayerPrefs.GetInt ("0stars" + NameOfScene.ToString ()) < stars_num) {
				PlayerPrefs.SetInt ("0stars" + NameOfScene.ToString (), stars_num);
			}
		} else {
			PlayerPrefs.SetInt ("0stars" + NameOfScene.ToString (), stars_num);
		}
		//Save win
		PlayerPrefs.SetInt ("0win" + NameOfScene.ToString (), 1);

		yield return new WaitForSeconds (0.6f);
		onceClick = 0;
	}

    //То что надо
	public void ClickToWinNext(){
		if (onceClick == 0) {
			StartCoroutine (iClickToWinNext ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToWinNext(){
		Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toNextWin");
		yield return new WaitForSeconds (0.6f);
		SceneManager.LoadScene (NameOfNext);
	}

	public void ClickToWinReset(){
		if (onceClick == 0) {
			StartCoroutine (iClickToWinReset ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToWinReset(){
		Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toNextWin");
		yield return new WaitForSeconds (0.6f);
		SceneManager.LoadScene (NameOfScene);
	}

	public void ClickToWinMenu(){
		if (onceClick == 0) {
			StartCoroutine (iClickToWinMenu ());
			onceClick = 1;
		}
	}
	IEnumerator iClickToWinMenu(){
		Time.timeScale = 1;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons != null){
			GameObject.Find ("SC 5").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Tap_to_buttons);
		}
		GetComponent<Animation> ().Play ("UI_toNextWin");
		yield return new WaitForSeconds (0.6f);
		SceneManager.LoadScene (NameOfMenu);
	}

    public void calculate(int x) {
        Debug.Log("PIZDA");
        MainStart main = main0.GetComponent<MainStart>();

        float minW = (30 * main.NumberOfBalls) + (40 * main.NumberOfBalls) + (12 * main.NumberOfBalls);
        float k2 = main.BaseSpeed + 0.72f;
        float k3 = main.BaseSpeed + 0.556f;
        float mf2 = (minW / k2) ;
        float mf3 = minW / k3;
        int stars2 = Mathf.CeilToInt(mf2);
        int stars3 = Mathf.FloorToInt(mf3);
        
        ScoreFor2Stars = x*stars2;
        ScoreFor3Stars = x*stars3;

    }

 
    

}
