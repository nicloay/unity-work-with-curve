using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

public class CreateAnimation : Editor {
	public static string GAME_OBJECT_NAME="gameObject";
	public static string ANIMATION_CLIP_PATH = "Assets/animation.anim";
	public static Keyframe[]  keyframes = new Keyframe[]{
		new Keyframe(0.0f,  0),
		new Keyframe(0.1f,  1),
		new Keyframe(0.2f,  0),
		new Keyframe(0.3f,  1),
		new Keyframe(0.4f,  0)
	};


	[MenuItem("Assets/animation")]
	public static void createAnimation(){
		GameObject go = GameObject.Find(GAME_OBJECT_NAME);
		if (go != null){
			AnimatorController animCtrl = AnimatorController.GetEffectiveAnimatorController(go.GetComponent<Animator>());
			AssetDatabase.DeleteAsset(ANIMATION_CLIP_PATH);
			AssetDatabase.DeleteAsset(
				AssetDatabase.GetAssetPath( animCtrl)
				);
			GameObject.DestroyImmediate(go);
		}

		go = new GameObject(GAME_OBJECT_NAME);
		GameObject go2 = new GameObject(GAME_OBJECT_NAME);
		go2.transform.parent = go.transform;
		Animator animator = go.GetComponent<Animator>();
		if (animator == null)
			animator = go.AddComponent<Animator>();

		AnimationClip animationClip = new AnimationClip();
		AnimationUtility.SetAnimationType(animationClip, ModelImporterAnimationType.Generic);


		AnimationCurve curve = new AnimationCurve(keyframes);
		curve.setTangentModeConstant();
		animationClip.SetCurve(GAME_OBJECT_NAME, typeof(GameObject),"m_IsActive", curve);

		EditorWindow.GetWindow( Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow")).SendEvent(EditorGUIUtility.CommandEvent("CurveChanged"));

		AssetDatabase.CreateAsset(animationClip, ANIMATION_CLIP_PATH);
		AssetDatabase.SaveAssets();
		AddClipToAnimatorComponent(go, animator, animationClip);
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
