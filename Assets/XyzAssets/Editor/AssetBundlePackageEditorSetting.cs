using XyzAssets.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AssetBundlePackageEditorSetting", menuName = "Create AssetBundlePackageEditorSetting")]
public class AssetBundlePackageEditorSetting : ScriptableObject
{
    [System.Serializable]
    public class PackageBundles
    {
        public string ModeName;
        public bool CopyToStreamingAssets;
        public string[] Bundles;
    }
    [SerializeField]
    private string m_BuildOutputPath = "Output/";
    public string VariantName = string.Empty;
    public string ManifestName = "StreamingAssets";

    public BundleEncryptType EncryptType = BundleEncryptType.None;



    public BundleFileNameType BundleNameType;


    public string BuildOutputPath
    {
        get { return m_BuildOutputPath + ManifestName; }
    }



    [FormerlySerializedAs("m_Packages")]
    public PackageBundles[] Packages;

    private void OnValidate()
    {
        if (Packages == null) return;

        List<string> bundleNames = new List<string>();

        foreach (var item in Packages)
        {
            for (int i = 0; i < item.Bundles.Length; i++)
            {
                item.Bundles[i] = item.Bundles[i].ToLower();
                if (bundleNames.IndexOf(item.Bundles[i]) != -1)
                {
                    Debug.LogError($"Dumplicate Bundle Name:{item.Bundles[i]}, Index:{i}");
                }
                else
                    bundleNames.Add(item.Bundles[i]);
            }
        }
    }

}
