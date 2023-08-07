using XyzAssets.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XyzAssets.Editor;
using UnityEditor;
namespace XyzAssets.Editor
{

    [CreateAssetMenu(fileName = "AssetBundlePackageEditorSetting", menuName = "Create AssetBundlePackageEditorSetting")]
    public class AssetBundlePackageEditorSetting : ScriptableObject
    {
        [System.Serializable]
        public class PackageBundles
        {
            public string ModeName;
            public bool CopyToStreamingAssetsPath;

            public AssetBundleDatas[] Bundles;
        }


        [System.Serializable]
        public struct AssetBundleDatas
        {
            public string BundleName;
            public string VariantName;
            [UnityObjectSelector]
            public string[] AssetPaths;
        }



        [SerializeField]
        private string m_BuildOutputPath = "Output/";

        public string ManifestName = "StreamingAssets";

        [TypeSelector(typeof(IGameEncryptService))]
        public string EncryptService;

        public BundleFileNameType BundleNameType;


        [FormerlySerializedAs("m_Packages")]
        public PackageBundles[] Packages;


        public string BuildOutputPath
        {
            get { return System.IO.Path.Combine(m_BuildOutputPath, "Output", ManifestName); }
        }

        public string EncryptPath
        {
            get { return System.IO.Path.Combine(m_BuildOutputPath, "Encrypt"); }
        }
    }


    [CustomEditor(typeof(AssetBundlePackageEditorSetting))]
    public class AssetBundlePackageEditorSettingEditor : UnityEditor.Editor
    {
        AssetBundlePackageEditorSetting m_Target;

        private void OnEnable()
        {
            m_Target = target as AssetBundlePackageEditorSetting;
        }

        public override void OnInspectorGUI()
        {
            Check();
            base.OnInspectorGUI();
        }

        private void Check()
        {
            if (m_Target.Packages == null) return;

            List<string> bundleNames = new List<string>();

            string errorMsg = null;

            for (int p = 0; p < m_Target.Packages.Length; p++)
            {
                var item = m_Target.Packages[p];
                if (string.IsNullOrEmpty(item.ModeName))
                {
                    errorMsg = $"Package Mode name is null or empty. Index: {p} ";
                    break;
                }
                for (int i = 0; i < item.Bundles.Length; i++)
                {
                    if (item.Bundles[i].AssetPaths != null)
                    {
                        for (int v = 0; v < item.Bundles[i].AssetPaths.Length; v++)
                        {
                            var fullName = item.Bundles[i].AssetPaths[v];
                            fullName = fullName.ToLower();

                            if (bundleNames.IndexOf(fullName) != -1)
                            {
                                errorMsg = $"Dumplicate Bundle Name:{item.Bundles[i]}, ModeName:{item.ModeName}, Index:{i}";
                                break;
                            }
                            else
                                bundleNames.Add(fullName);
                        }
                    }
                    else
                    {
                        var fullName = item.Bundles[i].BundleName;
                        fullName = fullName.ToLower();

                        if (bundleNames.IndexOf(fullName) != -1)
                        {
                            errorMsg = $"Dumplicate Bundle Name:{item.Bundles[i]}, ModeName:{item.ModeName}, Index:{i}";
                            break;
                        }
                        else
                            bundleNames.Add(fullName);
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg))
                    break;
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                EditorGUILayout.HelpBox(errorMsg, MessageType.Error);
            }
        }

    }

}
