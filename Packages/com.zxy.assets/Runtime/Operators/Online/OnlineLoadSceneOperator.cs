using UnityEngine;
using UnityEngine.SceneManagement;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineLoadSceneOperator : LoadSceneOperator
    {
        private enum LoadSceneStep
        {
            None,
            LoadBundle,
            LoadScene,
            WaitSceneCompleted,
            Done,
        }

        internal OnlineLoadSceneOperator(OnlineSystemImpl impl, AssetInfo assetInfo, LoadSceneMode sceneMode) : base(assetInfo, sceneMode)
        {
            m_Impl = impl;
        }

        protected override void OnExecute()
        {
            if (m_Step == LoadSceneStep.LoadBundle)
            {
                if (m_LoadBundleOperator == null) return;

                Progress = m_LoadBundleOperator.Progress * 0.5f;

                if (!m_LoadBundleOperator.IsDone) return;

                if (m_LoadBundleOperator.Status == EOperatorStatus.Failed)
                {
                    Error = m_LoadBundleOperator.Error;
                    Status = m_LoadBundleOperator.Status;
                    m_Step = LoadSceneStep.None;
                }
                else
                {
                    m_LoadSceneOpera = SceneManager.LoadSceneAsync(m_SceneName, m_LoadSceneMode);
                    if (m_LoadSceneOpera != null)
                    {
                        m_LoadSceneOpera.allowSceneActivation = true;
                        m_SceneObject = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        m_Step = LoadSceneStep.LoadScene;
                    }
                    else
                    {
                        Error = StringUtility.Format("Failed to load scene: {0}", m_SceneName);
                        Status = EOperatorStatus.Failed;
                        m_Step = LoadSceneStep.None;
                    }

                }
            }
            else if (m_Step == LoadSceneStep.LoadScene)
            {
                Progress = .5f + m_LoadSceneOpera.progress;
                if (!m_LoadSceneOpera.isDone) return;
                Progress = 1;
                m_Step = LoadSceneStep.WaitSceneCompleted;
            }
            else if (m_Step == LoadSceneStep.WaitSceneCompleted)
            {
                if (m_SceneObject.IsValid())
                {
                    SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
                    SceneManager.SetActiveScene(m_SceneObject);
                    Status = EOperatorStatus.Succeed;
                }
                else
                {
                    Error = StringUtility.Format("The loaded scene is invalid : {0}", base.m_AssetInfo);
                    Status = EOperatorStatus.Failed;
                }
                m_Step = LoadSceneStep.Done;
            }
        }

        //protected override void OnDispose()
        //{
        //    SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;

        //    m_SceneObject = default;

        //    m_SceneName = null;

        //    m_LoadBundleOperator = null;
        //    m_LoadSceneOpera = null;

        //    if (m_AssetInfo != null)
        //    {
        //        m_Impl.UnLoadBundle(m_AssetInfo);
        //        m_AssetInfo = null;
        //    }
        //    m_Step = LoadSceneStep.None;
        //    m_Impl = null;
        //}

        protected override void OnStart()
        {
            m_LoadBundleOperator = m_Impl.LoadBundle(m_AssetInfo);
            m_Step = LoadSceneStep.LoadBundle;
        }
        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            if (arg0.name == m_SceneName)
            {
                //Dispose();
            }
        }

        private Scene m_SceneObject;
        private LoadSceneStep m_Step;
        private LoadBundleOperator m_LoadBundleOperator;
        private AsyncOperation m_LoadSceneOpera;
        private OnlineSystemImpl m_Impl;

    }
}