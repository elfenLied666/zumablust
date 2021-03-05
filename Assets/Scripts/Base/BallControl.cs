using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

	public CurveCreator ccObject; // object with points
	public MainStart msScript;
	//[HideInInspector]
	public float t; // from 0 to 1 (of distance between points)
	//[HideInInspector]
	public int c; // count of points 
	[Header("Other Settings")]
	public Sprite[] SetOfS_Balls; // for animation
	public float convertSpeed; // speed of animation 
	public float moved;
	public bool ItsBall = true; // if it`s simple ball
	private int spriteIndex;
    public bool isDestroyBall = true;


	public void LocalUpdate(float speed){
        if (speed != 0)
        {
            isDestroyBall = true;
            Vector3 new_position = ccObject.GetNewVector(this, speed, t, c, false); // get new position 
            Vector2 SaveInfo = new Vector2(c, t); // save info
            Vector3 double_position = new Vector3(0, 0, 0);
            // new position for get direction of ball
            if (speed > 0)
            {
                double_position = ccObject.GetNewVector(this, speed * 5, t, c, true);
            }
            else
            {
                double_position = ccObject.GetNewVector(this, -speed * 5, t, c, true);
            }
            c = (int)SaveInfo.x;
            t = SaveInfo.y;
            transform.LookAt(double_position);
            if (transform.position.y < double_position.y && transform.position.x <= double_position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, 180 - transform.eulerAngles.x);
            }
            else if (transform.position.y <= double_position.y && transform.position.x > double_position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.x);
            }
            else if (transform.position.y >= double_position.y && transform.position.x < double_position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, 180 - transform.eulerAngles.x);
            }
            else if (transform.position.y > double_position.y && transform.position.x >= double_position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.x);
            }
            transform.position = new_position; // set new position off ball
           
            ThrowBall tb = this.GetComponent<ThrowBall>();
           
            if (name == "ball" || name == "ball under off" || name == "throw ball off" || name == "ball off")
            {
                isDestroyBall = true;
            }
            
        }
        else
        {
            Debug.Log("");
        }
    }
	// animation off ball
	public void AnimationOfBall(float speed){
		moved += speed * convertSpeed;
		if (moved > 1) {
			while (moved >= 1) {
				if (spriteIndex < SetOfS_Balls.Length - 1) {
					spriteIndex++;
				} else {
					spriteIndex = 0;
				}
				moved--;
			}
		}
		if (moved < -1) {
			while (moved <= -1) {
				if (spriteIndex > 0) {
					spriteIndex--;
				} else {
					spriteIndex = SetOfS_Balls.Length - 1;
				}
				moved++;
			}
		}
		GetComponent<SpriteRenderer> ().sprite = SetOfS_Balls [spriteIndex];
	}

}