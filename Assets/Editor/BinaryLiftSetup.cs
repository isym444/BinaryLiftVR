using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using UnityEditor.XR.Management;
using UnityEditor.XR.OpenXR.Features;
using UnityEditor.XR.Management.Metadata;

namespace BinaryLift.EditorTools
{
    /// <summary>
    /// One-click project configuration for Quest. Menu: BinaryLift > Configure Project.
    /// Batch mode: -executeMethod BinaryLift.EditorTools.BinaryLiftSetup.Configure
    /// Also enables OpenXR + Meta Quest Support for Android.
    /// </summary>
    public static class BinaryLiftSetup
    {
        const string ScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("BinaryLift/Configure Project")]
        public static void Configure()
        {
            EnsureScene();
            ConfigurePlayerSettings();
            IncludeShaders("Standard", "Sprites/Default");
            EnableOpenXR();
            AssetDatabase.SaveAssets();
            Debug.Log("BinaryLift: project configured.");
        }

        /// <summary>Batch-mode APK build: Unity -batchmode -buildTarget Android -executeMethod ...BuildAndroid</summary>
        public static void BuildAndroid()
        {
            Configure();
            Directory.CreateDirectory("Builds");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = "Builds/BinaryLift.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None,
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                EditorApplication.Exit(1);
        }

        /// <summary>Turns on OpenXR + Meta Quest Support + Touch controller profile for Android.</summary>
        static void EnableOpenXR()
        {
            const BuildTargetGroup android = BuildTargetGroup.Android;

            if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget container))
            {
                Directory.CreateDirectory("Assets/XR");
                container = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(container, "Assets/XR/XRGeneralSettingsPerBuildTarget.asset");
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, container, true);
            }
            if (!container.HasSettingsForBuildTarget(android)) container.CreateDefaultSettingsForBuildTarget(android);
            if (!container.HasManagerSettingsForBuildTarget(android)) container.CreateDefaultManagerSettingsForBuildTarget(android);

            XRGeneralSettings general = container.SettingsForBuildTarget(android);
            general.InitManagerOnStart = true;
            XRPackageMetadataStore.AssignLoader(container.ManagerSettingsForBuildTarget(android),
                typeof(OpenXRLoader).FullName, android);

            foreach (string id in new[] { MetaQuestFeature.featureId, OculusTouchControllerProfile.featureId })
            {
                OpenXRFeature feature = FeatureHelpers.GetFeatureWithIdForBuildTarget(android, id);
                if (feature == null) { Debug.LogWarning("BinaryLift: OpenXR feature not found: " + id); continue; }
                feature.enabled = true;
                EditorUtility.SetDirty(feature);
            }
            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
        }

        static void EnsureScene()
        {
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory("Assets/Scenes");
                // The game builds itself at runtime, so the scene is intentionally empty.
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        static void ConfigurePlayerSettings()
        {
            PlayerSettings.companyName = "BinaryLift";
            PlayerSettings.productName = "Binary Lift VR";
            PlayerSettings.colorSpace = ColorSpace.Linear;

            var android = NamedBuildTarget.Android;
            PlayerSettings.SetApplicationIdentifier(android, "com.binarylift.vr");
            PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
        }

        // The game creates its materials at runtime with Shader.Find, so the shaders must survive build stripping.
        static void IncludeShaders(params string[] names)
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0];
            var so = new SerializedObject(asset);
            var list = so.FindProperty("m_AlwaysIncludedShaders");

            foreach (string name in names)
            {
                Shader shader = Shader.Find(name);
                if (shader == null) continue;

                bool present = false;
                for (int i = 0; i < list.arraySize; i++)
                    if (list.GetArrayElementAtIndex(i).objectReferenceValue == shader) { present = true; break; }
                if (present) continue;

                list.InsertArrayElementAtIndex(list.arraySize);
                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = shader;
            }
            so.ApplyModifiedProperties();
        }
    }
}
