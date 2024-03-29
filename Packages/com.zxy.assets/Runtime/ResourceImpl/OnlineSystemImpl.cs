﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineSystemImpl : IAssetsSystemImpl
    {
        #region public methods
        public void SetBundleWrapFunc(Func<string, string> wrapFunc)
        {
            m_BundleWrapFunc = wrapFunc;
        }
        public ExtractResourcesOperator ExtractResources(string extractPath, string[] modeNames)
        {
            var bundleInfos = GetBundleInfos(modeNames);
            return new OnlineExtractResourcesOperator(extractPath, bundleInfos);
        }
        public BaseLoadAssetOperator LoadAssetAsync<T>(string assetPath)
        {
            return LoadAssetAsync(assetPath, typeof(T));
        }

        public BaseLoadAssetOperator LoadAssetAsync(string assetPath, Type type)
        {
            var assetInfo = GetAssetInfo(assetPath);
            if (assetInfo == null)
                throw new ArgumentNullException();
            return new OnlineLoadAssetOperator(this, assetInfo, type, true);
        }
        public InitializeOperator Initialize(InitializeParameters initializeParameters)
        {
            var onlineParams = initializeParameters as OnlineInitializeParameters;
            var op = new OnlineInitializeOperator(this, onlineParams);
            return op;
        }
        public UpdateManifestOperator UpdateManifest()
        {
            return new OnlineUpdateManifestOperator(this);
        }
        public LoadSceneOperator LoadSceneOperator(string scenePath, LoadSceneMode sceneMode)
        {
            var assetInfo = GetAssetInfo(scenePath);
            if (assetInfo == null)
                throw new ArgumentNullException();
            return new OnlineLoadSceneOperator(this, assetInfo, sceneMode);
        }
        public DownloadOperator CreateDownloaderByModes(string[] modeNames)
        {
            return new OnlineDownloadOperator(this, GetDeferentBundles(modeNames));
        }

        public BaseLoadAllAssetsOperator LoadAssetsAsync(List<KeyValuePair<string, Type>> valuePairs)
        {
            return new OnlineLoadAllSubAssetsOperator(this, valuePairs);
        }
        public BaseLoadAllAssetsOperator LoadAssetsAsync<T>(IEnumerable<string> collection)
        {
            return new OnlineLoadAllSubAssetsOperator(this, collection, typeof(T));
        }

        public void Dispose()
        {
            m_LoadBundleOperator.Clear();
            m_BundleRef.Clear();
            m_RemoteManifest = null;
            m_ActiveManifest = null;
            m_PlayModeService = null;
            XyzLogger.Log("OnlineAssetsSystemImpl Dispose");
        }

        #endregion

        #region internal

        internal IPlayModeService GetModeService()
        {
            return m_PlayModeService;
        }

        internal void UnLoadBundle(AssetInfo assetInfo)
        {
            if (assetInfo != null)
            {
                foreach (var item in assetInfo.Dependencies)
                {
                    InternalUnloadBundle(item);
                }
            }
        }
        internal void SetActiveManifest(RuntimeManifest manifest)
        {
            m_ActiveManifest = manifest;
        }
        internal void SetRemoteManifest(RuntimeManifest manifest)
        {
            m_RemoteManifest = manifest;
        }

        internal LoadBundleOperator LoadBundle(int bundleId)
        {
            var newBundleName = GetBundleWrapName(bundleId);
            var newBundleInfo = GetActiveBundleInfo(newBundleName, out int newBundleId);

            if (m_LoadBundleOperator.ContainsKey(newBundleId))
            {
                m_BundleRef[newBundleId]++;
                return m_LoadBundleOperator[newBundleId];
            }

            var op = new OnlineLoadBundleOperator(newBundleId, true);
            m_LoadBundleOperator.Add(newBundleId, op);
            m_BundleRef[newBundleId] = 1;
            return op;
        }
        internal LoadBundleOperator LoadBundle(AssetInfo assetInfo)
        {
            return null;
        }

        #endregion

        #region private methods
        private AssetInfo GetAssetInfo(string assetPath)
        {
            if (m_RemoteManifest == null)
                return m_ActiveManifest.GetAssetInfo(assetPath);
            else
                return m_RemoteManifest.GetAssetInfo(assetPath);
        }
        private BundleInfo[] GetBundleInfos(string[] modeNames)
        {
            if (m_ActiveManifest == null && m_RemoteManifest == null)
                return null;

            if (m_RemoteManifest == null)
                return m_ActiveManifest.GetModeBundleInfos(modeNames);
            else
                return m_RemoteManifest.GetModeBundleInfos(modeNames);
        }
        private BundleInfo GetActiveBundleInfo(int bundleId)
        {
            if (m_RemoteManifest == null)
                return m_ActiveManifest.GetBundleInfo(bundleId);
            else
                return m_RemoteManifest.GetBundleInfo(bundleId);
        }
        private BundleInfo GetActiveBundleInfo(string bundleName, out int bundleId)
        {
            if (m_RemoteManifest == null)
                return m_ActiveManifest.GetBundleInfo(bundleName, out bundleId);
            else
                return m_RemoteManifest.GetBundleInfo(bundleName, out bundleId);
        }
        private BundleInfo GetLocalBundleInfo(string bundleName)
        {
            if (m_ActiveManifest == null)
                return null;

            for (int i = 0; i < m_ActiveManifest.BundleList.Length; i++)
            {
                if (m_ActiveManifest.BundleList[i].BundleName == bundleName)
                    return m_ActiveManifest.BundleList[i];
            }
            return null;
        }
        private BundleInfo[] InternalGetBundleInfosByModeName(RuntimeManifest manifest, string modeName)
        {
            if (string.IsNullOrEmpty(modeName))
                return manifest.BundleList;
            List<BundleInfo> infos = new List<BundleInfo>();
            for (int m = 0; m < manifest.BundleList.Length; m++)
            {
                if (manifest.BundleList[m].ModeName == modeName)
                    infos.Add(manifest.BundleList[m]);
            }
            return infos.ToArray();
        }
        private BundleInfo[] GetDeferentBundles(string[] modeNames)
        {
            if (m_RemoteManifest == null) return null;

            List<BundleInfo> result = new List<BundleInfo>();
            for (int i = 0; i < modeNames.Length; i++)
            {
                var remoteBundles = InternalGetBundleInfosByModeName(m_RemoteManifest, modeNames[i]);
                if (remoteBundles == null)
                    continue;

                for (int _i = 0; _i < remoteBundles.Length; _i++)
                {
                    var bundle = remoteBundles[_i];
                    var localBundle = GetLocalBundleInfo(bundle.BundleName);
                    if (localBundle == null)
                        result.Add(bundle);
                    else
                    {
                        if (bundle.Version != localBundle.Version || bundle.Crc != localBundle.Crc || bundle.NameType != localBundle.NameType || bundle.EncryptType != localBundle.EncryptType)
                        {
                            result.Add(bundle);
                        }
                        else
                        {
                            var bundleName = bundle.NameType == BundleFileNameType.BundleName ? bundle.BundleName : bundle.Version;
                            if (!System.IO.File.Exists(AssetsPathHelper.GetFileExternalPath(bundleName)))
                                result.Add(bundle);
                        }
                    }
                }
            }

            return result.ToArray();
        }
        private void InternalUnloadBundle(int bundleId)
        {
            if (m_LoadBundleOperator.ContainsKey(bundleId))
            {
                var op = m_LoadBundleOperator[bundleId];
                if (op.Status == EOperatorStatus.Succeed)
                {
                    m_BundleRef[bundleId] -= 1;
                    if (m_BundleRef[bundleId] <= 0)
                    {
                        m_LoadBundleOperator.Remove(bundleId);
                        m_BundleRef.Remove(bundleId);
                    }
                }
                else if (op.Status == EOperatorStatus.None)
                {
                    var _id = bundleId;
                    op.OnComplete += (handler) =>
                    {
                        if (m_BundleRef.ContainsKey(_id))
                        {
                            m_BundleRef[_id] -= 1;
                            if (m_BundleRef[_id] <= 0)
                            {
                                var _op = m_LoadBundleOperator[_id];
                                m_LoadBundleOperator.Remove(_id);
                                m_BundleRef.Remove(_id);
                            }
                        }
                    };
                }
            }
        }

        private string GetBundleWrapName(int bundleId)
        {
            var bundleInfo = GetActiveBundleInfo(bundleId);
            return m_BundleWrapFunc == null ? bundleInfo.BundleName : m_BundleWrapFunc(bundleInfo.BundleName);
        }

        #endregion

        private RuntimeManifest m_ActiveManifest;
        private RuntimeManifest m_RemoteManifest;
        private readonly Dictionary<int, LoadBundleOperator> m_LoadBundleOperator = new Dictionary<int, LoadBundleOperator>();
        private readonly Dictionary<int, int> m_BundleRef = new Dictionary<int, int>();
        private IPlayModeService m_PlayModeService;

        private Func<string, string> m_BundleWrapFunc;

    }
}
