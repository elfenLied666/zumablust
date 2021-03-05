using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallChangeTrigger : MonoBehaviour
{
    public GameObject Plate;
    public bool IsBallChange;
   // public GameObject NextBall;
    public GameObject BorderForPlate;

    // Use this for initialization
    void Start()
    {
        IsBallChange = false;
    }

    void OnMouseDown()
    {
        IsBallChange = true;
    }

}