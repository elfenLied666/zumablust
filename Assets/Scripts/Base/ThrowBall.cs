using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ThrowBall : MonoBehaviour {

	public AnimationCurve DinamicSpeed; // curve of speed when player throw a ball
	public bool This_destroyBall = false; // true on object than can destroy a ball
	public bool This_fireBall = false; // true on object "fire"
	public bool This_DestroyColor = false; // true on arrow that can delete balls with one color
    public bool This_multiBall = false;
	private MainStart mainStart; // component that will use
	private float speedCount; // dynamic speed

	private int mode; // other int 
	private int dmode; // other int

	private bool once = false; // bool for OnTriggerEnter
	private BallControl timeBall; // auxiliary component
	private int StopIndex2; // other index

	void Start () {
		StartCoroutine ("WillActivate");
	}

	IEnumerator WillActivate(){ // start
		yield return new WaitForFixedUpdate ();
		GameObject.Find ("Main Camera").GetComponent<ControlOfPl> ().LastBall = this;
		GetComponent<SphereCollider> ().enabled = true;
		StartCoroutine ("WillMove");
	}

	IEnumerator WillMove(){ // move object
		speedCount += Time.fixedDeltaTime;
		transform.Translate (- DinamicSpeed.Evaluate(speedCount),0,0);
		yield return new WaitForFixedUpdate ();
		StartCoroutine ("WillMove");
	}


	void OnTriggerEnter(Collider other){
		if (!once && GameObject.Find ("Main Camera").GetComponent<ControlOfPl> ().Player_off == 0) { // if all right
			if (other.gameObject.GetComponent<BallControl> () != null && gameObject.name == "throw ball" && !This_destroyBall && !This_fireBall && !This_DestroyColor && other.gameObject.name != "ball under off") { // if it`s simple ball
				if (other.GetComponent<BallControl> ().c < other.GetComponent<BallControl> ().ccObject.SetOfPoints.Count - 7) { // if it`s not near finish 
					BallControl otherToScript = other.gameObject.GetComponent<BallControl> (); // get component from other ball
					mainStart = otherToScript.msScript;

					GetComponent<BallControl> ().ccObject = otherToScript.ccObject;
					GetComponent<BallControl> ().msScript = otherToScript.msScript;
                    
                    //тут идет проигрывание звука шарик ударяется
					if (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Ball_collision_with_line != null) {
						GameObject.Find ("SC 1").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Ball_collision_with_line);
					}

					timeBall = mainStart.TimeBall;
					once = true;
					Vector3 GetFirst = otherToScript.ccObject.GetNewVector (otherToScript, 0.001f, otherToScript.t - 1, otherToScript.c, true);
					Vector3 GetSecond = otherToScript.ccObject.GetNewVector (otherToScript, 0.001f, otherToScript.t + 1, otherToScript.c, true);
					// calculate a two distances, need for get info about future position of ball 
					float dist1 = Mathf.Sqrt ((transform.position.x - GetFirst.x) * (transform.position.x - GetFirst.x) + (transform.position.y - GetFirst.y) * (transform.position.y - GetFirst.y) + (transform.position.z - GetFirst.z) * (transform.position.z - GetFirst.z));
					float dist2 = Mathf.Sqrt ((transform.position.x - GetSecond.x) * (transform.position.x - GetSecond.x) + (transform.position.y - GetSecond.y) * (transform.position.y - GetSecond.y) + (transform.position.z - GetSecond.z) * (transform.position.z - GetSecond.z));
					float interval = mainStart.interval;

					if (dist1 > dist2) {
						// After this ball 
						mode = mainStart.GeneratedBalls.IndexOf (otherToScript) - 1;
						StopIndex2 = mainStart.FindStopIndex (mode, 1f, mainStart.BaseSpeed, 0);
						if (mainStart.BaseSpeed < 0) {
							StopIndex2 = mainStart.FindStopIndex (mode, 1f, -mainStart.BaseSpeed, 0);
						}

						if (mode >= 0) { 
							if ((interval * mainStart.GeneratedBalls [mode].c + interval * mainStart.GeneratedBalls [mode].t) - (interval * mainStart.GeneratedBalls [mode + 1].c + interval * mainStart.GeneratedBalls [mode + 1].t) > mainStart.distance * 0.99f &&
							    (interval * mainStart.GeneratedBalls [mode].c + interval * mainStart.GeneratedBalls [mode].t) - (interval * mainStart.GeneratedBalls [mode + 1].c + interval * mainStart.GeneratedBalls [mode + 1].t) < mainStart.distance * 3f) {
								StopIndex2 = mainStart.FindStopIndex (mode, 1f, mainStart.BaseSpeed, 0);
								if (mainStart.BaseSpeed < 0) {
									StopIndex2 = mainStart.FindStopIndex (mode, 1f, -mainStart.BaseSpeed, 0);
								}
								dmode = -1;
							}
						}
					} else {
						// Before this ball
						mode = mainStart.GeneratedBalls.IndexOf (otherToScript);
						StopIndex2 = mainStart.FindStopIndex (mode, 1f, mainStart.BaseSpeed, 0);
						if (mainStart.BaseSpeed < 0) {
							StopIndex2 = mainStart.FindStopIndex (mode, 1f, -mainStart.BaseSpeed, 0);
						}
						if (mode < mainStart.GeneratedBalls.Count - 2) {
							if ((interval * mainStart.GeneratedBalls [mode].c + interval * mainStart.GeneratedBalls [mode].t) - (interval * mainStart.GeneratedBalls [mode + 1].c + interval * mainStart.GeneratedBalls [mode + 1].t) > mainStart.distance * 1.01f) {
								dmode = 1;
								StopIndex2 = mainStart.FindStopIndex ((mode - 1), 1f, mainStart.BaseSpeed, 0);
								if (mainStart.BaseSpeed < 0) {
									StopIndex2 = mainStart.FindStopIndex ((mode - 1), 1f, -mainStart.BaseSpeed, 0);
								}
							}
						}
					}
					// start Coroutine for other work
					if (mainStart.BaseSpeed >= 0) {
						StartCoroutine (pGoInLine (other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder));
					}
					if (mainStart.BaseSpeed < 0) {
						StartCoroutine (nGoInLine (other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder));
					}
					mainStart.mode_IndexStop = mode;
				}
			} else if (other.gameObject.GetComponent<BallControl> () != null && gameObject.name == "throw ball" && This_destroyBall && other.gameObject.name != "ball under off") { // if it`s object that can destroy a ball
				once = true;
				if (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Bonus_Set_of_destroyBall != null) {
					GameObject.Find ("SC 7").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Bonus_Set_of_destroyBall);
				}
				BallControl otherToScript = other.gameObject.GetComponent<BallControl> ();
				GetComponent<BallControl> ().ccObject = otherToScript.ccObject;
				GetComponent<BallControl> ().msScript = otherToScript.msScript;
				mainStart = otherToScript.msScript;
				int Indexofball = mainStart.GeneratedBalls.IndexOf (otherToScript);
				mainStart.GeneratedBalls.Remove (otherToScript);
				mainStart.CreateDestroyBall (otherToScript.gameObject);

				Vector3 ResultPosition = otherToScript.gameObject.transform.position;
				mainStart.GetComponent<ControlOfPl> ().AddScoreGlobal (mainStart.GetComponent<ControlOfPl> ().Score_Bonuse, otherToScript.GetComponent<SpriteRenderer> ().color, ResultPosition);

				Destroy (otherToScript.gameObject);
				mainStart.IndexStop = mainStart.FindStopIndex ((mainStart.GeneratedBalls.Count - 2), 1f, mainStart.BaseSpeed, 0);
				mainStart.gameObject.GetComponent<ControlOfPl> ().SearchAvCollors ();
				if (Indexofball > 0 && Indexofball < mainStart.GeneratedBalls.Count - 1 && !mainStart.EndOfLine) {
					if (mainStart.GeneratedBalls [Indexofball].GetComponent<SpriteRenderer> ().color == mainStart.GeneratedBalls [Indexofball - 1].GetComponent<SpriteRenderer> ().color) {
						if (Indexofball > 1) {
							if (mainStart.GeneratedBalls [Indexofball - 1].GetComponent<SpriteRenderer> ().color == mainStart.GeneratedBalls [Indexofball - 2].GetComponent<SpriteRenderer> ().color) {
								mainStart.BaseSpeed = mainStart.saveBaseSpeed;
								mainStart.StartCoroutine (mainStart.CollisionInline (Indexofball - 1));
							}
						}
						if (Indexofball < mainStart.GeneratedBalls.Count - 2) {
							if (mainStart.GeneratedBalls [Indexofball].GetComponent<SpriteRenderer> ().color == mainStart.GeneratedBalls [Indexofball + 1].GetComponent<SpriteRenderer> ().color) {
								mainStart.BaseSpeed = mainStart.saveBaseSpeed;
								mainStart.StartCoroutine (mainStart.CollisionInline (Indexofball - 1));
							}
						}
					}
				}
				Destroy (gameObject);
			}
			// if`s fire
			if (other.gameObject.GetComponent<BallControl> () != null && gameObject.name == "throw ball" && This_fireBall) {
				BallControl otherToScript = other.gameObject.GetComponent<BallControl> ();
				GetComponent<BallControl> ().ccObject = otherToScript.ccObject;
				GetComponent<BallControl> ().msScript = otherToScript.msScript;
				mainStart = otherToScript.msScript;
				mainStart.GeneratedBalls.Remove (otherToScript);
				mainStart.CreateDestroyBall (otherToScript.gameObject);

				Vector3 ResultPosition = otherToScript.gameObject.transform.position;
				mainStart.GetComponent<ControlOfPl> ().AddScoreGlobal (mainStart.GetComponent<ControlOfPl> ().Score_Bonuse, otherToScript.GetComponent<SpriteRenderer> ().color, ResultPosition);

				Destroy (otherToScript.gameObject);
				mainStart.IndexStop = mainStart.FindStopIndex ((mainStart.GeneratedBalls.Count - 2), 1f, mainStart.BaseSpeed, 0);
				mainStart.gameObject.GetComponent<ControlOfPl> ().SearchAvCollors ();
			}
			// if it`s arrow
			if (other.gameObject.GetComponent<BallControl> () != null && gameObject.name == "throw ball" && This_DestroyColor && other.gameObject.name != "ball under off") {
				once = true;
				if (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Bonus_Destroy_color != null) {
					GameObject.Find ("SC 8").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter> ().Bonus_Destroy_color);
				}
				BallControl otherToScript = other.gameObject.GetComponent<BallControl> ();
				GetComponent<BallControl> ().ccObject = otherToScript.ccObject;
				GetComponent<BallControl> ().msScript = otherToScript.msScript;
				mainStart = otherToScript.msScript;
				Color Color_use = otherToScript.GetComponent<SpriteRenderer> ().color;
				int countGB = 0;
				List<BallControl> ListOfDel = new List<BallControl> { }; 
				while (countGB < mainStart.GeneratedBalls.Count) {
					if (mainStart.GeneratedBalls [countGB].GetComponent<SpriteRenderer> ().color == Color_use && mainStart.GeneratedBalls [countGB].gameObject.name == "ball") {
						ListOfDel.Add (mainStart.GeneratedBalls [countGB]);
					}
					countGB++;
				}

				while (ListOfDel.Count > 0) {
					mainStart.GeneratedBalls.Remove (ListOfDel [0]);
					mainStart.CreateDestroyBall (ListOfDel [0].gameObject);

					Vector3 ResultPosition = ListOfDel [0].gameObject.transform.position;
					mainStart.GetComponent<ControlOfPl> ().AddScoreGlobal (mainStart.GetComponent<ControlOfPl> ().Score_Bonuse, otherToScript.GetComponent<SpriteRenderer> ().color, ResultPosition);

					Destroy (ListOfDel [0].gameObject);
					ListOfDel.Remove (ListOfDel [0]);
				}

				mainStart.gameObject.GetComponent<ControlOfPl> ().SearchAvCollors ();
				mainStart.IndexStop = mainStart.FindStopIndex ((mainStart.GeneratedBalls.Count - 2), 1f, mainStart.BaseSpeed, 0);
				Destroy (gameObject);
			}

		}
	}
	// if speed >= 0
	IEnumerator pGoInLine(int sortOrder){
		StopCoroutine ("WillMove"); // no for fly
		float interval = mainStart.interval; // get interval
		float dBBalls = mainStart.distance; // get standard index
		Vector3 SavePos = transform.position; // position of new ball
		float currentDist = dBBalls / 15f; // get interval
		float one = 0;
		while (one < 1) {
			if (StopIndex2 != -1 && dmode != -1) {
				StopIndex2 = mainStart.FindStopIndex (StopIndex2, 1f, mainStart.BaseSpeed, 0);
			}
			if (dmode == -1) {
				
				if ((interval * mainStart.GeneratedBalls [mode].c + interval * mainStart.GeneratedBalls [mode].t) - (interval * mainStart.GeneratedBalls [mode + 1].c + interval * mainStart.GeneratedBalls [mode + 1].t) < mainStart.distance * 2.01f) {
					StopIndex2 = mainStart.FindStopIndex (mode, 2f, mainStart.BaseSpeed, 0);
					if (StopIndex2 == -1) {
						StopIndex2 = mainStart.FindStopIndex ((mode - 1), 1f, mainStart.BaseSpeed, 0);
					}
				} else {
					StopIndex2 = mainStart.FindStopIndex (mode, 1f, mainStart.BaseSpeed, 0);
				}
			}

			if(mode >= 0){
				mainStart.OnlyMove (mode, (currentDist/interval), StopIndex2, 0);
			}
			one += 0.07f;
			if (mode < mainStart.GeneratedBalls.Count - 1 && dmode != 1) {
				timeBall.c = mainStart.GeneratedBalls [mode + 1].c + (int)mainStart.ConvertTo.x;
				timeBall.t = mainStart.GeneratedBalls [mode + 1].t + mainStart.ConvertTo.y + mainStart.BaseSpeed;
				if (timeBall.t > 1) {
					timeBall.c++;
					timeBall.t -= 1;
				}
			} else {
				timeBall.c = mainStart.GeneratedBalls [mode].c - (int)mainStart.ConvertTo.x;
				timeBall.t = mainStart.GeneratedBalls [mode].t - mainStart.ConvertTo.y;
				if (timeBall.t < 0) {
					timeBall.c--;
					timeBall.t += 1;
				}
			}
			timeBall.LocalUpdate (0.001f);
			transform.position = new Vector3(Mathf.Lerp(SavePos.x,timeBall.transform.position.x,one), Mathf.Lerp(SavePos.y,timeBall.transform.position.y,one), Mathf.Lerp(SavePos.z,timeBall.transform.position.z,one));
			timeBall.LocalUpdate (2);
			transform.LookAt (timeBall.transform);
			if (transform.position.x < timeBall.transform.position.x) {
				transform.eulerAngles = new Vector3 (0, 0, 180 - transform.eulerAngles.x);
			} else {
				transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.x);
			}
			yield return new WaitForFixedUpdate ();
		}
		name = "ball";
		if (mode < mainStart.GeneratedBalls.Count - 1 && dmode != 1) {
			GetComponent<BallControl> ().c = mainStart.GeneratedBalls [mode + 1].c + (int)mainStart.ConvertTo.x;
			GetComponent<BallControl> ().t = mainStart.GeneratedBalls [mode + 1].t + mainStart.ConvertTo.y + mainStart.BaseSpeed;
			if (GetComponent<BallControl> ().t > 1) {
				GetComponent<BallControl> ().c++;
				GetComponent<BallControl> ().t -= 1;
			}
		} else {
			GetComponent<BallControl> ().c = mainStart.GeneratedBalls [mode].c - (int)mainStart.ConvertTo.x;
			if (dmode != 1) {
				GetComponent<BallControl> ().t = mainStart.GeneratedBalls [mode].t - mainStart.ConvertTo.y;
			} else {
				GetComponent<BallControl> ().t = mainStart.GeneratedBalls [mode].t - mainStart.ConvertTo.y - mainStart.BaseSpeed;
			}
			if (GetComponent<BallControl> ().t < 0) {
				GetComponent<BallControl> ().c--;
				GetComponent<BallControl> ().t += 1;
			}
		}
		mainStart.GeneratedBalls.Insert (mode + 1, GetComponent<BallControl>());
		GetComponent<BallControl> ().enabled = true;
		GetComponent<SpriteRenderer> ().sortingOrder = sortOrder + 1;
		tag = mainStart.BaseBall.tag;
		if (dmode != 1) {
			if (mode < mainStart.GeneratedBalls.Count - 2) {
				GetComponent<BallControl> ().LocalUpdate ((currentDist / interval));
			} else {
				GetComponent<BallControl> ().LocalUpdate (-(currentDist / interval));
			}
		} else {
			GetComponent<BallControl> ().LocalUpdate (-(currentDist / interval));
		}
		StopIndex2 = -1;
		mainStart.FindOfColors (mainStart.GeneratedBalls.IndexOf(GetComponent<BallControl> ()), false);
		GetComponent<BallControl> ().LocalUpdate (0.001f);
		mainStart.IndexStop = mainStart.FindStopIndex((mainStart.GeneratedBalls.Count - 2), 1f, mainStart.BaseSpeed, 0);
		mainStart.NormalizeLine (mode);
		mainStart.NormalizeLine ((mode + 1));
		mainStart.mode_IndexStop = -1;
		yield return new WaitForSeconds (1);
		Destroy(GetComponent<ThrowBall>());
	}
	// if speed < 0
	IEnumerator nGoInLine(int sortOrder){
		StopCoroutine ("WillMove"); // no for fly
		float interval = mainStart.interval; // get interval
		float dBBalls = mainStart.distance; // get standard index
		Vector3 SavePos = transform.position; // position of new ball
		float currentDist = dBBalls / 15f; // get interval
		float one = 0;
		while (one < 1) {

			if (StopIndex2 != -1 && dmode != -1) {
				StopIndex2 = mainStart.FindStopIndex (StopIndex2, 1f, -mainStart.BaseSpeed, 0);
			}
			if (dmode == -1) {

				if ((interval * mainStart.GeneratedBalls [mode].c + interval * mainStart.GeneratedBalls [mode].t) - (interval * mainStart.GeneratedBalls [mode + 1].c + interval * mainStart.GeneratedBalls [mode + 1].t) < mainStart.distance * 2.01f) {
					StopIndex2 = mainStart.FindStopIndex (mode, 2f, -mainStart.BaseSpeed, 0);
					if (StopIndex2 == -1) {
						StopIndex2 = mainStart.FindStopIndex ((mode - 1), 1f, -mainStart.BaseSpeed, 0);
					}
				} else {
					StopIndex2 = mainStart.FindStopIndex ((mode - 1), 1f, -mainStart.BaseSpeed, 0);
				}

			}
			if(mode >= 0){
				mainStart.OnlyMove (mode, (currentDist/interval), StopIndex2, 0);
			}
			one += 0.07f;
			if (mode < mainStart.GeneratedBalls.Count - 1 && dmode != 1) {
				timeBall.c = mainStart.GeneratedBalls [mode + 1].c + (int)mainStart.ConvertTo.x;
				timeBall.t = mainStart.GeneratedBalls [mode + 1].t + mainStart.ConvertTo.y + mainStart.BaseSpeed;
				if (timeBall.t > 1) {
					timeBall.c++;
					timeBall.t -= 1;
				}
			} else {
				timeBall.c = mainStart.GeneratedBalls [mode].c - (int)mainStart.ConvertTo.x;
				timeBall.t = mainStart.GeneratedBalls [mode].t - mainStart.ConvertTo.y;
				if (timeBall.t < 0) {
					timeBall.c--;
					timeBall.t += 1;
				}
			}
			timeBall.LocalUpdate (0.001f);
			transform.position = new Vector3(Mathf.Lerp(SavePos.x,timeBall.transform.position.x,one), Mathf.Lerp(SavePos.y,timeBall.transform.position.y,one), Mathf.Lerp(SavePos.z,timeBall.transform.position.z,one));
			timeBall.LocalUpdate (2);
			transform.LookAt (timeBall.transform);
			if (transform.position.x < timeBall.transform.position.x) {
				transform.eulerAngles = new Vector3 (0, 0, 180 - transform.eulerAngles.x);
			} else {
				transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.x);
			}
			yield return new WaitForFixedUpdate ();
		}
		name = "ball";
		if (mode < mainStart.GeneratedBalls.Count - 1 && dmode != 1) {
			GetComponent<BallControl> ().c = mainStart.GeneratedBalls [mode + 1].c + (int)mainStart.ConvertTo.x;
			GetComponent<BallControl> ().t = mainStart.GeneratedBalls [mode + 1].t + mainStart.ConvertTo.y + mainStart.BaseSpeed;
			if (GetComponent<BallControl> ().t > 1) {
				GetComponent<BallControl> ().c++;
				GetComponent<BallControl> ().t -= 1;
			}
		} else {
			GetComponent<BallControl> ().c = mainStart.GeneratedBalls [mode].c - (int)mainStart.ConvertTo.x;
			if (dmode != 1) {
				GetComponent<BallControl> ().t = mainStart.GeneratedBalls [mode].t - mainStart.ConvertTo.y;
			} else {
				GetComponent<BallControl> ().t = mainStart.GeneratedBalls [mode].t - mainStart.ConvertTo.y - mainStart.BaseSpeed;
			}
			if (GetComponent<BallControl> ().t < 0) {
				GetComponent<BallControl> ().c--;
				GetComponent<BallControl> ().t += 1;
			}
		}
		mainStart.GeneratedBalls.Insert (mode + 1, GetComponent<BallControl>());
		GetComponent<BallControl> ().enabled = true;
		GetComponent<SpriteRenderer> ().sortingOrder = sortOrder + 1;
		tag = mainStart.BaseBall.tag;
		if (dmode != 1) {
			if (mode < mainStart.GeneratedBalls.Count - 2) {
				GetComponent<BallControl> ().LocalUpdate ((currentDist / interval));
			} else {
				GetComponent<BallControl> ().LocalUpdate (-(currentDist / interval));
			}
		} else {
			GetComponent<BallControl> ().LocalUpdate (-(currentDist / interval));
		}
		StopIndex2 = -1;
		mainStart.FindOfColors (mainStart.GeneratedBalls.IndexOf(GetComponent<BallControl> ()), false);
		GetComponent<BallControl> ().LocalUpdate (0.001f);
		mainStart.IndexStop = mainStart.FindStopIndex(0, 1f, mainStart.BaseSpeed, 0);
		mainStart.NormalizeLine (mode);
		mainStart.NormalizeLine ((mode + 1));
		mainStart.mode_IndexStop = -1;
		yield return new WaitForSeconds (1);
		Destroy(GetComponent<ThrowBall>());
	}
}
