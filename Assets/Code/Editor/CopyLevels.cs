using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CopyLevels
{
    private const string basePath = "Assets/_Game/Resources/";

    private const string OldLevelsPath = "GameLevels/";
    private const string NewLevelsPath = "GameLevels_1_4_5/";

    private const string LevelChangeFileName = "/LevelsChange.txt";
    
	[MenuItem ("TabTale/Levels/CopyLevels")]
    public static void CopyLevelAssetAsset()
    {
        FileInfo theSourceFile = null;
        StreamReader reader = null;
        string text = " "; // assigned to allow first line to be read below

        var path = Application.streamingAssetsPath + LevelChangeFileName;

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
           
            EditorUtility.DisplayProgressBar("Copy levels", message, count * 1.0f / lineCount);

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

        EditorUtility.DisplayDialog("Copy Levels", 
            totalCount + " Levels copied Succesfully in " + (int)(DateTime.Now - startTime).TotalSeconds + " seconds", 
            "Ok");
    }

    static bool CopyFile(string oldName, string newName)
    {
        var oldPath = basePath + OldLevelsPath + oldName + ".txt";
        var newPath = basePath + NewLevelsPath + newName + ".txt";

        var success = AssetDatabase.CopyAsset(oldPath, newPath);
        
        //Debug.Log(success + ", copied old: " + oldName + " to: " + newName);

        return success;
    }
}
