using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

public class CreateAnimation : Editor {
	public static string GAME_OBJECT_NAME="gameObject";
	public static string ANIMATION_CLIP_PATH = "Assets/animation.anim";

	[MenuItem("Assets/animation")]
	public static void createAnimation(){
		GameObject go = GameObject.Find(GAME_OBJECT_NAME);
		if (go != null){
			AnimatorController animCtrl = AnimatorController.GetEffectiveAnimatorController(go.GetComponent<Animator>());
			Debug.Log(animCtrl);

			AssetDatabase.DeleteAsset(ANIMATION_CLIP_PATH);
			AssetDatabase.DeleteAsset(
				AssetDatabase.GetAssetPath( animCtrl)
				);
			GameObject.DestroyImmediate(go);
		}

		go = new GameObject(GAME_OBJECT_NAME);
		Animator animator = go.GetComponent<Animator>();
		if (animator == null)
			animator = go.AddComponent<Animator>();

		AnimationClip animationClip = new AnimationClip();


		AssetDatabase.CreateAsset(animationClip, ANIMATION_CLIP_PATH);

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
