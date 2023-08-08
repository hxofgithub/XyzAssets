﻿using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineLoadBundleOperator : LoadBundleOperator
    {
        private enum EStep
        {
            None,

            DownloadFromRemote,

            WaitAssetBundleRequest,

            ExtractFormStreamingAssetPath,
        }

        public override EOperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                if (value == EOperatorStatus.Succeed)
                    XyzLogger.LogWarning(StringUtility.Format("-------[Load Bundle]: {0}", m_BundleInfo.BundleName));

                base.Status = value;
            }
        }

        internal OnlineLoadBundleOperator(OnlineAssetsSystemImpl impl, BundleInfo bundleInfo) : base(bundleInfo)
        {
            m_Impl = impl;
            m_Step = EStep.None;

        }
        protected override void OnDispose()
        {
            XyzLogger.LogWarning(StringUtility.Format("-------[UnLoad Bundle]: {0}", m_BundleInfo.BundleName));

            if (m_AsyncRequest != null)
            {
                m_AsyncRequest.Abort();
                m_AsyncRequest.Dispose();
                m_AsyncRequest = null;
            }

            if (m_Stream != null)
            {
                m_Stream.Close();
                m_Stream.Dispose();
                m_Stream = null;
            }

            if (CachedBundle != null)
            {
                CachedBundle.Unload(true);
                CachedBundle = null;
            }

            if (m_DownloadOperator != null)
            {
                m_DownloadOperator.Dispose();
                m_DownloadOperator = null;
            }

            m_Impl = null;
        }

        protected override void OnExecute()
        {
            if (m_Step == EStep.ExtractFormStreamingAssetPath)
            {
                if (m_ExtractOpera == null) return;

                Progress = m_ExtractOpera.Progress * .5f;

                if (!m_ExtractOpera.IsDone) return;

                if (m_ExtractOpera.Status == EOperatorStatus.Succeed)
                {
                    m_Step = EStep.WaitAssetBundleRequest;
                }
                else
                {
                    Error = m_ExtractOpera.Error;
                    Status = EOperatorStatus.Failed;
                }
                m_ExtractOpera.Dispose();
                m_ExtractOpera = null;
            }
            else if (m_Step == EStep.DownloadFromRemote)
            {
                if (m_DownloadOperator == null) return;
                Progress = m_DownloadOperator.Progress * .5f;
                if (!m_DownloadOperator.IsDone) return;

                if (m_DownloadOperator.Status == EOperatorStatus.Succeed)
                {
                    m_Step = EStep.WaitAssetBundleRequest;
                }
                else
                {
                    Error = m_DownloadOperator.Error;
                    m_Step = EStep.None;
                    Status = EOperatorStatus.Failed;
                }
                m_DownloadOperator = null;
            }
            else if (m_Step == EStep.WaitAssetBundleRequest)
            {
                if (CachedBundle == null)
                    LoadAssetBundleFromExternal();
            }
        }

        protected override void OnStart()
        {
            m_Step = EStep.None;

            var bundleName = m_BundleInfo.NameType == BundleFileNameType.Hash ? m_BundleInfo.Version : m_BundleInfo.BundleName;
            //cached ?
            if (!File.Exists(XyzAssetPathHelper.GetFileExternalPath(bundleName)))
            {

                if (!StreamingAssetsHelper.FileExists(bundleName))
                {
                    m_Step = EStep.DownloadFromRemote;
                    m_DownloadOperator = new OnlineDownloadOperator(m_Impl, new BundleInfo[] { m_BundleInfo });
                }
                else
                {
                    LoadAssetBundleFromStreamingAssets();
                }
            }
            else
            {
                LoadAssetBundleFromExternal();
            }
        }

        private void LoadAssetBundleFromExternal()
        {
            var bundleName = m_BundleInfo.NameType == BundleFileNameType.Hash ? m_BundleInfo.Version : m_BundleInfo.BundleName;
            var filePath = XyzAssetPathHelper.GetFileExternalPath(bundleName);

            //XyzLogger.Log($"----LoadAssetBundleFromExternal---: Name:{m_BundleInfo.BundleName}  Version:{m_BundleInfo.Version}--");

            if (m_BundleInfo.EncryptType == BundleEncryptType.FileOffset)
            {
                CachedBundle = AssetBundle.LoadFromFile(filePath, m_BundleInfo.Crc, (ulong)m_Impl.GetDecryptService().GetFileOffset(filePath));
                if (CachedBundle == null)
                {
                    Error = StringUtility.Format("Can load assetbundle form {0}", filePath);
                    Status = EOperatorStatus.Failed;
                }
                else
                {
                    Status = EOperatorStatus.Succeed;
                }

                m_Step = EStep.None;
            }
            else if (m_BundleInfo.EncryptType == BundleEncryptType.Stream)
            {
                m_Stream = m_Impl.GetDecryptService().GetDecryptStream(filePath);
                CachedBundle = AssetBundle.LoadFromStream(m_Stream, m_BundleInfo.Crc);
                if (CachedBundle == null)
                {
                    Error = StringUtility.Format("Can load assetbundle form {0}", filePath);
                    Status = EOperatorStatus.Failed;
                }
                else
                {
                    Status = EOperatorStatus.Succeed;
                }

                m_Step = EStep.None;
            }
            else
            {
                CachedBundle = AssetBundle.LoadFromFile(filePath, m_BundleInfo.Crc);
                if (CachedBundle == null)
                {
                    Error = StringUtility.Format("Can load assetbundle form {0}", filePath);
                    Status = EOperatorStatus.Failed;
                }
                else
                {
                    Status = EOperatorStatus.Succeed;
                }
            }
        }

        private void LoadAssetBundleFromStreamingAssets()
        {

            var bundleName = m_BundleInfo.NameType == BundleFileNameType.Hash ? m_BundleInfo.Version : m_BundleInfo.BundleName;
            var filePath = Path.Combine(Application.streamingAssetsPath, bundleName);

            //XyzLogger.Log($"----LoadAssetBundleFromStreamingAssets---:{filePath}");

            if (Application.platform == RuntimePlatform.Android)
            {
                m_ExtractOpera = new AndriodExtractToExtenalFromStreamingAssets(bundleName);
                m_Step = EStep.ExtractFormStreamingAssetPath;
            }
            else
            {
                if (m_BundleInfo.EncryptType == BundleEncryptType.FileOffset)
                {
                    CachedBundle = AssetBundle.LoadFromFile(filePath, m_BundleInfo.Crc, (ulong)m_Impl.GetDecryptService().GetFileOffset(filePath));
                    if (CachedBundle == null)
                    {
                        Error = StringUtility.Format("Can load assetbundle form {0}", filePath);
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        Status = EOperatorStatus.Succeed;
                    }
                }
                else if (m_BundleInfo.EncryptType == BundleEncryptType.Stream)
                {
                    m_Stream = m_Impl.GetDecryptService().GetDecryptStream(filePath);
                    CachedBundle = AssetBundle.LoadFromStream(m_Stream, m_BundleInfo.Crc);
                    if (CachedBundle == null)
                    {
                        Error = StringUtility.Format("Can load assetbundle form {0}", filePath);
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        Status = EOperatorStatus.Succeed;
                    }
                }
                else
                {
                    CachedBundle = AssetBundle.LoadFromFile(filePath, m_BundleInfo.Crc);
                    if (CachedBundle == null)
                    {
                        Error = StringUtility.Format("Can load assetbundle form {0}", filePath);
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        Status = EOperatorStatus.Succeed;
                    }
                }
            }
        }

        private OnlineAssetsSystemImpl m_Impl;
        private Stream m_Stream;
        private EStep m_Step;

        private UnityWebRequest m_AsyncRequest;
        private DownloadOperator m_DownloadOperator;
        private AndriodExtractToExtenalFromStreamingAssets m_ExtractOpera;

        private class AndriodExtractToExtenalFromStreamingAssets : AsyncOperationBase
        {
            public AndriodExtractToExtenalFromStreamingAssets(string fileName)
            {
                m_FileName = fileName;
            }

            protected override void OnDispose()
            {
                if (m_WebRequest != null)
                {
                    m_WebRequest.Abort();
                    m_WebRequest.Dispose();
                    m_WebRequest = null;
                }
            }

            protected override void OnExecute()
            {
                Progress = m_WebRequest.downloadProgress;
                if (!m_WebRequest.isDone) return;
                if (string.IsNullOrEmpty(m_WebRequest.error))
                {
                    File.WriteAllBytes(XyzAssetPathHelper.GetFileExternalPath(m_FileName), m_WebRequest.downloadHandler.data);
                    Status = EOperatorStatus.Succeed;
                }
                else
                {
                    Error = m_WebRequest.error;
                    Status = EOperatorStatus.Failed;
                }
            }

            protected override void OnStart()
            {
                m_WebRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, m_FileName));
                m_WebRequest.disposeDownloadHandlerOnDispose = true;
                m_WebRequest.SendWebRequest();
            }

            private UnityWebRequest m_WebRequest;
            private readonly string m_FileName;

        }
    }

    internal class OnlineLoadBundleOperatorEx : LoadBundleOperator
    {
        public OnlineLoadBundleOperatorEx(OnlineAssetsSystemImpl impl, int mainId, int[] bundleInfo) : base(null)
        {
            m_Impl = impl;
            m_WaitBundleInfos = bundleInfo;
            m_MainId = mainId;
        }

        protected override void OnDispose()
        {
            m_Impl = null;
            m_WaitBundleInfos = null;
            m_LoadBundleOperators = null;

            m_MainId = -1;

            CachedBundle = null;
        }

        protected override void OnExecute()
        {
            if (m_LoadBundleOperators.Length == 0) return;
            m_Progress = 0;
            bool _isDone = true;
            for (int i = 0; i < m_LoadBundleOperators.Length; i++)
            {
                m_Progress += m_LoadBundleOperators[i].Progress;
                if (!m_LoadBundleOperators[i].IsDone)
                    _isDone = false;
            }
            Progress = m_Progress / m_LoadBundleOperators.Length;

            if (!_isDone) return;

            var _state = EOperatorStatus.Succeed;
            for (int i = 0; i < m_LoadBundleOperators.Length; i++)
            {
                if (m_LoadBundleOperators[i].Status == EOperatorStatus.Failed)
                {
                    _state = EOperatorStatus.Failed;
                    Error = m_LoadBundleOperators[i].Error;
                    break;
                }
            }

            if (_state == EOperatorStatus.Succeed)
                CachedBundle = m_Impl.GetBundle(m_MainId);
            Status = _state;

        }

        protected override void OnStart()
        {
            if (m_WaitBundleInfos == null || m_WaitBundleInfos.Length == 0)
            {
                Status = EOperatorStatus.Succeed;
            }
            else
            {
                m_LoadBundleOperators = new LoadBundleOperator[m_WaitBundleInfos.Length];
                for (int i = 0; i < m_WaitBundleInfos.Length; i++)
                {
                    m_LoadBundleOperators[i] = m_Impl.LoadBundle(m_WaitBundleInfos[i]);
                }
            }
        }

        private LoadBundleOperator[] m_LoadBundleOperators;
        private int[] m_WaitBundleInfos;
        private OnlineAssetsSystemImpl m_Impl;
        private int m_MainId;
        private float m_Progress;
    }

}
