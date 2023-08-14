using UnityEngine.Networking;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineUpdateManifestOperator : UpdateManifestOperator
    {
        internal OnlineUpdateManifestOperator(OnlineSystemImpl impl)
        {
            m_Impl = impl;
        }

        public override void SaveManifest()
        {
            if (m_Manifest != null)
            {
                System.IO.File.WriteAllBytes(AssetsPathHelper.GetFileExternalPath(XyzConfiguration.ManifestName), ManifestSerialize.SerializeToBinary(m_Manifest));
            }
        }
        protected override void OnDispose()
        {
            m_Impl = null;
            if (m_WebRequest != null)
            {
                m_WebRequest.Abort();
                m_WebRequest.Dispose();
                m_WebRequest = null;
            }

            m_Manifest = null;

            m_ResUrls = null;
        }

        protected override void OnExecute()
        {
            if (m_WebRequest == null)
            {
                m_WebRequest = UnityWebRequest.Get(System.IO.Path.Combine(m_ResUrls[m_CurrentUrlIndex], XyzConfiguration.ManifestName));
                m_WebRequest.disposeDownloadHandlerOnDispose = true;
                m_WebRequest.timeout = 10;
                m_WebRequest.SendWebRequest();
            }
            else
            {
                Progress = m_WebRequest.downloadProgress;
                if (!m_WebRequest.isDone) return;
                if (string.IsNullOrEmpty(m_WebRequest.error))
                {
                    m_Manifest = ManifestSerialize.DeserializeFromBinary(m_WebRequest.downloadHandler.data);
                    (m_Impl as OnlineSystemImpl).SetRemoteManifest(m_Manifest);
                    Status = EOperatorStatus.Succeed;
                }
                else
                {
                    if (m_CurrentRetryTimes >= m_MaxRetryTimes)
                    {
                        Error = StringUtility.Format("Uri:{0} Error:{1}", m_WebRequest.uri, m_WebRequest.error);
                        m_WebRequest.Dispose();
                        m_WebRequest = null;
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        m_CurrentRetryTimes++;

                        m_CurrentUrlIndex++;
                        m_CurrentUrlIndex %= m_ResUrls.Length;
                        m_WebRequest.Dispose();
                        m_WebRequest = null;
                    }
                }
            }
        }

        protected override void OnStart()
        {

            m_ResUrls = m_Impl.GetModeService().ResUrls;
            m_MaxRetryTimes = m_Impl.GetModeService().MaxRetryTimes;

            if (m_ResUrls == null || m_ResUrls.Length == 0)
            {
                Error = "OnlineUpdateManifest Error. ResUrls is null or empty";
                Status = EOperatorStatus.Failed;
            }
            m_CurrentRetryTimes = 0;
            m_CurrentUrlIndex = 0;
        }

        private int m_CurrentRetryTimes = 0;
        private int m_CurrentUrlIndex = 0;
        private UnityWebRequest m_WebRequest;
        private OnlineSystemImpl m_Impl;
        private string[] m_ResUrls;
        private int m_MaxRetryTimes;
        private RuntimeManifest m_Manifest;

    }
}