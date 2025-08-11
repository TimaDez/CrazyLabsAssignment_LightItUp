using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CopyLevelsEditor : EditorWindow
{
	private const string basePath = "Assets/_Game/Resources/";

	private const string OldLevelsPath = "GameLevels/";
	private const string NewLevelsPath = "GameLevels_1_2_0/";

	private const string LevelChangeFileName = "/LevelsChange.txt";
	
	[MenuItem ("TabTale/Levels/Copy Levels")]
    public static void CopyLevelAssetAsset()
    {
        FileInfo theSourceFile = null;
        StreamReader reader = null;
        string text = " "; // assigned to allow first line to be read below

        //var path = Application.streamingAssetsPath + LevelChangeFileName;
        
        string path = EditorUtility.OpenFilePanel("Levels File", "", "txt");
        if (path.Length == 0)
        {
	       Debug.LogError("No file is selected");
	       return;
        }
        
        var startTime = DateTime.Now;
        
        EditorUtility.DisplayProgressBar("Copy levels", "Preparing", 0);
        
        string[] lines = File.ReadAllLines(path);

        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("can't find lines");
            return;
        }

        var list = lines.ToList();

        int lineCount = list.Count;
        
        int count = 0;
        int totalCount = 0;

        bool cancelOperation = false;
        
        foreach (var line in list)
        {
            var arr = line.Split(',');
            
            count++;

            if (arr.Length != 2)
            {
                Debug.LogError("wrong number of levels in array for line: " + line);
                continue;
            }

            var oldName = arr[0].Trim();
            var newName = arr[1].Trim();

            if (!oldName.StartsWith("Level"))
            {
                int oldIndex, newIndex;
                if (int.TryParse(oldName, out oldIndex) && int.TryParse(newName, out newIndex))
                {
                    oldName = "Level_" + oldIndex.ToString("000");
                    newName = "Level_" + newIndex.ToString("000");
                }
                else
                {
                    Debug.LogError("can't parse line: " + line);
                    continue;
                }
            }
            
            var message = string.Format("copying {0}{1} to {2}{3}", OldLevelsPath, oldName, NewLevelsPath, newName);
           
            bool cancel = EditorUtility.DisplayCancelableProgressBar("Copy levels", message, count * 1.0f / lineCount);

            if (cancel)
            {
	            cancelOperation = true;
	            break;
            }
	          
            if (CopyFile(oldName, newName))
            {
                totalCount++;
            }
            else
            {
                Debug.LogError(message + " failed");
            }
        }           
        
        EditorUtility.ClearProgressBar();

        if (cancelOperation)
        {
	        EditorUtility.DisplayDialog("Copy Levels", "Operation canceled by user", "Ok");
        }
        else
        {
	        EditorUtility.DisplayDialog("Copy Levels", 
		        totalCount + " Levels copied Succesfully in " + (int)(DateTime.Now - startTime).TotalSeconds + " seconds", 
		        "Ok");
        }        
    }

    static bool CopyFile(string oldName, string newName)
    {
        var oldPath = basePath + OldLevelsPath + oldName + ".txt";
        var newPath = basePath + NewLevelsPath + newName + ".txt";

        var success = AssetDatabase.CopyAsset(oldPath, newPath);
        
        //Debug.Log(success + ", copied old: " + oldName + " to: " + newName);

        return success;
    }
	
	
	
	[MenuItem("Example/Overwrite Texture")]
	static void Apply()
	{
		Texture2D texture = Selection.activeObject as Texture2D;
		if (texture == null)
		{
			EditorUtility.DisplayDialog("Select Texture", "You must select a texture first!", "OK");
			return;
		}

		string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
		if (path.Length != 0)
		{
			var fileContent = File.ReadAllBytes(path);
			texture.LoadImage(fileContent);
		}
	}
}