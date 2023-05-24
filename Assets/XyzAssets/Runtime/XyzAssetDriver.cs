
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal class XyzAssetDriver : MonoBehaviour
    {
        internal static void TryCreateInstance()
        {
            if (_mInstance == null)
            {
                var g = new GameObject("XyzAssetDriver");
                DontDestroyOnLoad(g);
                _mInstance = g.AddComponent<XyzAssetDriver>();
            }
        }
        private static XyzAssetDriver _mInstance;

        private void Update()
        {
            XyzAsset.Execute();
        }

        private void OnDestroy()
        {
            XyzAsset.Dispose();
        }
    }

}
