using System.IO;
using System.Collections.Generic;
namespace XyzAssets.Runtime
{
    sealed internal class BundleStreamManager
    {
        class AssetBundleStreamRef
        {
            internal Stream stream;
            internal int refCnt;
        }

        internal static Stream GetStream(string fileName)
        {
            if (_mAssetBundleStreamDict.ContainsKey(fileName))
            {
                _mAssetBundleStreamDict[fileName].refCnt++;
                return _mAssetBundleStreamDict[fileName].stream;
            }

            else
            {
                AssetBundleStream bundleStream = new AssetBundleStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                _mAssetBundleStreamDict.Add(fileName, new AssetBundleStreamRef() { stream = bundleStream, refCnt = 1 });
                return bundleStream;
            }
        }

        internal static void ReleaseStream(string fileName)
        {
            if (_mAssetBundleStreamDict.ContainsKey(fileName))
            {
                _mAssetBundleStreamDict[fileName].refCnt--;
            }
        }
        internal static void Update()
        {
            foreach (var item in _mAssetBundleStreamDict)
            {
                if (item.Value.refCnt <= 0)
                    _mRemoveIndex.Add(item.Key);
            }
            for (int i = _mRemoveIndex.Count - 1; i >= 0; i--)
            {
                var key = _mRemoveIndex[i];
                var rf = _mAssetBundleStreamDict[key];
                rf.stream.Dispose();
                _mAssetBundleStreamDict.Remove(key);
            }
            _mRemoveIndex.Clear();
        }


        private readonly static Dictionary<string, AssetBundleStreamRef> _mAssetBundleStreamDict = new Dictionary<string, AssetBundleStreamRef>();
        private readonly static List<string> _mRemoveIndex = new List<string>();
    }
}