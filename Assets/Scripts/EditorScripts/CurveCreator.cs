using UnityEngine;
using System.Collections.Generic;

public class CurveCreator : MonoBehaviour
{

    [SerializeField]
   // [HideInInspector]
    public List<Vector3> EvoPoint; // set of points (in editor)
    [HideInInspector]
    public float LengthOfBezier = 2.786111f; // length of curve
    [SerializeField]
  //  [HideInInspector]
    public List<Vector3> SetOfPoints; // set of points (as curve) calculate

    [HideInInspector]
    public Vector3 newPos; // other vector

    [SerializeField]
 //   [HideInInspector]
    public float interval; // interval between points 
    [HideInInspector]
    public MainStart SendTo; // script that has it curve


    public Vector3 GetPoint(int index)
    { // return a position of point
        return EvoPoint[index];
    }

    public int EvoPointCount
    { // count of points
        get
        {
            return EvoPoint.Count;
        }
    }

    public void Delete()
    { // function of delete a one point from curve
        EvoPoint.RemoveAt(EvoPoint.Count - 1);
        EvoPoint.RemoveAt(EvoPoint.Count - 1);
        EvoPoint.RemoveAt(EvoPoint.Count - 1);
    }


    public void AddToCurve()
    { // function of add one point to curve 
        Vector3 LastPoint = EvoPoint[EvoPoint.Count - 1];
        Vector3 iLastPoint = EvoPoint[EvoPoint.Count - 2];
        Vector3 FirstDel = LastPoint + LastPoint - iLastPoint;
        EvoPoint.Add(FirstDel); // add grey point 
        FirstDel.x += 1f - 2 * (FirstDel.x - LastPoint.x);
        EvoPoint.Add(FirstDel); // add grey point 
        FirstDel = LastPoint;
        FirstDel.x += 1f;
        EvoPoint.Add(FirstDel); // add new green point 
    }


    public void SetPoint(int index, Vector3 currPoint)
    { // function of move point in editor
        if (index % 3 == 0)
        { // if it`s green point 
            Vector3 del = currPoint - EvoPoint[index];
            if (index > 0)
            {
                EvoPoint[index - 1] += del;
            }
            if (index + 1 < EvoPoint.Count)
            {
                EvoPoint[index + 1] += del;
            }
        }
        else
        { // if it`s grey point 
            int sIndex = 0;
            if ((index + 2 < EvoPoint.Count) && (index > 1))
            {
                if ((index + 1) % 3 == 0)
                {
                    sIndex = index + 2;
                }
                else
                {
                    sIndex = index - 2;
                }
                Vector3 del = currPoint - EvoPoint[index];
                EvoPoint[sIndex] -= del;
            }
        }
        EvoPoint[index] = currPoint;
    }

    public void CalculateLength(float interval, Transform PointTr)
    { // function of calculate a length and create array of points (with interval)
        LengthOfBezier = 0;
        for (int i = 0; i < EvoPointCount - 3; i += 3)
        {
            float del = 0;
            float LengthOfCurve = 0f;
            while (del < 1)
            {
                LengthOfCurve += (GetBezier(EvoPoint[i], EvoPoint[i + 1], EvoPoint[i + 2], EvoPoint[i + 3], del) - GetBezier(EvoPoint[i], EvoPoint[i + 1], EvoPoint[i + 2], EvoPoint[i + 3], del + 0.05f)).magnitude;
                del += 0.05f;
            }
            LengthOfBezier += LengthOfCurve;
        }
        if (interval >= 0.1f)
        {
            SetOfPoints = new List<Vector3> { };
            Vector3 LastPoint = EvoPoint[0];
            SetOfPoints.Add(PointTr.TransformPoint(LastPoint));
            for (int i = 0; i < EvoPointCount - 3; i += 3)
            {
                float t = 0f;
                Vector3 CurrentPoint = new Vector3(0, 0, 0);
                while (t < 1)
                {
                    t += 0.0001f;
                    CurrentPoint = GetBezier(EvoPoint[i], EvoPoint[i + 1], EvoPoint[i + 2], EvoPoint[i + 3], t);
                    float Length = Mathf.Sqrt((CurrentPoint.x - LastPoint.x) * (CurrentPoint.x - LastPoint.x) + (CurrentPoint.y - LastPoint.y) * (CurrentPoint.y - LastPoint.y) + (CurrentPoint.z - LastPoint.z) * (CurrentPoint.z - LastPoint.z));
                    if (Length > interval * 0.98f && Length < interval * 1.02f)
                    {
                        SetOfPoints.Add(PointTr.TransformPoint(CurrentPoint));
                        LastPoint = CurrentPoint;
                    }
                    else
                    {
                        if (Length > interval * 1.01f)
                        {
                            t -= 0.00021f;
                        }
                    }
                }
            }
        }
    }


    public Vector3 GetNewVector(BallControl fobject, float speed, float t, int c, bool for_direction)
    { // calculate a new positions for balls
      // fobject - script that on a ball
      // speed - speed for move
      // t - percentage between two points [0,1]
      // c - number of point in array of points (SetOfPoints)
      // for_direction - true if need only get a position
        Vector3 ireturn = fobject.gameObject.transform.position;
        t += speed;
        if (speed > 0)
        {
            if (t >= 1)
            {
                t -= 1;
                c++;
                while (t >= 1)
                {
                    t -= 1;
                    c++;
                }
                if (c < SetOfPoints.Count)
                {
                    fobject.c = c;
                }
            }

            if (c > SetOfPoints.Count - 2)
            { // if it`s and of line 
                if (!for_direction)
                {
                    if (SendTo.GetComponent<ControlOfPl>().Player_off == 0)
                    {
                        SendTo.GetComponent<ControlOfPl>().Player_off = 1;
                        SendTo.GetComponent<ControlOfPl>().CanThrow.z = 1;
                    }
                    else if (SendTo.GetComponent<ControlOfPl>().Player_off == -1)
                    {
                        int index = SendTo.GeneratedBalls.IndexOf(fobject);
                        Destroy(fobject.gameObject);
                        SendTo.GeneratedBalls.RemoveAt(index);
                    }
                }
            }
            if (c < SetOfPoints.Count - 1 && c >= 0)
            {
                ireturn = new Vector3(Mathf.Lerp(SetOfPoints[c].x, SetOfPoints[c + 1].x, t), Mathf.Lerp(SetOfPoints[c].y, SetOfPoints[c + 1].y, t), Mathf.Lerp(SetOfPoints[c].z, SetOfPoints[c + 1].z, t));
            }
            else
            {
                ireturn = new Vector3(-100, -100, -100);
            }
        }

        if (speed < 0)
        {
            if (t <= 0)
            {
                t += 1;
                c--;
                while (t <= 0)
                {
                    t += 1;
                    c--;
                }
                if (c > 0)
                {
                    if (!for_direction)
                    {
                        fobject.c = c;
                    }
                }
                else
                {
                    fobject.c = 0;
                }
            }
            if (c > 0)
            {
                ireturn = new Vector3(Mathf.Lerp(SetOfPoints[c].x, SetOfPoints[c + 1].x, t), Mathf.Lerp(SetOfPoints[c].y, SetOfPoints[c + 1].y, t), Mathf.Lerp(SetOfPoints[c].z, SetOfPoints[c + 1].z, t));
            }
            else
            {
                ireturn = new Vector3(-100, -100, -100);
            }
        }
        fobject.t = t;
        return ireturn;
    }


    public static Vector3 GetBezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    { // function of calculate for bezier
        float dec = 1f - t;
        return dec * dec * dec * P0 + 3f * dec * dec * t * P1 + 3f * dec * t * t * P2 + t * t * t * P3;
    }

}
