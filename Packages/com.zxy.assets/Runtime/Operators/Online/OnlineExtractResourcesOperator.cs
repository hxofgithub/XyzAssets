
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineExtractResourcesOperator : ExtractResourcesOperator
    {
        internal OnlineExtractResourcesOperator(string extractPath, BundleInfo[] bundleInfos) : base(extractPath, bundleInfos)
        {

        }

        protected override void OnStart()
        {
            if (m_BundleInfos == null || m_BundleInfos.Length == 0)
                Status = EOperatorStatus.Succeed;
            else
            {
                //XyzAssetPathHelper.ExternalPath = m_ExtractRootPath;
                m_ExtractIndex = -1;
                for (int i = 0; i < m_BundleInfos.Length; i++)
                {
                    var _bundle = m_BundleInfos[i];
                    var bundleName = _bundle.NameType == BundleFileNameType.Hash ? _bundle.Version : _bundle.BundleName;
                    if (StreamingAssetsHelper.FileExists(bundleName) && !File.Exists(AssetsPathHelper.GetFileExternalPath(bundleName)))
                    {
#if UNITY_ANDROID && !UNITY_EDITOR
                        m_WebRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, bundleName));
                        m_WebRequest.disposeUploadHandlerOnDispose = true;
                        m_WebRequest.SendWebRequest();
#endif
                        m_ExtractIndex = i;
                        break;
                    }
                }
                if (m_ExtractIndex == -1)
                {
                    Status = EOperatorStatus.Succeed;
                }
            }
        }

        protected override void OnExecute()
        {
            if (m_WebRequest != null)
            {
                Progress = (m_ExtractIndex + m_WebRequest.downloadProgress) / m_BundleInfos.Length;
                if (!m_WebRequest.isDone) return;

                var _bundle = m_BundleInfos[m_ExtractIndex];
                var bundleName = _bundle.NameType == BundleFileNameType.Hash ? _bundle.Version : _bundle.BundleName;

                if (string.IsNullOrEmpty(m_WebRequest.error))
                {
                    File.WriteAllBytes(AssetsPathHelper.GetFileExternalPath(bundleName), m_WebRequest.downloadHandler.data);
                    m_ExtractIndex++;

                    if (m_ExtractIndex == m_BundleInfos.Length)
                    {
                        Status = EOperatorStatus.Succeed;

                        m_WebRequest.Dispose();
                        m_WebRequest = null;

                    }
                    else
                    {
                        m_WebRequest.Dispose();
                        m_WebRequest = null;

                        _bundle = m_BundleInfos[m_ExtractIndex];
                        bundleName = _bundle.NameType == BundleFileNameType.Hash ? _bundle.Version : _bundle.BundleName;
                        m_WebRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, bundleName));
                        m_WebRequest.disposeUploadHandlerOnDispose = true;
                        m_WebRequest.SendWebRequest();
                    }
                }
                else
                {
                    Error = StringUtility.Format("Extract Failed: {0}", m_WebRequest.error);
                    Status = EOperatorStatus.Failed;
                }
            }
            else
            {
                Progress = (m_ExtractIndex + 1f) / m_BundleInfos.Length;

                var _bundle = m_BundleInfos[m_ExtractIndex];
                var bundleName = _bundle.NameType == BundleFileNameType.Hash ? _bundle.Version : _bundle.BundleName;

                var streamingPath = Path.Combine(Application.streamingAssetsPath, bundleName);
                var externalPath = AssetsPathHelper.GetFileExternalPath(bundleName);
                if (File.Exists(streamingPath) && !File.Exists(externalPath))
                    File.Copy(streamingPath, externalPath, true);

                m_ExtractIndex++;

                if (m_ExtractIndex == m_BundleInfos.Length)
                {
                    Status = EOperatorStatus.Succeed;
                }
            }
        }

        protected override void OnDispose()
        {
            if (m_WebRequest != null)
            {
                m_WebRequest.Abort();
                m_WebRequest.Dispose();
                m_WebRequest = null;
            }

            m_BundleInfos = null;
            m_ExtractRootPath = null;
        }


        private int m_ExtractIndex = 0;
        private UnityWebRequest m_WebRequest;
    }

}
