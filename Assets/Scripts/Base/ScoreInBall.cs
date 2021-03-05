using UnityEngine;
using System.Collections;

public class ScoreInBall : MonoBehaviour {
	// this script for score that fly after destroy(ball)
	private SpriteRenderer GetThis;
	public SpriteRenderer ThisChild;
	private float SpeedX = 0; // random speed

	void Start(){
		SpeedX = Random.Range (-0.05f, 0.05f);
		GetThis = ThisChild.GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate(){
		if (GetThis.color.a > 0) {
			GetThis.color = new Color (GetThis.color.r, GetThis.color.g, GetThis.color.b, GetThis.color.a - 0.02f);
		} else {
			Destroy (gameObject);
		}
		transform.position = new Vector3 (transform.position.x + SpeedX, transform.position.y, transform.position.z);
	}
}
