using UnityEngine;
using System.Collections;

public class BonuseCenter : MonoBehaviour {
	[Header("ACTIVE")]
	public bool InStart = true; // active bonuse function (when "Start")

	[Header("Other [<10]")]
	public int Frequency  = 10; // frequency of change (bonuses)

	[Header("Set of bonuses")]
	public bool Multi_ball = false; // active multi-ball
	public bool Slowly_move = false; // slow speed of balls
	public bool Stop_move = false; // speed of balls is 0
	public bool Back_move = false; // if line move back
	public bool Set_of_destroyBall = false; // object that can destroy any ball
	public bool Fire_ball = false; // delete all balls that touch to this object
	public bool Destroy_color = false; // arrow that can delete balls with one color 
    public bool Bomb_ball = false;
	//[HideInInspector]
	public int of_destroyBall = 0; // count of objects that can destroy any ball
	public Sprite[] SpritesOfBonuse; // sprites of bonuses // sort is important 
	public Sprite[] TextOfBonuse; // sprites of texts for bonuses // sort is important
	public GameObject BonusePack; // example of bonuse

	[HideInInspector]
	public GameObject CurrentBall; // ball that has a bonuse
	public SpriteRenderer ObjectBonuse; // component of bonuse
	private Transform TransformOfOB; // transform component of bonuse

	private bool stopCreate = false; // if need a stop of create bonuses
	private bool ActiveBonuse = false; // "true" if this script is on a bonuse object
	private int RandomBonuse = 0; // index of bonuse 

	[HideInInspector]
	public float SaveSpeed = 0; // save speed for other

	void Start(){
		Frequency = 10 - Frequency;
		TransformOfOB = ObjectBonuse.transform;
		if (InStart) { // if need a bonuses
			StartCoroutine ("AwaitForBonuse");
		}
	}

	IEnumerator AwaitForBonuse(){
		yield return new WaitForSeconds ((3 + 3 * Frequency)); // await (for create a bonuse)
		MainStart CurrentMS = GetComponent<ControlOfPl> ().SetOfLines[Random.Range(0,GetComponent<ControlOfPl> ().SetOfLines.Length)]; // Get a line when it will
		if (CurrentMS.Active) {
			if (!stopCreate && CurrentMS.GeneratedBalls.Count > 5) { // if balls in line > 5
				CurrentBall = CurrentMS.GeneratedBalls [Random.Range (0, CurrentMS.GeneratedBalls.Count)].gameObject;
				RandomBonuse = Random.Range (0, 8);
                // Сгенерируем бонусы самостоятельно
              //  RandomBonuse = 6;
				ObjectBonuse.sprite = SpritesOfBonuse [RandomBonuse];
				ObjectBonuse.color = CurrentBall.GetComponent<SpriteRenderer> ().color;
				ObjectBonuse.sortingOrder = CurrentBall.GetComponent<SpriteRenderer> ().sortingOrder + 1;
				ObjectBonuse.gameObject.SetActive (true);
				ObjectBonuse.color = new Color (ObjectBonuse.color.r, ObjectBonuse.color.g, ObjectBonuse.color.b, 0);
				ActiveBonuse = true;
			}
		} else {
			ActiveBonuse = false;
			StartCoroutine ("AwaitForBonuse");
		}
		yield return new WaitForSeconds ((5 + Frequency));
		ActiveBonuse = false;
		StartCoroutine ("AwaitForBonuse");
	}
		

	void Update(){
		if (Slowly_move || Stop_move || Back_move) { // if it work with speed
			StopCoroutine ("WorkWithSpeed");
			ResetSpeed ();
			StartCoroutine ("WorkWithSpeed");
		}
		if (Set_of_destroyBall) { // if it Set_of_destroyBall
			Set_of_destroyBall = false;
			of_destroyBall = 5;
		}

		if (ActiveBonuse) {
			if (CurrentBall != null) { // ball when bonuse
				if (ObjectBonuse.color.a < 1) {
					ObjectBonuse.color = new Color (ObjectBonuse.color.r,ObjectBonuse.color.g,ObjectBonuse.color.b,ObjectBonuse.color.a + 0.1f);				
				}
				TransformOfOB.position = CurrentBall.transform.position;
			} else { // if player destroy a ball where this bonuse
				ActiveBonuse = false;

				if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Get_bonus != null){
					GameObject.Find ("SC 6").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Get_bonus);
				}

				Multi_ball = false;
				Slowly_move = false;
				Stop_move = false;
				Back_move = false;
				Set_of_destroyBall = false;
				Fire_ball = false;
				Destroy_color = false;
				of_destroyBall = 0;


				if (RandomBonuse == 0) {
					Multi_ball = true;
				}
				if (RandomBonuse == 1) {
					Slowly_move = true;
				}
				if (RandomBonuse == 2) {
					Stop_move = true;
				}
				if (RandomBonuse == 3) {
					Back_move = true;
				}
				if (RandomBonuse == 4) {
					Set_of_destroyBall = true;
				}
				if (RandomBonuse == 5) {
					Fire_ball = true;
				}
				if (RandomBonuse == 6) {
					Destroy_color = true;
				}

				GameObject NewBP = Instantiate(BonusePack);
				NewBP.GetComponent<ScoreInBall>().ThisChild.sprite = TextOfBonuse[RandomBonuse];
				NewBP.transform.position = ObjectBonuse.transform.position;
				NewBP.SetActive (true);
				NewBP.GetComponent<ScoreInBall>().ThisChild.GetComponent<Animation> ().Play ("ScoreInBall");

				StopCoroutine ("AwaitForBonuse");
				StartCoroutine ("AwaitForBonuse");
			}
		} else {
			if (ObjectBonuse.color.a > 0) {
				ObjectBonuse.color = new Color (ObjectBonuse.color.r,ObjectBonuse.color.g,ObjectBonuse.color.b,ObjectBonuse.color.a - 0.1f);
			} else {
				ObjectBonuse.gameObject.SetActive (false);
			}
		}
	}


	IEnumerator WorkWithSpeed(){
		if (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Bonus_change_speed != null){
			GameObject.Find ("SC 9").GetComponent<AudioSource> ().PlayOneShot (GameObject.Find ("Sound Center").GetComponent<SoundCenter>().Bonus_change_speed);
		}
		MainStart[] SetOfLines = GetComponent<ControlOfPl> ().SetOfLines;
		int inMS = 0;
		if (Slowly_move) {
			while (inMS < SetOfLines.Length){
				SetOfLines [inMS].BaseSpeed = SetOfLines [inMS].saveBaseSpeed / 2;
				inMS++;
			}
			Slowly_move = false;
		}
		inMS = 0;
		if (Stop_move) {
			while (inMS < SetOfLines.Length){
				SetOfLines [inMS].BaseSpeed = 0;
				inMS++;
			}
			Stop_move = false;
		}
		inMS = 0;
		if (Back_move) {
			while (inMS < SetOfLines.Length){
				SetOfLines [inMS].BaseSpeed = -SetOfLines [inMS].saveBaseSpeed;
				SetOfLines [inMS].IndexStop = SetOfLines [inMS].FindStopIndex (0, 1f, SetOfLines [inMS].BaseSpeed, 0);
				inMS++;
			}
			Back_move = false;
		}
		SaveSpeed = SetOfLines [0].BaseSpeed;
		yield return new WaitForSeconds (7);
		ResetSpeed ();
	}

	void ResetSpeed(){
		MainStart[] SetOfLines = GetComponent<ControlOfPl> ().SetOfLines;
		int inMS = 0;
		while (inMS < SetOfLines.Length){
			SetOfLines [inMS].BaseSpeed = SetOfLines [inMS].saveBaseSpeed;
			SetOfLines [inMS].IndexStop = SetOfLines [inMS].FindStopIndex (SetOfLines [inMS].GeneratedBalls.Count - 2, 1f, SetOfLines [inMS].BaseSpeed, 0);
			inMS++;
		}
		SaveSpeed = 0;
	}

}
