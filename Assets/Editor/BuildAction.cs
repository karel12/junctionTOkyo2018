using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildAction
{
	private static string[] Scenes
	{
		get 
		{
			return new string[]{"Assets/main.unity"};
		}
	}
	

	public static void SyncMonoDevelopProjects()
	{
		EditorApplication.ExecuteMenuItem("Assets/Sync MonoDevelop Project");
	}


	public static void PerformBuildAndroid ()
	{
		UnityEditor.EditorPrefs.SetString ("AndroidSdkRoot", "/Users/buildserver/Library/Developer/Xamarin/android-sdk-mac_x86");

		Debug.Log ("Called build for Android");
		//UnityEditor.EditorUserBuildSettings.SwitchActiveBuildTarget (UnityEditor.BuildTarget.Android);
		UnityEditor.BuildPipeline.BuildPlayer (Scenes, "Builds/AndroidBuild.apk", UnityEditor.BuildTarget.Android, UnityEditor.BuildOptions.None);
	}

	public static void PerformBuildTouch ()
	{
		Debug.Log ("Called build for iOS");
		//UnityEditor.EditorUserBuildSettings.SwitchActiveBuildTarget (UnityEditor.BuildTarget.Android);
		UnityEditor.BuildPipeline.BuildPlayer (Scenes, "Builds/TouchXCodeProj", UnityEditor.BuildTarget.iOS, UnityEditor.BuildOptions.None);
	}
}
