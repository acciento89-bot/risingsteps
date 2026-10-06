#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Kamilunavo.RisingSteps;

namespace Kamilunavo.RisingSteps.Editor
{
    [InitializeOnLoad]
    public static class ProjectBootstrap
    {
        private const string ScenePath="Assets/Scenes/Main.unity";
        static ProjectBootstrap()=>EditorApplication.delayCall+=InitializeProject;
        public static void InitializeProject(){if(EditorApplication.isPlayingOrWillChangePlaymode)return;PlayerSettings.companyName="Kamilunavo";PlayerSettings.productName="Rising Steps";PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,"com.kamilunavo.risingsteps");PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.kamilunavo.risingsteps");if(!File.Exists(ScenePath)){Directory.CreateDirectory("Assets/Scenes");var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("GameBootstrap").AddComponent<GameBootstrap>();EditorSceneManager.SaveScene(scene,ScenePath);}EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};}
    }
}

#endif
