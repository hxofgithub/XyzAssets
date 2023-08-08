using UnityEngine;
using UnityEngine.SceneManagement;

namespace XyzAssets.Runtime
{
    internal sealed class SimulateLoadSceneOperator : LoadSceneOperator
    {
        private enum LoadSceneStep
        {
            None,
            LoadBundle,
            LoadScene,
            WaitSceneCompleted,
            Done,
        }

        internal SimulateLoadSceneOperator(AssetInfo scenePath, LoadSceneMode sceneMode) : base(scenePath, sceneMode)
        {
        }

        protected override void OnExecute()
        {
#if UNITY_EDITOR
            if (m_Step == LoadSceneStep.LoadBundle)
            {
                var parameters = new LoadSceneParameters
                {
                    loadSceneMode = m_LoadSceneMode
                };
                m_LoadSceneOpera = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(m_AssetInfo.AssetPath, parameters);
                if (m_LoadSceneOpera != null)
                {
                    m_LoadSceneOpera.allowSceneActivation = true;
                    m_SceneObject = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                    m_Step = LoadSceneStep.LoadScene;
                }
                else
                {
                    Error = StringUtility.Format("Failed to load scene: {0}", m_AssetInfo.AssetPath);
                    Status = EOperatorStatus.Failed;
                    m_Step = LoadSceneStep.None;
                }
            }
            else if (m_Step == LoadSceneStep.LoadScene)
            {
                Progress = m_LoadSceneOpera.progress;
                if (!m_LoadSceneOpera.isDone) return;
                Progress = 1;
                m_Step = LoadSceneStep.WaitSceneCompleted;
            }
            else if (m_Step == LoadSceneStep.WaitSceneCompleted)
            {
                if (m_SceneObject.IsValid())
                {
                    Status = EOperatorStatus.Succeed;
                    SceneManager.SetActiveScene(m_SceneObject);
                }
                else
                {
                    Error = StringUtility.Format("The loaded scene is invalid : {0}", m_AssetInfo.AssetPath);
                    Status = EOperatorStatus.Failed;
                }

                m_Step = LoadSceneStep.Done;
            }
#endif

        }

        protected override void OnDispose()
        {
#if UNITY_EDITOR
            m_AssetInfo = null;
            m_SceneName = null;

            m_LoadSceneOpera = null;
            m_SceneObject = default;
            m_Step = LoadSceneStep.None;
#endif
        }

        protected override void OnStart()
        {

#if UNITY_EDITOR
            m_Step = LoadSceneStep.LoadBundle;
#endif
        }

#if UNITY_EDITOR
        private Scene m_SceneObject;
        private AsyncOperation m_LoadSceneOpera;
        private LoadSceneStep m_Step;
#endif
    }
}