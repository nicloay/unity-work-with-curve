﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using CurveExtended;

public class CreateAnimation : Editor {
	public static string GAME_OBJECT_NAME="gameObject";
	public static string ANIMATION_CLIP_PATH = "Assets/animation.anim";
	public static string CUBE1_NAME = "blnkCube";
	public static string CUBE2_NAME = "lineCube";
	public static string CUBE3_NAME = "smoothCube";
	public static Vector3 CUBE1_POSITION = Vector3.left*2;
	public static Vector3 CUBE2_POSITION = new Vector3(-1.0f, +2.0f, 0);

	public static float stepTime = 0.5f;
	public static float[] stepValues = new float[]{
		0,1,0,1,0
	};

	[MenuItem("Assets/animation")]
	public static void createAnimation(){
		GameObject go = GameObject.Find(GAME_OBJECT_NAME);
		if (go != null)
			removeGameObjectAndComponents(go);

		Animator animator;
		go = createGameObjects(out animator);
		AnimationClip animationClip = new AnimationClip();
		AnimationUtility.SetAnimationType(animationClip, ModelImporterAnimationType.Generic);


		AnimationCurve activeCurve = new AnimationCurve();
		AnimationCurve positionLineCurve = new AnimationCurve();
		AnimationCurve positionSmoothCurve = new AnimationCurve();
		
		
		for (int i = 0; i < stepValues.Length; i++) {
			float time = stepTime * i;
			activeCurve        .AddKey(KeyframeUtil.GetNew(time, stepValues[i]    , TangentMode.Stepped));
			positionLineCurve  .AddKey(KeyframeUtil.GetNew(time, stepValues[i] + 2, TangentMode.Linear));
			positionSmoothCurve.AddKey(KeyframeUtil.GetNew(time, stepValues[i] - 2, TangentMode.Smooth));			
		}

		//this will be linear curve, so need to update tangents (should be after keyframes assignments)
		positionLineCurve.UpdateAllLinearTangents();



		animationClip.SetCurve(CUBE1_NAME, typeof(GameObject),"m_IsActive", activeCurve);
		animationClip.SetCurve(CUBE2_NAME, typeof(Transform),"localPosition.x", positionLineCurve);
		animationClip.SetCurve(CUBE2_NAME, typeof(Transform),"localPosition.y", positionSmoothCurve);

		AssetDatabase.CreateAsset(animationClip, ANIMATION_CLIP_PATH);
		AssetDatabase.SaveAssets();
		AddClipToAnimatorComponent(go, animator, animationClip);
	}
	
	static GameObject createGameObjects(out Animator animator){
		GameObject go = new GameObject(GAME_OBJECT_NAME);

		createCubeAndAttachTo(CUBE1_NAME, CUBE1_POSITION, go);
		createCubeAndAttachTo(CUBE2_NAME, CUBE2_POSITION, go);

		animator =  go.GetComponent<Animator>();
		if (animator == null)
			animator = go.AddComponent<Animator>();
		return go;
	}

	static void createCubeAndAttachTo(string cubeName, Vector3 position, GameObject parentGO){
		GameObject go= GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.name= cubeName;
		go.transform.parent = parentGO.transform;
		go.transform.localPosition = position;
	}


	static void removeGameObjectAndComponents(GameObject go){
		AnimatorController animCtrl = AnimatorController.GetEffectiveAnimatorController(go.GetComponent<Animator>());
		AssetDatabase.DeleteAsset(ANIMATION_CLIP_PATH);
		AssetDatabase.DeleteAsset(
			AssetDatabase.GetAssetPath( animCtrl)
			);
		GameObject.DestroyImmediate(go);
	}


	public static AnimationClip AddClipToAnimatorComponent(GameObject go, Animator animator, AnimationClip newClip)
	{
	
		AnimatorController animatorController = AnimatorController.GetEffectiveAnimatorController(animator);
		if (animatorController == null)
		{
			AnimatorController controllerForClip = AnimatorController.CreateAnimatorControllerForClip(newClip, go);
			AnimatorController.SetAnimatorController(animator, controllerForClip);
			if (controllerForClip != null)
				return newClip;
			else
				return null;
		}
		else
		{
			AnimatorController.AddAnimationClipToController(animatorController, newClip);
			return newClip;
		}
	}

}
