﻿using XyzAssets.Runtime;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace XyzAssets.Editor
{
    public class ResourceSystemBuildTools : MonoBehaviour
    {
        #region Build
        [MenuItem("XyzAssets/Build And Move", priority = 1001)]
        static void BuildAll_Append()
        {
            BuildAll(false, false);
        }

        [MenuItem("XyzAssets/Build And Move(For Editor)", priority = 1001)]
        static void BuildAll_Append_ForEditor()
        {
            BuildAll(false, true);
        }

        [MenuItem("XyzAssets/Build And Move(Force Rebuild)", priority = 1003)]
        static void BuildAll_Clear()
        {
            BuildAll(true, false);
        }

        private static AssetBundlePackageEditorSetting GetSetting()
        {
            var paths = AssetDatabase.FindAssets($"t:{typeof(AssetBundlePackageEditorSetting).Name}");
            var settings = AssetDatabase.LoadAssetAtPath<AssetBundlePackageEditorSetting>(AssetDatabase.GUIDToAssetPath(paths[0]));
            return settings;
        }

        static void BuildAll(bool forceRebuild, bool forEditor)
        {

            //            var settings = GetSetting();
            //            var resFolderPath = Path.Combine(settings.EncryptPath, GetPlatformName(forEditor));
            //            try
            //            {
            //                if (CheckRepeatedNames()) return;


            //                XyzAssetsEditorManifest _allResourceManifest = new XyzAssetsEditorManifest();
            //                XyzAssetsEditorManifest _streamingManifest = new XyzAssetsEditorManifest();
            //                IGameEncryptService encryptService = null;
            //                if (!string.IsNullOrEmpty(settings.EncryptService))
            //                {
            //                    try
            //                    {
            //                        var type = Type.GetType(settings.EncryptService);
            //                        if (type != null)
            //                            encryptService = Activator.CreateInstance(type) as IGameEncryptService;
            //                    }
            //                    catch
            //                    {

            //                    }
            //                }

            //                //build
            //                if (forceRebuild)
            //                {
            //                    if (Directory.Exists(settings.BuildOutputPath))
            //                        Directory.Delete(settings.BuildOutputPath, true);
            //                    Directory.CreateDirectory(settings.BuildOutputPath);
            //                }
            //                else
            //                {
            //                    if (!Directory.Exists(settings.BuildOutputPath))
            //                        Directory.CreateDirectory(settings.BuildOutputPath);
            //                }

            //                var options = forceRebuild ? (BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle) : BuildAssetBundleOptions.ChunkBasedCompression;
            //                var assetBundleManifest = BuildPipeline.BuildAssetBundles(settings.BuildOutputPath, options, forEditor ? BuildTarget.StandaloneWindows : EditorUserBuildSettings.activeBuildTarget);
            //                var bundles = assetBundleManifest.GetAllAssetBundles();

            //                Dictionary<string, string> m_AssetToBundle = new Dictionary<string, string>();
            //                Dictionary<string, BundleEncryptType> m_BundleEncryptType = new Dictionary<string, BundleEncryptType>();
            //                foreach (var _b in bundles)
            //                {
            //                    var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(_b);
            //                    foreach (var _asPath in assetPaths)
            //                    {
            //                        m_AssetToBundle.Add(_asPath, _b);
            //                    }
            //                }

            //                //move to resfloder
            //                if (Directory.Exists(resFolderPath))
            //                    Directory.Delete(resFolderPath, true);
            //                Directory.CreateDirectory(resFolderPath);

            //                if (Directory.Exists(Application.streamingAssetsPath))
            //                    Directory.Delete(Application.streamingAssetsPath, true);
            //                Directory.CreateDirectory(Application.streamingAssetsPath);

            //                float i = 0;
            //                var files = Directory.GetFiles(settings.BuildOutputPath, "*", SearchOption.AllDirectories);
            //                foreach (var item in files)
            //                {
            //                    EditorUtility.DisplayProgressBar("Move To Encrypt Folder", item, i++ / files.Length);

            //                    if (Path.GetExtension(item) == ".manifest")
            //                        continue;
            //                    string newFilePath = item.Replace(settings.BuildOutputPath, resFolderPath);
            //                    if (!Directory.Exists(Path.GetDirectoryName(newFilePath)))
            //                        Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

            //                    var result = ResourceSystemEncryptTools.EncryptItem(item, encryptService);
            //                    File.WriteAllBytes(newFilePath, result.Buffer);
            //                    m_BundleEncryptType.Add(newFilePath, result.EncryptType);
            //                }

            //                for (int _index = 0; _index < settings.Packages.Length; _index++)
            //                {
            //                    var packbundles = settings.Packages[_index];

            //                    if (packbundles == null) continue;

            //                    foreach (var item in packbundles.Bundles)
            //                    {
            //                        var bundleName = item;
            //                        foreach (var _bundleName in bundles)
            //                        {
            //                            if (Path.GetFileNameWithoutExtension(_bundleName) == bundleName)
            //                            {
            //                                var encryptPath = Path.Combine(resFolderPath, _bundleName);
            //                                if (File.Exists(encryptPath))
            //                                {
            //                                    BuildPipeline.GetCRCForAssetBundle(Path.Combine(settings.BuildOutputPath, _bundleName), out uint crc);
            //                                    var md5 = XyzAssetUtils.CalculateMD5(encryptPath);
            //                                    var stream = File.OpenRead(encryptPath);
            //                                    var totalLen = stream.Length;
            //                                    stream.Close();
            //                                    stream.Dispose();

            //                                    var bundleInfo = new EditorBundleInfo()
            //                                    {
            //                                        BundleName = _bundleName,
            //                                        Version = md5,
            //                                        Crc = crc,
            //                                        FileSize = totalLen,
            //                                        ModeName = packbundles.ModeName,
            //                                        EncryptType = m_BundleEncryptType[encryptPath],
            //                                        NameType = settings.BundleNameType,
            //                                    };
            //                                    var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(_bundleName);
            //                                    for (int _assetIndex = 0; _assetIndex < assetPaths.Length; _assetIndex++)
            //                                    {
            //                                        var assetInfo = new EditorAssetInfo() { AssetPath = assetPaths[_assetIndex], MainBundle = _bundleName };
            //                                        var _assetDeps = AssetDatabase.GetDependencies(assetPaths[_assetIndex]);
            //                                        List<string> result = new List<string>();
            //                                        foreach (var _asDep in _assetDeps)
            //                                        {
            //                                            if (m_AssetToBundle.ContainsKey(_asDep))
            //                                            {
            //                                                var _d = m_AssetToBundle[_asDep];
            //                                                if (result.IndexOf(_d) == -1)
            //                                                    result.Add(m_AssetToBundle[_asDep]);
            //                                            }
            //                                        }
            //                                        assetInfo.Dependencies = result.ToArray();
            //                                        _allResourceManifest.AddAssetInfo(assetInfo);
            //                                    }

            //                                    _allResourceManifest.AddBundleInfo(bundleInfo);

            //                                    if (settings.BundleNameType == BundleFileNameType.Hash)
            //                                        FileUtil.MoveFileOrDirectory(Path.Combine(resFolderPath, _bundleName), Path.Combine(resFolderPath, md5));

            //                                    if (packbundles.CopyToStreamingAssetsPath)
            //                                    {
            //                                        for (int _assetIndex = 0; _assetIndex < assetPaths.Length; _assetIndex++)
            //                                        {
            //                                            var assetInfo = new EditorAssetInfo() { AssetPath = assetPaths[_assetIndex], MainBundle = _bundleName };
            //                                            var _assetDeps = AssetDatabase.GetDependencies(assetPaths[_assetIndex]);
            //                                            List<string> result = new List<string>();
            //                                            foreach (var _asDep in _assetDeps)
            //                                            {
            //                                                if (m_AssetToBundle.ContainsKey(_asDep))
            //                                                {
            //                                                    var _d = m_AssetToBundle[_asDep];
            //                                                    if (result.IndexOf(_d) == -1)
            //                                                        result.Add(m_AssetToBundle[_asDep]);
            //                                                }
            //                                            }
            //                                            assetInfo.Dependencies = result.ToArray();
            //                                            _streamingManifest.AddAssetInfo(assetInfo);
            //                                        }

            //                                        _streamingManifest.AddBundleInfo(bundleInfo);
            //                                        if (settings.BundleNameType == BundleFileNameType.Hash)
            //                                            File.Copy(Path.Combine(resFolderPath, md5), Path.Combine(Application.streamingAssetsPath, md5));
            //                                        else
            //                                            File.Copy(Path.Combine(resFolderPath, _bundleName), Path.Combine(Application.streamingAssetsPath, _bundleName));
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    Debug.LogError($"--------{_bundleName} is not exist in {resFolderPath}");
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                File.WriteAllBytes(Path.Combine(resFolderPath, XyzConfiguration.ManifestName), _allResourceManifest.SerializeToBinary());
            //                File.WriteAllText(Path.Combine(resFolderPath, XyzConfiguration.ManifestName) + ".json", _allResourceManifest.SerializeToJson());

            //                if (_streamingManifest.BundleInfos.Count > 0)
            //                {
            //                    File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath, XyzConfiguration.ManifestName), _streamingManifest.SerializeToBinary());
            //                    File.WriteAllText(Path.Combine(Application.streamingAssetsPath, XyzConfiguration.ManifestName + ".json"), _streamingManifest.SerializeToJson());
            //                }

            //                AssetDatabase.SaveAssets();
            //                Debug.Log("Build Success.");
            //            }
            //            catch (Exception e)
            //            {
            //                Debug.LogError(e.Message);
            //                Debug.LogError(e.StackTrace);
            //                Debug.Log("Build Failed.");
            //            }
            //            finally
            //            {
            //                EditorUtility.ClearProgressBar();
            //                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);


            //#if UNITY_EDITOR
            //                Application.OpenURL($"file:///{Path.Combine(Directory.GetCurrentDirectory(), resFolderPath)}");
            //#endif

            //            }
        }
        static bool CheckRepeatedNames()
        {
            bool isRepeated = false;
            try
            {
                var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (var abName in assetBundleNames)
                {
                    EditorUtility.DisplayProgressBar(abName, "", 0);
                    var files = AssetDatabase.GetAssetPathsFromAssetBundle(abName);

                    for (int i = 0; i < files.Length; i++)
                    {
                        EditorUtility.DisplayProgressBar(abName, "", i * 1f / files.Length);
                        var left = Path.GetFileName(files[i]);
                        for (int j = i + 1; j < files.Length; j++)
                        {
                            var right = Path.GetFileName(files[j]);
                            if (left == right)
                            {
                                isRepeated = true;
                                Debug.LogError($"AssetBundle:{abName} files error. Dumplicated file name.\r\n{files[i]}\r\n{files[j]}");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log($"Check done.");
            }
            return isRepeated;
        }

        #endregion

        static string GetPlatformName(bool forEditor)
        {
            if (forEditor)
                return "windows";

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    return "macos";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "windows";
                case BuildTarget.iOS:
                    return "iphone";
                case BuildTarget.Android:
                    return "android";
                default:
                    return Application.platform.ToString().ToLower();
            }
        }

    }

}
