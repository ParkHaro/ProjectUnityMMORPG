using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
    [MenuItem("Tools/Run Multiplayers/2 Players")]
    private static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }
    
    [MenuItem("Tools/Run Multiplayers/3 Players")]
    private static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }
    
    [MenuItem("Tools/Run Multiplayers/4 Players")]
    private static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }
    
    private static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for (int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                $"Builds/Win64/{GetProjectName()}{i}/{GetProjectName()}/{i}.exe",
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer
            );
        }
    }

    private static string GetProjectName()
    {
        var s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    private static string[] GetScenePaths()
    {
        var scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}