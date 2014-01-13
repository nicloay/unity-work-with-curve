using UnityEngine;
using System;
using System.Collections;
using System.Reflection;


//  http://answers.unity3d.com/questions/313276/undocumented-property-keyframetangentmode.html
/// <summary>
/// Static utility class to work around lack of support for Keyframe.tangentMode
/// This utility class mimics the functionality that happens behind the scenes in UnityEditor when you manipulate an AnimationCurve. All of this information
/// was discovered via .net reflection, and thus relies on reflection to work
/// --testure 09/05/2012
/// </summary>
using UnityEditor;


public enum TangentMode
{
	Editable = 0,
	Smooth = 1,
	Linear = 2,
	Stepped = Linear | Smooth,
}

public static class AnimationCurveExtension : System.Object
{
	
	public enum TangentDirection
	{
		Left,
		Right
	}
	
	public static void setTangentModeConstant(this AnimationCurve curve){
		Type t = typeof( UnityEngine.Keyframe );
		FieldInfo field = t.GetField( "m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
		
		for ( int i = 0; i < curve.length; i++ )
		{
			Keyframe boxed = curve.keys[ i ]; // getting around the fact that Keyframe is a struct by pre-boxing
			field.SetValue( boxed, 31 );
			field.SetValue( boxed, 31 );
			boxed.inTangent = 0;
			boxed.outTangent = float.PositiveInfinity;
			curve.MoveKey( i, ( Keyframe ) boxed );
			//curve.SmoothTangents( i, 0f );

		}
	}
	
	public static void UpdateTangentsFromMode(this AnimationCurve curve){
		
		Type t = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.CurveUtility");		
		MethodInfo method = t.GetMethod("UpdateTangentsFromModeSurrounding", new Type[]{typeof(AnimationCurve),typeof(int)});
		
		for (int i = 0; i < curve.keys.Length; i++) {
			method.Invoke(null,new System.Object[]{curve, i});			
		}
	}
	
	public static void SetTangent( this AnimationCurve curve, TangentMode tangentMode )
	{
		Type t = typeof( UnityEngine.Keyframe );
		FieldInfo field = t.GetField( "m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
		
		for ( int i = 0; i < curve.length; i++ )
		{
			object boxed = curve.keys[ i ]; // getting around the fact that Keyframe is a struct by pre-boxing
			field.SetValue( boxed, GetNewTangentKeyMode( ( int ) field.GetValue( boxed ), TangentDirection.Left, tangentMode) );
			field.SetValue( boxed, GetNewTangentKeyMode( ( int ) field.GetValue( boxed ), TangentDirection.Right, tangentMode) );
			curve.MoveKey( i, ( Keyframe ) boxed );
			curve.SmoothTangents( i, 0f );
		}
	}
	
	public static int GetNewTangentKeyMode( int currentTangentMode, TangentDirection leftRight, TangentMode mode )
	{
		int output = currentTangentMode;
		
		if ( leftRight == TangentDirection.Left )
		{
			output &= -7;
			output |= ( ( int ) mode ) << 1;
		}
		else
		{
			output &= -25;
			output |= ( ( int ) mode ) << 3;
		}
		return output;
	}
	
}