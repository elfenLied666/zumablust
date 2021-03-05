using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainStart : MonoBehaviour {

    public static int countMain = 0;

	[Header("Line")]
	public bool Active = true; // active line 
	[Header("Examples of ball")]
	public GameObject BaseBall; // example of ball
	public GameObject DestroyBall; // example of ball that destroy
	[Header("Colors that will be used")]
	public Color32[] SetOfColors; // colors in line
	[Header("Number of balls")]
	public int NumberOfBalls; // global count of balls 
	[Header("Base speed of balls")]
	public float BaseSpeed; // base speed 
	[Header("Start parameters")]
	public int StartBalls; // balls that come (when start)
	public float StartSpeed; // speed of balls that in start
	[HideInInspector]
	public int SBProgress; // count of balls for calculate (progress in LOSE)
	public Transform PSStart; // start glow that come to finish
	public GameObject LoseGlow; // sprite that in finish of line
    public GameObject LoseGlow2;
    public GameObject LoseGlow3;
    [SerializeField] private Sprite LoseGlowSpriteClose;
    [SerializeField] private Sprite LoseGlowSpriteOpen;
    [SerializeField] private Sprite LoseGlowSpriteClose2;
    [SerializeField] private Sprite LoseGlowSpriteOpen2;

    [HideInInspector]
	public List<BallControl> GeneratedBalls; // list of balls 

	//[HideInInspector]
	public float distance; // diameter of ball
	[HideInInspector]
	public float interval; // interval between two points (that from curve)
	[HideInInspector]
	public Vector2 ConvertTo; // x - count of interval; y - other float, that < interval; @convert parameters
	private GameObject NewBall; // object that content a new ball
	private float c_distance; // parameter of distance -> for generate balls in start
	private float d_distance; // parameter of distance -> for generate balls in start
	[HideInInspector]
	public float saveBaseSpeed; // save base speed
	[HideInInspector]
	public bool EndOfLine = false; // true if "LOSE"
	private ControlOfPl GetOnce;
	private bool reg_start = false;
	[HideInInspector]
	public int IndexStop = -1;
	[HideInInspector]
	public int mode_IndexStop = -1;

	[Header("For Scripts")]
	public BallControl TimeBall;
	private int SaveFirstBall;



	void Start(){
      //  SimpleAds.GetInstance().ShowGoogleAd(SimpleAds.START_LEVEL);
        if (Active) {
            countMain++;
            PSStart.gameObject.SetActive (true);
			SBProgress = NumberOfBalls; // save start count of balls 
			GetOnce = GetComponent<ControlOfPl> (); // save component of ControlOfPl for use in script
			BaseBall.GetComponent<BallControl> ().ccObject.SendTo = this; // send this script in CurveCreator
			saveBaseSpeed = BaseSpeed; // save start base speed
			interval = BaseBall.GetComponent<BallControl> ().ccObject.interval; // get interval from CurveCreator
			distance = BaseBall.GetComponent<SphereCollider> ().radius * 2f * BaseBall.transform.localScale.z; // calculate diameter of ball
			GeneratedBalls = new  List<BallControl> { }; // create empty list of balls
			float d = distance; // create float for convert diameter in point distance
			while (d >= interval) { // if distance > one interval 
				ConvertTo.x += 1; // add one interval ass point 
				d -= interval; // delete distance off interval from diameter
			}
			if (d > 0) {
				ConvertTo.y = d; // if distanse < interval then add other float
			}
			GetOnce.CanThrow = new Vector3 (GetOnce.CanThrow.x, GetOnce.CanThrow.y, 1); // stop of work`s player
			if (StartBalls > NumberOfBalls) { // if start balls > or = count of balls
				StartBalls = NumberOfBalls;
			}
			if (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Start_of_game != null) { // sound of start game
				GameObject.Find ("SC 4").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Start_of_game);
			}
            //SimpleAds.GetInstance().ShowGoogleAd(SimpleAds.START_LEVEL);
            StartCoroutine (iStart ()); // start other in IEnumerator
		} else {
			PSStart.gameObject.SetActive (false);
		}
       
	}

   

	IEnumerator iStart(){ // it need for start glow that go to finish
        
        int MaxC = BaseBall.GetComponent<BallControl> ().ccObject.SetOfPoints.Count;
		int FromStartPos = 0;
		while (FromStartPos < MaxC - 4) {
			FromStartPos += 2;
			Vector3 PosOfScore = BaseBall.GetComponent<BallControl> ().ccObject.GetNewVector (BaseBall.GetComponent<BallControl> (), 0.01f, 0, FromStartPos, false);
			PSStart.position = PosOfScore;
			yield return new WaitForFixedUpdate ();
		}
		PSStart.position = BaseBall.GetComponent<BallControl> ().ccObject.SetOfPoints[BaseBall.GetComponent<BallControl> ().ccObject.SetOfPoints.Count - 1];
		yield return new WaitForSeconds (1);
		Destroy (PSStart.gameObject);     
        StartCoroutine ("GenerateLine"); // Start Generate of line
		StartCoroutine ("OnlineSend"); // Start Move of generated balls
	}


	public IEnumerator GenerateLine(){
		if (NumberOfBalls > 0) { // it need if number of balls = 0 but regstart = true
			if (StartBalls > 0 && BaseSpeed != StartSpeed && !reg_start) {
				BaseSpeed = StartSpeed; // new speed for start balls
				GetOnce.CanThrow = new Vector3 (GetOnce.CanThrow.x, GetOnce.CanThrow.y, 1); // player can`t throw
			} else if (StartBalls <= 0 && BaseSpeed != saveBaseSpeed && !reg_start) {
				BaseSpeed = saveBaseSpeed; // new base speed for normal mode
				GetOnce.EndOfStartSpeed++; // reg of end for one line
				GetOnce.AccessToThrow (); // Get access to throw
				reg_start = true; // reg of end start of this line
			}
			if (c_distance <= distance && BaseSpeed > 0) {
				c_distance += BaseSpeed * interval; // calculate distance between balls // When it = diameter -> create new ball
			} else if (BaseSpeed > 0) {
                //Можно предположить что тут смотриться а нужен ли новый шарик
				bool some = true; // create simple bool
				c_distance -= distance - d_distance; // calculate approx. distance
				d_distance = c_distance; // remains of distance 
				if (d_distance > interval) {
					if ((GeneratedBalls [GeneratedBalls.Count - 1].c - ConvertTo.x) < 2) {
						d_distance = 0;
						c_distance = 0;
						some = false;
					}
				}
				if (some) { // if create new ball
					if (StartBalls > 0) { // if this start balls 
						StartBalls--;
					}
					NumberOfBalls--; // count off balls that will be
					NewBall = Instantiate (BaseBall); // create 
					NewBall.name = "ball"; // set name 
					NewBall.transform.SetParent (BaseBall.transform.parent.transform); // set transform
					NewBall.GetComponent<SpriteRenderer> ().color = SetOfColors [Random.Range (0, SetOfColors.Length)]; // get color of ball
					NewBall.SetActive (true); // activate of ball
					NewBall.GetComponent<SpriteRenderer> ().sortingOrder = GeneratedBalls.Count; // set sorting order of sprite
					GeneratedBalls.Add (NewBall.GetComponent<BallControl> ()); // add this ball in array

					if (GeneratedBalls.Count > 1) { // set parameters to ball
						GeneratedBalls [GeneratedBalls.Count - 1].c = -(int)ConvertTo.x + GeneratedBalls [GeneratedBalls.Count - 2].c; // sec c
						GeneratedBalls [GeneratedBalls.Count - 1].t = -ConvertTo.y / interval + GeneratedBalls [GeneratedBalls.Count - 2].t; // set t
						if (GeneratedBalls [GeneratedBalls.Count - 1].t < 0) { // if t < 0
							GeneratedBalls [GeneratedBalls.Count - 1].t++;
							GeneratedBalls [GeneratedBalls.Count - 1].c--;
						}
					}
				}

			}
		} else if (!reg_start) { // if NumberOfBalls = StartBalls
			BaseSpeed = saveBaseSpeed;
			GetOnce.EndOfStartSpeed++;
			GetOnce.AccessToThrow ();
			reg_start = true; 
		}
		yield return new WaitForFixedUpdate ();
		StartCoroutine ("GenerateLine");
	}

	IEnumerator OnlineSend(){ // as fixedUpdate
		if (BaseSpeed > 0.01f) { // if line move to finish
			OnlyMove ((GeneratedBalls.Count - 1), BaseSpeed, IndexStop, 0); // start function of move for balls
			if(IndexStop >= 0){ // if line has a empty space 
				if (IndexStop < (GeneratedBalls.Count - 1) && (interval * GeneratedBalls [IndexStop].c + interval * GeneratedBalls [IndexStop].t) - (interval * GeneratedBalls [IndexStop + 1].c + interval * GeneratedBalls [IndexStop + 1].t) < distance * 1.01f) { // if distance of empty space = 0
					IndexStop = FindStopIndex (IndexStop, 1f, BaseSpeed, 0); // find a new empty space
				}
			}
		}

		if (BaseSpeed < -0.01f) { // if line move back
			if (GeneratedBalls.Count > 0) {
				if (IndexStop != -1) { // if line has a empty space 
					OnlyMove (0, BaseSpeed, IndexStop + 1, 0); // move
				} else { // if line hasn`t empty space 
					OnlyMove (0, BaseSpeed, -1, 0); // move
				}

				if(IndexStop >= 0){ // if line has a empty space 
					if (IndexStop < (GeneratedBalls.Count - 1) && (interval * GeneratedBalls [IndexStop].c + interval * GeneratedBalls [IndexStop].t) - (interval * GeneratedBalls [IndexStop + 1].c + interval * GeneratedBalls [IndexStop + 1].t) < distance * 1.01f) { // if distance of empty space = 0
						IndexStop = FindStopIndex (IndexStop, 1f, BaseSpeed, 0); // find a new empty space
					}
				}

				if (GeneratedBalls [GeneratedBalls.Count - 1].c <= 0) { // if ball come back in start
					NumberOfBalls++; // add ball 
					Destroy (GeneratedBalls [GeneratedBalls.Count - 1].gameObject); // destroy this ball
					GeneratedBalls.RemoveAt (GeneratedBalls.Count - 1); // remove it from Array
				}
				if (GeneratedBalls.Count > 0) {
					c_distance = GeneratedBalls [GeneratedBalls.Count - 1].c * interval + GeneratedBalls [GeneratedBalls.Count - 1].t * interval; // calculate distance
				}
			}
		}

		if (GeneratedBalls.Count < 10 && NumberOfBalls == 0 && GeneratedBalls.Count > 0) {
			SaveFirstBall = GeneratedBalls [0].c; // save c of dirst ball, it need for win
		}
		// for glow (sprite) that in finish of line
		if (GeneratedBalls.Count > 1) {
			if (!LoseGlow.activeInHierarchy) {
				if (GeneratedBalls [0].GetComponent<BallControl> ().c > BaseBall.GetComponent<BallControl> ().ccObject.SetOfPoints.Count * 0.9f) {
                    //LoseGlow.SetActive (true);
                    LoseGlow3.GetComponent<SpriteRenderer>().sprite = LoseGlowSpriteOpen;
                    LoseGlow2.GetComponent<SpriteRenderer>().sprite = LoseGlowSpriteOpen;

				}
			} else {
				if (GeneratedBalls [0].GetComponent<BallControl> ().c < BaseBall.GetComponent<BallControl> ().ccObject.SetOfPoints.Count * 0.9f) {
                    //LoseGlow.SetActive (false);
                    LoseGlow3.GetComponent<SpriteRenderer>().sprite = LoseGlowSpriteClose;
                    LoseGlow2.GetComponent<SpriteRenderer>().sprite = LoseGlowSpriteClose;
                }
			}
		}
		// FIND WIN
		if (GeneratedBalls.Count == 0 && NumberOfBalls == 0 && !EndOfLine){ // look at win in this line
			NumberOfBalls = -1; // set default number of balls
			StartCoroutine (AddWinScore());
		}
		yield return new WaitForFixedUpdate ();
		StartCoroutine ("OnlineSend");
	}

	public int FindStopIndex (int fromIndex, float Wdist, float newBaseSpeed, int additional_find){ // return new Stop Index
		int point = 0; // simple int
		if (additional_find == 0) { // change index for calculate of distance
			if (newBaseSpeed >= 0) { // if speed >= 0
				while (point == 0) {
					if (fromIndex >= 0 && fromIndex < GeneratedBalls.Count) {
						if (fromIndex <= (GeneratedBalls.Count - 2) && (interval * GeneratedBalls [fromIndex].c + interval * GeneratedBalls [fromIndex].t) - (interval * GeneratedBalls [fromIndex + 1].c + interval * GeneratedBalls [fromIndex + 1].t) < distance * 1.05f * Wdist) {
							fromIndex--;
						} else if (fromIndex <= (GeneratedBalls.Count - 2)) {
							point = 1;
						} else {
							point = 1;
						}
					} else {
						point = 1;
					}
				}
			} else { // if speed < 0
				while (point == 0) {
					if (fromIndex < GeneratedBalls.Count - 1 && fromIndex != -1) {
						if ((interval * GeneratedBalls [fromIndex].c + interval * GeneratedBalls [fromIndex].t) - (interval * GeneratedBalls [fromIndex + 1].c + interval * GeneratedBalls [fromIndex + 1].t) < distance * 1.05f * Wdist) {
							fromIndex++;
						} else {
							point = 1;
						}
					} else { // else if fromindex = -1
						fromIndex = -1;
						point = 1;
					}
				}
			}
		} else {
			if (newBaseSpeed >= 0) {
				while (point == 0) {
					if (fromIndex > 1 && fromIndex < GeneratedBalls.Count) {
						if ((interval * GeneratedBalls [fromIndex - 1].c + interval * GeneratedBalls [fromIndex - 1].t) - (interval * GeneratedBalls [fromIndex].c + interval * GeneratedBalls [fromIndex].t) < distance * 1.05f * Wdist) {
							fromIndex--;
						} else {
							point = 1;
						}
					} else {
						fromIndex = -1;
						point = 1;
					}
				}
			} else {
				while (point == 0) {
					if (fromIndex < GeneratedBalls.Count - 1) {
						if ((interval * GeneratedBalls [fromIndex].c + interval * GeneratedBalls [fromIndex].t) - (interval * GeneratedBalls [fromIndex + 1].c + interval * GeneratedBalls [fromIndex + 1].t) < distance * 1.05f * Wdist) {
							fromIndex++;
						} else {
							point = 1;
						}
					} else {
						fromIndex = GeneratedBalls.Count - 1;
						point = 1;
					}
				}
			}
		}
		return fromIndex;
	}

	public void OnlyMove(int fromIndex, float newBaseSpeed, int SIndex, int additional_move){ // function of move for balls
		if (newBaseSpeed > 0) { // if speed > 0
			while (fromIndex >= 0 && fromIndex < GeneratedBalls.Count) {
				if (fromIndex != SIndex) { // STOP?
					GeneratedBalls [fromIndex].LocalUpdate (newBaseSpeed); //start function of move in ball
				} else {
					fromIndex = -1;
				}
				fromIndex--;
			}
		}

		if (newBaseSpeed < 0) { // id speed < 0
			if (additional_move == 0) { // sort to end of line
				while (fromIndex < GeneratedBalls.Count) {
					if (fromIndex != SIndex && fromIndex >= 0) { // STOP?
						GeneratedBalls [fromIndex].LocalUpdate (newBaseSpeed);
					} else {
						fromIndex = GeneratedBalls.Count;
					}
					fromIndex++;
				}
			} else { // sort to start of line
				while (fromIndex >= 0) {
					if (fromIndex != SIndex) { // STOP?
						GeneratedBalls [fromIndex].LocalUpdate (newBaseSpeed);
					} else {
						GeneratedBalls [SIndex].LocalUpdate (newBaseSpeed);
						fromIndex = -1;
					}
					fromIndex--;
				}
			}
		}
	}


	public void NormalizeLine(int fromIndex){ // need for edit distances between balls if (it < diameter of ball)
		if (fromIndex <= GeneratedBalls.Count - 1 && fromIndex > 0) { // need for edit index (when calculate Qspeed)
			float Qspeed = (interval * GeneratedBalls [fromIndex - 1].c + interval * GeneratedBalls [fromIndex - 1].t) - (interval * GeneratedBalls [fromIndex].c + interval * GeneratedBalls [fromIndex].t);
			if (Qspeed < distance) {
				Qspeed = (distance - Qspeed) / interval;
				if (Qspeed > 0.01f) {
					OnlyMove (fromIndex, Qspeed, -1, 1);
				}
			}
		} else if (fromIndex < GeneratedBalls.Count - 1 && fromIndex >= 0) {
			float Qspeed = (interval * GeneratedBalls [fromIndex].c + interval * GeneratedBalls [fromIndex].t) - (interval * GeneratedBalls [fromIndex + 1].c + interval * GeneratedBalls [fromIndex + 1].t);
			if (Qspeed < distance) {
				Qspeed = (distance - Qspeed) / interval;
				if (Qspeed > 0.01f) {
					OnlyMove (fromIndex, Qspeed, -1, 1);
				}
			}
		}
	}

	public void GlobalNormalizeOfLine(){ // need for edit distances between balls in this line if (it < diameter of ball)
		int iIndex = 0;
		while(iIndex <= GeneratedBalls.Count - 1){
			if (iIndex <= GeneratedBalls.Count - 1 && iIndex > 0) {
				float Qspeed = (interval * GeneratedBalls [iIndex - 1].c + interval * GeneratedBalls [iIndex - 1].t) - (interval * GeneratedBalls [iIndex].c + interval * GeneratedBalls [iIndex].t);
				if (Qspeed < distance) {
					Qspeed = (distance - Qspeed) / interval;
					if (Qspeed > 0.01f) {
						OnlyMove ((iIndex - 1), Qspeed, -1, 1);
					}
				}
			}
			iIndex++;
		}
	}


	public void FindOfColors(int index, bool thisCombo){ // find balls that with one color
        int indNext = 0;
        int indPrev = 0;
        bool isPrevMove = false;
        GlobalNormalizeOfLine ();
		List<BallControl> OfColor = new List<BallControl>{ }; // create list of balls 
		OfColor.Add (GeneratedBalls[index]); // add first ball
		int SecondIndex = index; // save start index of first ball
		int Point = 0;
		int create_from_id = 0;
		int minIndex = 0;
		if (GeneratedBalls[index].GetComponent<SpriteRenderer>().color != Color.white) { // if it`s not multi-ball
			while (Point == 0) {
				SecondIndex--;
				if (SecondIndex >= 0) {
					if (GeneratedBalls [SecondIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [index].GetComponent<SpriteRenderer> ().color) {
						if ((interval * GeneratedBalls [SecondIndex].c + interval * GeneratedBalls [SecondIndex].t) - (interval * GeneratedBalls [SecondIndex + 1].c + interval * GeneratedBalls [SecondIndex + 1].t) < distance * 1.1f || thisCombo) {
							OfColor.Add (GeneratedBalls [SecondIndex]);
						} else {
							create_from_id = SecondIndex;
							Point = 1;
						}
					} else {
						create_from_id = SecondIndex;
						Point = 1;
					}
				} else {
					Point = 1;
				}
			}
			minIndex = SecondIndex;
			SecondIndex = index;
			while (Point == 1) {
				SecondIndex++;
				if (SecondIndex < GeneratedBalls.Count) {
					if (GeneratedBalls [SecondIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [index].GetComponent<SpriteRenderer> ().color) {
						if ((interval * GeneratedBalls [SecondIndex - 1].c + interval * GeneratedBalls [SecondIndex - 1].t) - (interval * GeneratedBalls [SecondIndex].c + interval * GeneratedBalls [SecondIndex].t) < distance * 1.1f || thisCombo) {
							OfColor.Add (GeneratedBalls [SecondIndex]);
						} else {
							Point = 2;
						}
					} else {
						Point = 2;
					}
				} else {
					Point = 2;
				}
			}
		} else {
			// if this multi-ball
			int SetIndex_1 = index - 1;
			int SetIndex_2 = index + 1;
			OfColor.Add (GeneratedBalls [index]);
			if (SetIndex_1 >= 0){
				Point = 0;
				OfColor.Add (GeneratedBalls [SetIndex_1]);
				SecondIndex = SetIndex_1;
				while (Point == 0) {
					SecondIndex--;
					if (SecondIndex >= 0) {
						if (GeneratedBalls [SecondIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [SetIndex_1].GetComponent<SpriteRenderer> ().color) {
							if ((interval * GeneratedBalls [SecondIndex].c + interval * GeneratedBalls [SecondIndex].t) - (interval * GeneratedBalls [SecondIndex + 1].c + interval * GeneratedBalls [SecondIndex + 1].t) < distance * 1.1f || thisCombo) {
								OfColor.Add (GeneratedBalls [SecondIndex]);
							} else {
								create_from_id = SecondIndex;
								Point = 1;
							}
						} else {
							create_from_id = SecondIndex;
							Point = 1;
						}
					} else {
						Point = 1;
					}
				}
			}
			if (SetIndex_2 < GeneratedBalls.Count){
				Point = 0;
				OfColor.Add (GeneratedBalls [SetIndex_2]);
				SecondIndex = SetIndex_2;
				while (Point == 0) {
					SecondIndex++;
					if (SecondIndex < GeneratedBalls.Count) {
						if (GeneratedBalls [SecondIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [SetIndex_2].GetComponent<SpriteRenderer> ().color) {
							if ((interval * GeneratedBalls [SecondIndex - 1].c + interval * GeneratedBalls [SecondIndex - 1].t) - (interval * GeneratedBalls [SecondIndex].c + interval * GeneratedBalls [SecondIndex].t) < distance * 1.1f || thisCombo) {
								OfColor.Add (GeneratedBalls [SecondIndex]);
							} else {
								Point = 2;
							}
						} else {
							Point = 2;
						}
					} else {
						Point = 2;
					}
				}
			}
		}
		Color ColorOfBall = new Color (1,1,1,1);
		Vector3 ResultPositionCombo = GeneratedBalls [index].gameObject.transform.position; // if double collision
		// destroy balls
		if(OfColor.Count >= 3){ // if balls >= 3
			if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Delete_ball != null){
				GameObject.Find ("SC 3").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Delete_ball);
			}
			ColorOfBall = OfColor [OfColor.Count - 1].GetComponent<SpriteRenderer> ().color;
            indNext = (GeneratedBalls.IndexOf(OfColor[0]));
            indPrev = GeneratedBalls.IndexOf(OfColor[0]);
            indNext +=   1;
            indPrev -=  OfColor.Count;

            if (indNext <= (GeneratedBalls.Count - 1) && indPrev > -1) {
                if (GeneratedBalls[indNext].GetComponent<SpriteRenderer>().color == GeneratedBalls[indPrev].GetComponent<SpriteRenderer>().color) {
                    isPrevMove = true;
                }
            }
            
            while (OfColor.Count > 0){
				// Destroy balls, add score
				CreateDestroyBall (OfColor[OfColor.Count - 1].gameObject);
				GeneratedBalls.Remove (OfColor[OfColor.Count - 1]);
				int ResultScore = 0;
				Vector3 ResultPosition = OfColor [OfColor.Count - 1].gameObject.transform.position;
				ResultScore = GetComponent<ControlOfPl> ().Score_OneBall;
				if (OfColor.Count > 3) {
					ResultScore = GetComponent<ControlOfPl> ().Score_MoreThree;
				}
				GetComponent<ControlOfPl> ().AddScoreGlobal (ResultScore,ColorOfBall, ResultPosition);
				Destroy (OfColor[OfColor.Count - 1].gameObject);
				OfColor.Remove (OfColor[OfColor.Count - 1]);
			}
            // if all = true -> "come back" in line
            /*&& GeneratedBalls [minIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [minIndex - 1].GetComponent<SpriteRenderer> ().color */
            /*GeneratedBalls[minIndex].GetComponent<SpriteRenderer>().color == GeneratedBalls[minIndex - 1].GetComponent<SpriteRenderer>().color*/
            /*(minIndex < (GeneratedBalls.Count - 1) && GeneratedBalls [minIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [minIndex + 1].GetComponent<SpriteRenderer> ().color &&
				    minIndex > 1) || (minIndex < (GeneratedBalls.Count - 1) && minIndex > 1 && GeneratedBalls[minIndex].GetComponent<SpriteRenderer>().color == GeneratedBalls[minIndex - 1].GetComponent<SpriteRenderer>().color) ||
				    (minIndex < GeneratedBalls.Count - 2 && GeneratedBalls [minIndex].GetComponent<SpriteRenderer> ().color == GeneratedBalls [minIndex + 2].GetComponent<SpriteRenderer> ().color)/* || 
					Random.Range(0,5) == 0*/
           
            if (minIndex != -1) {
				if ((minIndex < (GeneratedBalls.Count - 1) && GeneratedBalls[minIndex].GetComponent<SpriteRenderer>().color == GeneratedBalls[minIndex + 1].GetComponent<SpriteRenderer>().color ) /*|| (minIndex < (GeneratedBalls.Count - 1) && minIndex > 1 && GeneratedBalls[minIndex].GetComponent<SpriteRenderer>().color == GeneratedBalls[minIndex - 1].GetComponent<SpriteRenderer>().color)*/  /* ||
                  (minIndex < GeneratedBalls.Count - 2 && GeneratedBalls[minIndex].GetComponent<SpriteRenderer>().color == GeneratedBalls[minIndex + 2].GetComponent<SpriteRenderer>().color)/* || 
					Random.Range(0,5) == 0*/)
                {
					if (!EndOfLine && GetComponent<ControlOfPl>().Player_off == 0) {
						BaseSpeed = saveBaseSpeed;
						StartCoroutine (CollisionInline (create_from_id));
					}
				}
			}
			if (thisCombo) {
				GetComponent<ControlOfPl> ().AddScoreGlobal (GetComponent<ControlOfPl> ().Score_Combo,ColorOfBall, ResultPositionCombo);
			}
		}
		if (GeneratedBalls.Count > 0) {
			IndexStop = FindStopIndex ((GeneratedBalls.Count - 2), 1f, BaseSpeed, 0);
		}
		GetComponent<ControlOfPl> ().SearchAvCollors ();
	}

	public void CreateDestroyBall(GameObject forball){
		GameObject CurrentBall = Instantiate (DestroyBall);
		CurrentBall.GetComponent<ForDestroyBall> ().DestroyedObject = forball;
		CurrentBall.SetActive (true);
	}

	public IEnumerator CollisionInline(int B_from){ // function of "come back" in line
		int B_to = FindStopIndex (B_from, 1f, 0.1f, 1);
		float Dinamic_speed = -0.5f;
		int Point = 0;
		int SaveCount = GeneratedBalls.Count;
		int Size = 1;
		float save_speed = BaseSpeed;
		BaseSpeed = 0;
		while (Point == 0) {
			if(mode_IndexStop == B_from){
				Size = 2;
			}
			if (Size == 2){
				if (SaveCount != GeneratedBalls.Count){
					Size = 1;
				}
			}
			if (SaveCount != GeneratedBalls.Count && mode_IndexStop < B_from) {
				B_from++;
				SaveCount = GeneratedBalls.Count;
			}
			if (SaveCount != GeneratedBalls.Count && mode_IndexStop >= B_from) {
				SaveCount = GeneratedBalls.Count;
			}

			if (B_from < (GeneratedBalls.Count - 1)) {
				if (B_from != 0 && (interval * GeneratedBalls [B_from].c + interval * GeneratedBalls [B_from].t) - (interval * GeneratedBalls [B_from + 1].c + interval * GeneratedBalls [B_from + 1].t) > distance * 1.05f * Size) {
					OnlyMove (B_from, Dinamic_speed, B_to, 1);
					if (Dinamic_speed > -2f) {
						Dinamic_speed -= 0.1f;
					}
				} else if (B_from == 0 && (interval * GeneratedBalls [B_from].c + interval * GeneratedBalls [B_from].t) - (interval * GeneratedBalls [B_from + 1].c + interval * GeneratedBalls [B_from + 1].t) > distance * 1.05f * Size) {
					OnlyMove (B_from, Dinamic_speed, -1, 1);
					if (Dinamic_speed > -2f) {
						Dinamic_speed -= 0.1f;
					}
				} else{
					float Qspeed = (interval * GeneratedBalls [B_from].c + interval * GeneratedBalls [B_from].t) - (interval * GeneratedBalls [B_from + 1].c + interval * GeneratedBalls [B_from + 1].t);
					if (Qspeed < distance) {
						Qspeed = (distance - Qspeed) / interval;
						OnlyMove (B_from, Qspeed, B_to, 1);
					}
					if (mode_IndexStop == -1) {
						NormalizeLine (B_from);
						if (B_from < GeneratedBalls.Count - 1) {
							NormalizeLine (B_from + 1);
						}
						if (B_from > 0) {
							NormalizeLine (B_from - 1);
						}
						if (!EndOfLine) {
							BaseSpeed = save_speed;
                            Debug.Log("B_from="+B_from);
							FindOfColors (B_from + 1, true);
						}
					}
					Point = 1;
				}
			} else {
				Point = 1;
			}
			yield return new WaitForFixedUpdate ();
		}
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Collision_in_line != null){
			GameObject.Find ("SC 2").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Collision_in_line);
		}
		// after collision -> line come back
		Point = 0;
		GlobalNormalizeOfLine ();
		IndexStop = FindStopIndex (0, 1f, -0.01f, 0);
		int save_m_IS = mode_IndexStop;
		while(Point == 0){
			if (!EndOfLine) {
				if (mode_IndexStop == -1) {
					if (Dinamic_speed < 0) {
						BaseSpeed = Dinamic_speed;
						Dinamic_speed += 0.2f;
					} else {
						Point = 1;
					}
					save_m_IS = mode_IndexStop;
				} else {
					if (save_m_IS != -1) {
						Point = 1;
					} else {
						Dinamic_speed *= 0.3f;
					}
				}
			} else {
				Point = 1;
			}
			yield return new WaitForFixedUpdate ();
		}
		if (!EndOfLine) {
			if (GetComponent<BonuseCenter> ().SaveSpeed == 0) {
				BaseSpeed = saveBaseSpeed;
			} else {
				BaseSpeed = GetComponent<BonuseCenter> ().SaveSpeed;
			}
		}
		IndexStop = FindStopIndex ((GeneratedBalls.Count - 2), 1f, BaseSpeed, 0);
		GlobalNormalizeOfLine ();
	}

	IEnumerator AddWinScore(){ // win -> add score from empty line
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Win != null){
			GameObject.Find ("SC 4").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Win);
		}
		int MaxC = BaseBall.GetComponent<BallControl> ().ccObject.SetOfPoints.Count;
		while (SaveFirstBall < MaxC - 2 - ConvertTo.x) {
			SaveFirstBall += (int) ConvertTo.x + 1;
			Vector3 PosOfScore = BaseBall.GetComponent<BallControl> ().ccObject.GetNewVector (BaseBall.GetComponent<BallControl> (), 0.01f, 0, SaveFirstBall, false);
			GetComponent<ControlOfPl> ().AddScoreGlobal (GetComponent<ControlOfPl> ().Score_OneBall, SetOfColors[Random.Range(0,SetOfColors.Length - 1)], PosOfScore);
			yield return new WaitForFixedUpdate ();
		}
		yield return new WaitForSeconds (1);
		GetComponent<ControlOfPl>().WinProcess (this);
	}

}
