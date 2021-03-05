using UnityEngine;
using System.Collections;

public class ForDestroyBall : MonoBehaviour {

	[HideInInspector]
	public GameObject DestroyedObject; // ball that destroy
	public Sprite[] ListOfDestroy; // spritelist of destroy ball
	public Sprite[] ListOfDestroyMB; // spritelist of destroy multi-ball

	void Start () {
		if (DestroyedObject != null) {
			StartCoroutine ("DestroyPlay");
		}
	}

	IEnumerator DestroyPlay(){
		GetComponent<SpriteRenderer> ().color = DestroyedObject.GetComponent<SpriteRenderer> ().color;
		transform.position = DestroyedObject.transform.position;

		int int_PlayList = 0;

		if (GetComponent<SpriteRenderer> ().color != new Color (1, 1, 1, 1)) { // if color - white, it mean that`s multi-ball 
			while (int_PlayList < ListOfDestroy.Length) {
				GetComponent<SpriteRenderer> ().sprite = ListOfDestroy [int_PlayList];
				int_PlayList++;
				transform.localScale = new Vector3 (transform.localScale.x + 0.005f, transform.localScale.y + 0.005f, transform.localScale.z + 0.005f);
				yield return new WaitForFixedUpdate ();
			}
		} else {
			while (int_PlayList < ListOfDestroyMB.Length) {
				GetComponent<SpriteRenderer> ().sprite = ListOfDestroyMB [int_PlayList];
				int_PlayList++;
				transform.localScale = new Vector3 (transform.localScale.x + 0.005f, transform.localScale.y + 0.005f, transform.localScale.z + 0.005f);
				yield return new WaitForFixedUpdate ();
			}
		}
		yield return new WaitForSeconds (2);
		Destroy (gameObject);
	}

}
