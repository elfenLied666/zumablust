using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControlOfPl : MonoBehaviour {

    public const string GLOBAL_SCORE_KEY = "globalScore";
    public const string GLOBAL_MODE = "globalMode";
    public const int CAMPANION_MODE = 1;
    public const int TRAINING_MODE = 2;
    public int globalMode;

    private float prevTime;
    private float prevTime2;
    private float minTime;
    private bool isChangePos = false;

    public static int o = 1;
    public GameObject colorForNextBall;
    public GameObject colorForNextBall2;
    private Color[] arrColorsForNextBall;

    public bool idDebug;

	public bool EditCameraSize = false; // dependence (width && height) for screen
	public MainStart[] SetOfLines; // set of lines
	public LayerMask ShotInputLayer; // shot input layer
	public GameObject Player; // object of player
	public GameObject CurrentBall; // example of  base ball
	public GameObject MultiBall; // example of multi-ball
	public GameObject DestroyBall; // example of object that can delete a ball
	public GameObject FireBall; // example of fire
	public GameObject DestroyColor; // example of arrow that can delete all balls with one color

	public GameObject OfPlayer; // object with sprites of player
	public Transform PivotForNewBalls; // point where generate new balls
	public AnimationClip ThrowOfPlayer; // animation of player
	private GameObject MoveBall; //generated ball
	//[HideInInspector]
	public Vector3 CanThrow; // access to throw
	private List<Color> CollorOfBalls; // colors of balls from all lines
	[HideInInspector]
	public List<Color> FullSetOfCollors; // colors of balls from all lines
	[HideInInspector]
	public int EndOfStartSpeed = 0; // for access to throw (after start)
	private Vector2 ToTouch; // position of touch
	private RaycastHit hit; // simple raycast
	private Vector3 WorldTouch; // world position of touch
	[HideInInspector]
	public BallControl SaveComponent; // component of ball that fly

	[Header("For Score")]
	public int Score_OneBall = 50;
	public int Score_MoreThree = 60;
	public int Score_Bonuse = 30;
	public int Score_Combo = 100;
	public Sprite Sprite_OneBall;
	public Sprite Sprite_MoreThree;
	public Sprite Sprite_Bonuse;
	public Sprite Sprite_Combo;
	public GameObject ExampleScore;
	public Text UIViewScore;
	[HideInInspector]
	public int Score = 0;

	[HideInInspector]
	public int Player_off = 0; // if != 0, than player can`t throw balls
	[HideInInspector]
	public ThrowBall LastBall; // component of ball that fly

    public GameObject ClickPlate;

	void Start () {
        //colorForNextBall.GetComponent<SpriteRenderer>().color = Color.red;
        //colorForNextBall.SetActive(true);
        

        minTime = 0.2f;
        prevTime = 0;
        prevTime2 = 0;
        //isInArea(3f, 1f);
		int CountMS = 0;
		int NumOfCollor = 0;
        arrColorsForNextBall = new Color[2];
        arrColorsForNextBall[0] = Color.clear;
		// get colors from all lines тут заполняется массив 
		while(CountMS < SetOfLines.Length){
			if (NumOfCollor < SetOfLines [CountMS].SetOfColors.Length) {
				FullSetOfCollors.Add (SetOfLines [CountMS].SetOfColors [NumOfCollor]);
				NumOfCollor++;
			} else {
				NumOfCollor = 0;
				CountMS++;
			}
		}
		CollorOfBalls = FullSetOfCollors;
		CreateNewBall (); // create first ball
		if (EditCameraSize) {
			SettingOfCameraSize (); // edit camera size
		}
	}


	void CreateNewBall(){


        if (idDebug)
        {
            MoveBall = Instantiate(MultiBall);
            MoveBall.transform.eulerAngles = CurrentBall.transform.eulerAngles;
            MoveBall.transform.localScale = CurrentBall.transform.localScale;
            MoveBall.transform.position = PivotForNewBalls.position;
            MoveBall.transform.SetParent(PivotForNewBalls);
            MoveBall.name = "throw ball";
            MoveBall.SetActive(true);
            if (GetComponent<BonuseCenter>().Fire_ball)
            {
                GetComponent<BonuseCenter>().Fire_ball = false;
                if (GameObject.Find("Sound Center").GetComponent<SoundCenter>().Bonus_fire_ball != null)
                {
                    MoveBall.GetComponent<AudioSource>().PlayOneShot(GameObject.Find("Sound Center").GetComponent<SoundCenter>().Bonus_fire_ball);
                }
            }
            SaveComponent = MoveBall.GetComponent<BallControl>();
            StartCoroutine("AnimationOfSprite");
        }
        else
        {


            if (!GetComponent<BonuseCenter>().Fire_ball)
            { // if it isn`t fire
                if (GetComponent<BonuseCenter>().of_destroyBall == 0)
                { // if it isn`t object that can destroy a ball
                    if (GetComponent<BonuseCenter>().Destroy_color)
                    { // if it`s arrow
                        GetComponent<BonuseCenter>().Destroy_color = false;
                        MoveBall = Instantiate(DestroyColor);
                    }
                    else if (GetComponent<BonuseCenter>().Multi_ball)
                    { // if it`s multi-ball
                        MoveBall = Instantiate(MultiBall);
                        GetComponent<BonuseCenter>().Multi_ball = false;
                    }
                    else
                    { // if it`s simple ball
                        MoveBall = Instantiate(CurrentBall);
                        if (CollorOfBalls.Count == 0)
                        {
                            if (arrColorsForNextBall[0] != Color.clear)
                            {
                                arrColorsForNextBall[0] = arrColorsForNextBall[1];
                                arrColorsForNextBall[1] = GetComponent<MainStart>().SetOfColors[Random.Range(0, GetComponent<MainStart>().SetOfColors.Length)];
                            }
                            else
                            {
                                arrColorsForNextBall[0] = GetComponent<MainStart>().SetOfColors[Random.Range(0, GetComponent<MainStart>().SetOfColors.Length)];
                                arrColorsForNextBall[1] = GetComponent<MainStart>().SetOfColors[Random.Range(0, GetComponent<MainStart>().SetOfColors.Length)];
                            }

                        }
                        else
                        {
                            if (arrColorsForNextBall[0] != Color.clear)
                            {
                                arrColorsForNextBall[0] = arrColorsForNextBall[1];
                                arrColorsForNextBall[1] = CollorOfBalls[Random.Range(0, CollorOfBalls.Count)];
                            }
                            else
                            {
                                arrColorsForNextBall[0] = CollorOfBalls[Random.Range(0, CollorOfBalls.Count)];
                                arrColorsForNextBall[1] = CollorOfBalls[Random.Range(0, CollorOfBalls.Count)];
                            }
                        }
                        MoveBall.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[0];
                        // colorForNextBall.GetComponent<BallChangeTrigger>().NextBall.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[1];
                        colorForNextBall.GetComponent<BallChangeTrigger>().BorderForPlate.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[1];
                    }
                }
                else
                { //if it`s object that can destroy a ball
                    MoveBall = Instantiate(DestroyBall);
                    GetComponent<BonuseCenter>().of_destroyBall--;
                }
            }
            else
            { // if it`s fire
                MoveBall = Instantiate(FireBall);
            }
            MoveBall.transform.eulerAngles = CurrentBall.transform.eulerAngles;
            MoveBall.transform.localScale = CurrentBall.transform.localScale;
            MoveBall.transform.position = PivotForNewBalls.position;
            MoveBall.transform.SetParent(PivotForNewBalls);
            MoveBall.name = "throw ball";
            MoveBall.SetActive(true);
            if (GetComponent<BonuseCenter>().Fire_ball)
            {
                GetComponent<BonuseCenter>().Fire_ball = false;
                if (GameObject.Find("Sound Center").GetComponent<SoundCenter>().Bonus_fire_ball != null)
                {
                    MoveBall.GetComponent<AudioSource>().PlayOneShot(GameObject.Find("Sound Center").GetComponent<SoundCenter>().Bonus_fire_ball);
                }
            }
            
           
            SaveComponent = MoveBall.GetComponent<BallControl>();
            StartCoroutine("AnimationOfSprite");
        }
	}

	public IEnumerator AnimationOfSprite(){ // animation off object (example: ball, multi-ball etc.)]
        bool isAnim = true;
        ThrowBall tb =  SaveComponent.GetComponent<ThrowBall>();
        if (tb != null) {
            if (tb.This_destroyBall || tb.This_multiBall) {
                isAnim = false;
            }
        }
        if (isAnim)
        {
            SaveComponent.AnimationOfBall(0.5f);
            yield return new WaitForFixedUpdate();
            StartCoroutine("AnimationOfSprite");

        }
        else {
            yield return new WaitForFixedUpdate();
        }
		
	}

	
	void FixedUpdate(){
		if (Player_off == 0) { // if player can throw
			if (CanThrow.x == 0) {
				if (Input.GetMouseButton (0)) {
                    // work with touch
					Ray ray = GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit, 100, ShotInputLayer)) {
						ToTouch = hit.point;
                       if (isInArea(ToTouch.x, ToTouch.y))
                        {
                            changeColorArr();
                            return;
                        }
                        if (isInArea2(ToTouch.x, ToTouch.y))
                        {
                            changePlayerPos();
                            return;
                        }
                        WorldTouch = new Vector3 (ToTouch.x, ToTouch.y, Player.transform.position.z);
						Player.transform.LookAt (WorldTouch);
                        Vector3 trig = colorForNextBall.GetComponent<Transform>().position;
                        if (ToTouch.x > Player.transform.position.x)
                        {
                          
                            Player.transform.eulerAngles = new Vector3(0, 0, -Player.transform.eulerAngles.x);
                            if (isInArea(ToTouch.x, ToTouch.y)) 
                            {
                                changeColorArr();
                                return ;
                            }
                            if (isInArea2(ToTouch.x, ToTouch.y)) {
                                Debug.Log("It's working");
                                changePlayerPos();
                                return;
                            }
                        }
                        else
                        {
                            //colorForNextBall.GetComponent<SpriteRenderer>().color = GetComponent<MainStart>().SetOfColors[Random.Range(0, GetComponent<MainStart>().SetOfColors.Length)];
                            Player.transform.eulerAngles = new Vector3(0, 0, 180 + Player.transform.eulerAngles.x);
                            if (isInArea(ToTouch.x, ToTouch.y)) 
                            {
                                changeColorArr();
                                return;
                            }
                            if (isInArea2(ToTouch.x, ToTouch.y))
                            {
                                Debug.Log("It's working");
                                changePlayerPos();
                                return;
                            }
                        }
                     //   CanThrow.x = 0; CanThrow.y = 0; CanThrow.z = 0;


                        if (CanThrow.x == 0 && CanThrow.y == 0 && CanThrow.z == 0 ) {
							CanThrow.z = 1;
							CanThrow.y = 1;
							StartCoroutine ("Will");
						}
					}
				} else {
					CanThrow.y = 0;
				}
			}
		} else if (Player_off > 0) { // if it`s LOSE
			if (LastBall == null) {
				Player_off = -1;
				GameObject.Find ("Canvas").GetComponent<InGame> ().onceClick = 1;
				int CountMS = 0;
				while (CountMS < SetOfLines.Length) {
					SetOfLines [CountMS].EndOfLine = true;
					CountMS++;
				}
				IsGameOver ();
			}
		}
		if (SaveComponent != null) {
			if (SaveComponent.GetComponent<ThrowBall> () == null) {
				SaveComponent = null;
			}
		}
	}

    //test function
    private bool isInArea(float x, float y) {
        return false;
        /*Vector3 v= colorForNextBall.GetComponent<Transform>().position;
        Vector3 v2 = colorForNextBall.GetComponent<Transform>().localScale;
        Debug.Log("scale x=" + v2.x + "| y=" + v2.y);
        float radX = v2.x / 2;
        float radY = v2.y / 2;
        float minX = v.x - radX;
        float maxX = v.x + radX;
        float minY = v.y - radY;
        float maxY = v.y + radY;
        if (x > maxX || x < minX || y > maxY || y < minY)
        {
            return false;
        }
        else {
            return true;
        }*/
    }

    private bool isInArea2(float x, float y)
    {
        if (ClickPlate == null) {
            return false;
        }
        Vector3 v = ClickPlate.GetComponent<Transform>().position;
        Vector3 v2 = ClickPlate.GetComponent<Transform>().localScale;
        Debug.Log("scale x=" + v2.x + "| y=" + v2.y);
        float radX = v2.x / 2;
        float radY = v2.y / 2;
        float minX = v.x - radX;
        float maxX = v.x + radX;
        float minY = v.y - radY;
        float maxY = v.y + radY;
        if (x > maxX || x < minX || y > maxY || y < minY)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    IEnumerator corChange() {
        isChangePos = true;
        Debug.Log("pizda1");
        yield return new WaitForSeconds(0.15f);
        Debug.Log("pizda2");
        isChangePos = false;
    }

    private void changePlayerPos() {
        StartCoroutine(corChange());
        Debug.Log("pizda4");
        float nowTime = Time.time;
        if (prevTime2 == 0)
        {
            prevTime2 = nowTime - 0.3f;
        }
        float minTime0 = nowTime - prevTime;
        prevTime2 = nowTime;
        if (minTime0 < minTime)
        {
            return;
        }
        Vector3 oldPos = Player.GetComponent<Transform>().position;
        Player.GetComponent<Transform>().position = ClickPlate.GetComponent<Transform>().position;
        GameObject pl = ClickPlate.GetComponent<BallChangeTrigger>().Plate;
        if (pl != null) {
            pl.GetComponent<Transform>().position = oldPos;
        }
        Debug.Log("pizda7");

    }

    private void changeColorArr() {
        /* Color pass = arrColorsForNextBall[0];
         arrColorsForNextBall[0] = arrColorsForNextBall[1];
         arrColorsForNextBall[1] = pass;
         MoveBall.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[0];
         colorForNextBall.GetComponent<BallChangeTrigger>().NextBall.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[1];*/
        Debug.Log("pizda3");
        float nowTime = Time.time;
        if (prevTime == 0)
        {
            prevTime = nowTime - 0.3f;
        }
        float minTime0 = nowTime - prevTime;
        prevTime = nowTime;
        if (minTime0 < minTime || isChangePos) {
            Debug.Log("pizda5");
            return;
        }

        MoveBall.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[1];
       // colorForNextBall.GetComponent<BallChangeTrigger>().NextBall.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[0];
        colorForNextBall.GetComponent<BallChangeTrigger>().BorderForPlate.GetComponent<SpriteRenderer>().color = arrColorsForNextBall[0];
        arrColorsForNextBall[0] = arrColorsForNextBall[1];

        arrColorsForNextBall[1] = colorForNextBall.GetComponent<BallChangeTrigger>().BorderForPlate.GetComponent<SpriteRenderer>().color;
        Debug.Log("pizda6");
    } 

	IEnumerator Will(){ // throw ball
			MoveBall.transform.SetParent (CurrentBall.transform.parent);
			MoveBall.transform.SetParent (GameObject.Find ("Will Create").transform);
			MoveBall.GetComponent<ThrowBall> ().enabled = true;
			OfPlayer.GetComponent<Animation> ().Play (ThrowOfPlayer.name);
			if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Player_throw_ball != null){
				GameObject.Find ("SC 0").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Player_throw_ball);
			}
			StopCoroutine ("AnimationOfSprite");
			if (MoveBall.GetComponent<ThrowBall> ().This_fireBall) {
				yield return new WaitForSeconds (1.5f); 
			}
			CreateNewBall ();
        yield return new WaitForSeconds (0.5f); 
			CanThrow.z = 0;
	}

	public void AccessToThrow(){ // for access to throw after start game
		if (EndOfStartSpeed == SetOfLines.Length) {
			CanThrow.z = 0;
		}
	}


	public void SearchAvCollors(){ // when some color is not in line
		int CountMS = 0;
		CollorOfBalls = new List<Color>{ };
		while (CountMS < SetOfLines.Length) {
			if (SetOfLines [CountMS].GeneratedBalls.Count < 15) {
				int ipoint = 0;
				int countGB = 0;
				int countCB = 0;
				while (ipoint == 0) {
					if (countGB < SetOfLines [CountMS].GeneratedBalls.Count) {
						if (CollorOfBalls.Count != 0) {
							if (countCB < CollorOfBalls.Count) {
								if (SetOfLines [CountMS].GeneratedBalls [countGB].GetComponent<SpriteRenderer> ().color != CollorOfBalls [countCB]) {
									countCB++;
								} else {
									countGB++;
									countCB = 0;
								}
							} else {
								CollorOfBalls.Add (SetOfLines [CountMS].GeneratedBalls [countGB].GetComponent<SpriteRenderer> ().color);
								countGB++;
								countCB = 0;
							}
						} else {
							CollorOfBalls.Add (SetOfLines [CountMS].GeneratedBalls [countGB].GetComponent<SpriteRenderer> ().color);
							countGB++;
						}
					} else {
						ipoint = 1;
					}

				}
				CountMS++;
			} else {
				CountMS = SetOfLines.Length;
				CollorOfBalls = FullSetOfCollors;
			}
		}

	}

	public void WinProcess (MainStart GetScript){ // WIN
		GetScript.StopCoroutine ("GenerateLine");
		GetScript.StopCoroutine ("OnlineSend");
		GetScript.Active = false;
		int CountOfMS = 0;
		int SaveLinesActive = 0;
		while (CountOfMS < SetOfLines.Length) {
			if (!SetOfLines [CountOfMS].Active) {
				SaveLinesActive++;
			}
			CountOfMS++;
		}
		if (SaveLinesActive == SetOfLines.Length){
		GetComponent<ControlOfPl> ().CanThrow = new Vector3 (1,1,1);
		GameObject.Find ("Canvas").GetComponent<InGame> ().ToWin (GetComponent<ControlOfPl>().Score);
		}
	}


	public void IsGameOver(){ //LOSE
		int CountMS = 0;
		int GlobalCount = 0;
		int GlobalSBProgress = 0;
		while(CountMS < SetOfLines.Length){
			SetOfLines[CountMS].BaseSpeed = 2;
			SetOfLines[CountMS].StartSpeed = 2;
			GlobalCount += SetOfLines [CountMS].GeneratedBalls.Count;
			GlobalSBProgress += SetOfLines [CountMS].SBProgress;
			CountMS++;
		}
		int Progress = 100 - GlobalCount * 100 / GlobalSBProgress;
		if (Progress < 0) {
			Progress = 0;
		}
		StartCoroutine (AwaitForLose(Progress));
	}

	IEnumerator AwaitForLose(int Progress){ // after LOSE
		int CountMS = 0;
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Lose != null){
			GameObject.Find ("SC 4").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Lose);
		}
		while(CountMS < SetOfLines.Length){
			if (SetOfLines [CountMS].GeneratedBalls.Count > 0) {
				SetOfLines[CountMS].BaseSpeed = 2;
			} else {
				CountMS++;
			}
			yield return new WaitForFixedUpdate ();
		}
		GameObject.Find ("Canvas").GetComponent<InGame> ().ToLose (Progress);
	}

	public void AddScoreGlobal(int score, Color color, Vector3 position){ // count of score
		if (color != new Color (1, 1, 1, 1)) {
			Score += score;
			GameObject NewScore = Instantiate (ExampleScore);
			SpriteRenderer SpriteOfScore = NewScore.GetComponent<ScoreInBall> ().ThisChild;
			SpriteOfScore.color = color;
			NewScore.transform.position = position;
			if (score == Score_OneBall) {
				SpriteOfScore.sprite = Sprite_OneBall;
			}
			if (score == Score_MoreThree) {
				SpriteOfScore.sprite = Sprite_MoreThree;
			}
			if (score == Score_Bonuse) {
				SpriteOfScore.sprite = Sprite_Bonuse;
			}
			if (score == Score_Combo) {
				SpriteOfScore.sprite = Sprite_Combo;
			}
			NewScore.SetActive (true);
			UIViewScore.text = "Score: " + Score.ToString ();
		}
	}

	void SettingOfCameraSize (){
		float t_lerp = (Screen.width / Screen.height) / 1.78f;
		GetComponent<Camera> ().orthographicSize = Mathf.Lerp (7,5,t_lerp);
	}



}
