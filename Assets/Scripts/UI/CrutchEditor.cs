using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrutchEditor : MonoBehaviour {

   public GameObject inGame;
    public bool isTWo = false;
	

    public void Calculate()
    {
       InGame inGame2 = inGame.GetComponent<InGame>();
        int x = 1;
        if (isTWo) x = 2;
        inGame2.calculate(x);
    }
}
