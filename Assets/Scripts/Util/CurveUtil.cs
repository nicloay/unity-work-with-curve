using UnityEngine;
using System.Collections;

public class CurveUtil {

	public static void UpdateAllLinearTangents(AnimationCurve curve){
		for (int i = 0; i < curve.keys.Length; i++) {
			UpdateTangentsFromMode(curve, i);
		}
	}

	// UnityEditor.CurveUtility.cs (c) Unity Technologies
	public static void UpdateTangentsFromMode(AnimationCurve curve, int index)
	{
		if (index < 0 || index >= curve.length)
			return;
		Keyframe key = curve[index];
		if (KeyframeUtil.GetKeyTangentMode(key, 0) == TangentMode.Linear && index >= 1)
		{
			key.inTangent = CalculateLinearTangent(curve, index, index - 1);
			curve.MoveKey(index, key);
		}
		if (KeyframeUtil.GetKeyTangentMode(key, 1) == TangentMode.Linear && index + 1 < curve.length)
		{
			key.outTangent = CalculateLinearTangent(curve, index, index + 1);
			curve.MoveKey(index, key);
		}
		if (KeyframeUtil.GetKeyTangentMode(key, 0) != TangentMode.Smooth && KeyframeUtil.GetKeyTangentMode(key, 1) != TangentMode.Smooth)
			return;
		curve.SmoothTangents(index, 0.0f);
	}
	
	// UnityEditor.CurveUtility.cs (c) Unity Technologies
	private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
	{
		return (float) (((double) curve[index].value - (double) curve[toIndex].value) / ((double) curve[index].time - (double) curve[toIndex].time));
	}

}
