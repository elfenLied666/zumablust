using UnityEngine;
using System.Collections;

public class HideCollision : MonoBehaviour {

	public bool DestroyThen_In = false; // true if need to destroy when enter
	// need for bridge (cross of two lines)
	public CurveCreator UnderLine; // under
	public CurveCreator AboveLine; // above
	// if name != "ball", than balls (that fly) -> no collision 
	void OnTriggerStay(Collider other){
		if (AboveLine == null && UnderLine == null) { // if it`s not a bridge 
			if (other.gameObject.GetComponent<BallControl> ().ItsBall && other.gameObject.name == "throw ball" && !other.gameObject.GetComponent<BallControl> ().enabled) {
				other.gameObject.name = "throw ball off";
			}
			if (other.gameObject.GetComponent<BallControl> ().ItsBall && other.gameObject.name == "ball" && other.gameObject.GetComponent<BallControl> ().enabled) {
				other.gameObject.name = "ball off";
			}
			if (DestroyThen_In) {
				Destroy (other.gameObject); 
			}
		} else {
			if (other.gameObject.GetComponent<BallControl> ().ItsBall && other.gameObject.name == "ball"){
				if (other.gameObject.GetComponent<BallControl> ().ccObject == UnderLine) {
					other.gameObject.name = "ball under off";
				} else if (other.gameObject.GetComponent<BallControl> ().ccObject == AboveLine) {
					if (other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder < 300) {
						other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder += 300;
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (AboveLine == null && UnderLine == null) { // if it`s not a bridge 
			if (other.gameObject.GetComponent<BallControl> ().ItsBall && other.gameObject.name == "throw ball off") {
				other.gameObject.name = "throw ball";
			}
			if (other.gameObject.GetComponent<BallControl> ().ItsBall && other.gameObject.name == "ball off") {
				other.gameObject.name = "ball";
			}
		} else {
			if (other.gameObject.name == "ball under off"){
				other.gameObject.name = "ball";
			}
			if (other.gameObject.name == "ball") {
				if (other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder > 300) {
					other.gameObject.GetComponent<SpriteRenderer> ().sortingOrder -= 300;
				}
			}
		}
	}
}
