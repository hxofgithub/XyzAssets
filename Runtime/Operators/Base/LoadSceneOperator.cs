using UnityEngine.SceneManagement;
using UnityEngine;
namespace XyzAssets.Runtime
{
    public abstract class LoadSceneOperator : ResourceBaseOperator
    {

        internal LoadSceneOperator(AssetInfo assetInfo, LoadSceneMode sceneMode)
        {
            m_AssetInfo = assetInfo;
            m_SceneName = System.IO.Path.GetFileNameWithoutExtension(assetInfo.AssetPath);
        }

        protected LoadSceneMode m_LoadSceneMode;
        internal AssetInfo m_AssetInfo;
        protected string m_SceneName;
    }
}
