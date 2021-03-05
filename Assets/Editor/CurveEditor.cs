using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(CurveCreator))]
public class CurveEditor : Editor {

	private CurveCreator curveCreator; // script of editor 
	private Transform PointTr; // position of point that will move
	private Quaternion PointRot; // angle of point 
	private int iIndex = -1;

	public GUIStyle StyleEditor;
	// textures for editor
	public Texture EditCurve;
	public Texture ButtonAdd;
	public Texture ButtonDel;
	public Texture ButtonCntDel;
	public Texture LengthCurve;
	public Texture Calculate;
	public Texture Other;

	private float CalcInterval = 0.15f; // interval betweeen points 
	private bool showpath; // if need to show a points 

	public override void OnInspectorGUI(){ // for inspector
		DrawDefaultInspector ();
		EditorGUI.BeginChangeCheck();
		curveCreator = target as CurveCreator;
		GUI.skin.box = StyleEditor;
		GUI.skin.button = StyleEditor;
		EditorGUILayout.BeginVertical ();
		GUILayout.Box (EditCurve);
		EditorGUILayout.Space ();
		if (GUILayout.Button(ButtonAdd)) {
			Undo.RecordObject(curveCreator, "Add");
			curveCreator.AddToCurve();
			EditorUtility.SetDirty(curveCreator);
		}
		EditorGUILayout.Space ();
		if (curveCreator.EvoPointCount > 4) {
			if (GUILayout.Button (ButtonDel)) {
				Undo.RecordObject (curveCreator, "Delete");
				curveCreator.Delete ();
				EditorUtility.SetDirty (curveCreator);
			}
		} else {
			GUILayout.Box (ButtonCntDel);
		}
		GUILayout.Box (LengthCurve);
		EditorGUILayout.Space ();
		GUILayout.Box (" Intarval: " + CalcInterval.ToString());
		EditorGUILayout.Space ();
		if (GUILayout.Button(Calculate)) {
			Undo.RecordObject(curveCreator, "Calculate");
			curveCreator.CalculateLength (CalcInterval,PointTr);
			curveCreator.interval = CalcInterval;
		}
		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal ();
		showpath = GUILayout.Toggle (showpath, "Show path");
		float length = curveCreator.LengthOfBezier;
		GUILayout.FlexibleSpace ();
		EditorGUILayout.BeginVertical ();
		GUILayout.Label ("Length ~ " + length.ToString().Substring(0,length.ToString().Length - 3));
		GUILayout.Label ("Points : " + curveCreator.SetOfPoints.Count.ToString() );
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		if (iIndex >= 0 && iIndex < curveCreator.EvoPointCount) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Box (Other);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			OpenPointInspector ();
		}
		EditorGUILayout.EndVertical ();
	}


	private void OpenPointInspector() { // field for coordinates of point 
		EditorGUI.BeginChangeCheck();
		GUILayout.Label ("Position of point");
		Vector3 currPoint = EditorGUILayout.Vector3Field("", curveCreator.GetPoint(iIndex));
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(curveCreator, "Move Point");
			EditorUtility.SetDirty(curveCreator);
			curveCreator.SetPoint(iIndex, currPoint);
		}
	}


	public void OnSceneGUI(){ // for scene 
		curveCreator = target as CurveCreator;
		PointTr = curveCreator.transform;
		PointRot = Tools.pivotRotation == PivotRotation.Local ?
		PointTr.rotation : Quaternion.identity;
		Vector3 D0 = SetPoints (0);

			for (int i = 1; i < curveCreator.EvoPointCount; i += 3) {
				Vector3 D1 = SetPoints (i);
				Vector3 D2 = SetPoints (i + 1);
				Vector3 D3 = SetPoints (i + 2);
				Handles.color = Color.gray;
				Handles.DrawLine (D0, D1);
				Handles.DrawLine (D2, D3);
				Handles.DrawBezier (D0, D3, D1, D2, Color.green, null, 2f);
				D0 = D3;
			}

		Handles.Label (SetPoints(0), "Start");
		Handles.Label (SetPoints(curveCreator.EvoPointCount - 1), "End");


		if(GUI.changed){
			EditorUtility.SetDirty (target);
		}

		if (showpath) { // if need to show a points 
			if (curveCreator.SetOfPoints.Count >= 1) {
				for (int i = 1; i < curveCreator.SetOfPoints.Count; i++) {
					Handles.color = Color.red;
					Vector3 currOfPoint = curveCreator.SetOfPoints [i];
					float sizeCap = HandleUtility.GetHandleSize (currOfPoint);
					Handles.SphereCap (curveCreator.SetOfPoints.Count + i, currOfPoint, PointRot, sizeCap * 0.07f);
				}
			}
		}

	}

	private Vector3 SetPoints (int index) { // create points in scene 
		Vector3 currPoint = PointTr.TransformPoint(curveCreator.GetPoint(index));
		float sizeCap = HandleUtility.GetHandleSize(currPoint);
		if (index % 3 == 0) { // for main 
			Handles.color = Color.green;
			if (Handles.Button (currPoint, PointRot, sizeCap * 0.1f, sizeCap * 0.06f, Handles.SphereCap)) {
				iIndex = index;
				Repaint ();
			}
		} else { // for pivot
			Handles.color = Color.gray;
			if (Handles.Button (currPoint, PointRot, sizeCap * 0.04f, sizeCap * 0.06f, Handles.DotCap)) {
				iIndex = index;
				Repaint ();
			}
		}
		if (iIndex == index) {
				EditorGUI.BeginChangeCheck ();
				currPoint = Handles.PositionHandle (currPoint, PointRot);
				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (curveCreator, "Move Point");
					EditorUtility.SetDirty (curveCreator);
					curveCreator.SetPoint (index, PointTr.InverseTransformPoint (currPoint));
				}
		}
		return currPoint;
	}
}
