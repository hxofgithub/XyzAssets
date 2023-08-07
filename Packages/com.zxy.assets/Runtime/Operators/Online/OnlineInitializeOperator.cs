using UnityEngine;
using System.IO;
using UnityEngine.Networking;

namespace XyzAssets.Runtime
{
    public sealed class OnlineInitializeParameters : InitializeParameters
    {
    }
    internal sealed class OnlineInitializeOperator : InitializeOperator
    {
        internal OnlineInitializeOperator(OnlineAssetsSystemImpl impl, OnlineInitializeParameters parameters)
        {
            m_OnlineParameters = parameters;
            m_Impl = impl;
        }

        protected override void OnExecute()
        {
            if (m_WebRequestAsync == null) return;

            Progress = m_WebRequestAsync.downloadProgress;
            if (!m_WebRequestAsync.isDone) return;

            if (string.IsNullOrEmpty(m_WebRequestAsync.error))
            {
                ProcessManifest(m_WebRequestAsync.downloadHandler.data);
                m_WebRequestAsync.Dispose();
                m_WebRequestAsync = null;
            }
            else
            {
                if (++m_RetryTimes <= m_Impl.GetModeService().MaxRetryTimes)
                {
                    m_OnlineResIndex += 1;
                    m_OnlineResIndex %= m_Impl.GetModeService().ResUrls.Length;

                    m_WebRequestAsync.Dispose();
                    m_WebRequestAsync = null;

                    m_WebRequestAsync = UnityWebRequest.Get(Path.Combine(m_Impl.GetModeService().ResUrls[m_OnlineResIndex], XyzConfiguration.ManifestName));
                    m_WebRequestAsync.disposeDownloadHandlerOnDispose = true;
                    m_WebRequestAsync.SendWebRequest();
                }
                else
                {
                    Error = m_WebRequestAsync.error;
                    m_WebRequestAsync.Dispose();
                    m_WebRequestAsync = null;

                    Status = OperatorStatus.Failed;
                }

            }
        }

        protected override void OnStart()
        {
            //检测解压路径
            if (File.Exists(XyzAssetPathHelper.GetFileExternalPath(XyzConfiguration.ManifestName)))
            {
                var binary = File.ReadAllBytes(XyzAssetPathHelper.GetFileExternalPath(XyzConfiguration.ManifestName));
                ProcessManifest(binary);
            }
            else
            {
                //检测streamingassets
                if (StreamingAssetsHelper.FileExists(XyzConfiguration.ManifestName))
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    m_WebRequestAsync = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, XyzConfiguration.ManifestName));
                    m_WebRequestAsync.disposeDownloadHandlerOnDispose = true;
                    m_WebRequestAsync.SendWebRequest();
#else
                    var binary = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, XyzConfiguration.ManifestName));
                    ProcessManifest(binary);
#endif
                }
                else
                {
                    if (m_OnlineParameters == null || m_Impl.GetModeService().ResUrls == null || m_Impl.GetModeService().ResUrls.Length == 0)
                    {
                        Error = $"InitParameters data error. ResUrls is null";
                        Status = OperatorStatus.Failed;
                    }
                    else
                    {
                        m_OnlineResIndex = 0;
                        m_RetryTimes = 0;

                        m_WebRequestAsync = UnityWebRequest.Get(Path.Combine(m_Impl.GetModeService().ResUrls[m_OnlineResIndex], XyzConfiguration.ManifestName));
                        m_WebRequestAsync.disposeDownloadHandlerOnDispose = true;
                        m_WebRequestAsync.SendWebRequest();
                    }
                }
            }
        }
        protected override void OnDispose()
        {
            if (m_WebRequestAsync != null)
            {
                m_WebRequestAsync.Abort();
                m_WebRequestAsync.Dispose();
                m_WebRequestAsync = null;
            }
            m_OnlineParameters = null;

            //Debug.Log("-----OnlineInitializeOperator Dispose");

            m_Impl = null;
        }

        private void ProcessManifest(byte[] binary)
        {
            if (binary == null || binary.Length == 0)
            {
                Error = "Binary Data is null";
                Status = OperatorStatus.Failed;
            }
            else
            {
                try
                {
                    XyzAssetsRuntimeManifest manifest = ManifestSerialize.DeserializeFromBinary(binary);
                    var path = XyzAssetPathHelper.GetFileExternalPath(XyzConfiguration.ManifestName);
                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                    File.WriteAllBytes(path, binary);
                    m_Impl.SetActiveManifest(manifest);
                    Status = OperatorStatus.Success;
                }
                catch (System.Exception e)
                {
                    Error = e.Message;
                    Status = OperatorStatus.Failed;
                    throw;
                }
            }
        }

        private UnityWebRequest m_WebRequestAsync;
        private OnlineInitializeParameters m_OnlineParameters;
        private OnlineAssetsSystemImpl m_Impl;
        private int m_OnlineResIndex;
        private int m_RetryTimes;
    }
}