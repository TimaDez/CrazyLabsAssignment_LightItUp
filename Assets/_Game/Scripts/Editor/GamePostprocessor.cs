using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class GamePostprocessor : AssetPostprocessor
{
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
		switch (buildTarget) {
		case BuildTarget.iOS:
			XcodePostprocessBuild (path);
			break;
		}
	}

	private static void XcodePostprocessBuild (string path)
	{
#if UNITY_IOS
		var projPath = FindProjectPath (path);
		if (string.IsNullOrEmpty (projPath)) {
			Debug.LogWarning ("Error: missing xcodeproj file");
			return;
		}

		var proj = new PBXProject ();
		proj.ReadFromFile (projPath);
		var target = proj.TargetGuidByName ("Unity-iPhone");

		// bitcode flase
		proj.SetBuildProperty (target, "ENABLE_BITCODE", "NO");

		proj.WriteToFile (projPath);
#endif
	}

	private static string FindProjectPath (string filePath)
	{
		var projPath = string.Empty;
		if (filePath.EndsWith (".xcodeproj")) {
			projPath = filePath;
		} else {
			var projects = Directory.GetDirectories (filePath, "*.xcodeproj");
			if (projects.Length > 0) {
				projPath = projects [0];
			}
		}
		if (!string.IsNullOrEmpty (projPath)) {
			projPath += "/project.pbxproj";
		}
		return projPath;
	}
}
