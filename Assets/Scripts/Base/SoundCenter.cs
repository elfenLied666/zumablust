using UnityEngine;
using System.Collections;

public class SoundCenter : MonoBehaviour {
	// sound centre // set a audio in ...
	public AudioClip Player_throw_ball; // SC 0
	public AudioClip Ball_collision_with_line; // SC 1
	public AudioClip Collision_in_line; // SC 2
	public AudioClip Delete_ball; // SC 3
	public AudioClip Start_of_game; // SC 4
	public AudioClip Lose; // SC 4
	public AudioClip Win; // SC 4
	public AudioClip Tap_to_buttons; // SC 5
	public AudioClip Get_bonus; // SC 6
	public AudioClip Bonus_fire_ball; // fire - ball 
	public AudioClip Bonus_Set_of_destroyBall; // CS 7
	public AudioClip Bonus_Destroy_color; // CS 8
	public AudioClip Bonus_change_speed; // CS 9
	public AudioClip Music; // this

	void Start(){
		if (Music != null) {
			GetComponent<AudioSource> ().clip = Music;
			GetComponent<AudioSource> ().Play ();
		}
	}


}
