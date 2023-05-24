using System.IO;
using System.Collections.Generic;

namespace XyzAssets.Editor
{
    [System.Serializable]
    public class XyzAssetsEditorManifest
    {
        public List<EditorBundleInfo> BundleInfos = new List<EditorBundleInfo>();
        public List<EditorAssetInfo> AssetInfos = new List<EditorAssetInfo>();

        public void AddBundleInfo(EditorBundleInfo info)
        {
            BundleInfos.Add(info);
        }

        public void AddAssetInfo(EditorAssetInfo info)
        {
            AssetInfos.Add(info);
        }


        #region Read and write

        public void SerializeToBinary(BinaryWriter writer)
        {
            writer.Write(BundleInfos.Count);
            for (int i = 0; i < BundleInfos.Count; i++)
            {
                var item = BundleInfos[i];

                writer.Write(item.BundleName);
                writer.Write(item.Version);
                writer.Write(item.Crc);
                writer.Write(item.FileSize);
                writer.Write(item.ModeName);
                writer.Write((byte)item.EncryptType);
                writer.Write((byte)item.NameType);

            }

            writer.Write(AssetInfos.Count);
            for (int i = 0; i < AssetInfos.Count; i++)
            {
                var item = AssetInfos[i];
                writer.Write(item.AssetPath);
                writer.Write(FindBundleIndex(item.MainBundle));
                writer.Write(item.Dependencies.Length);
                for (int m = 0; m < item.Dependencies.Length; m++)
                {
                    writer.Write(FindBundleIndex(item.Dependencies[m]));
                }
            }
        }
        public byte[] SerializeToBinary()
        {
            var ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            SerializeToBinary(writer);
            var result = ms.ToArray();
            writer.Dispose();
            writer.Close();
            return result;
        }

        public string SerializeToJson()
        {
            return UnityEngine.JsonUtility.ToJson(this);
        }

        #endregion

        private int FindBundleIndex(string bundleName)
        {
            for (int i = 0; i < BundleInfos.Count; i++)
            {
                var item = BundleInfos[i];
                if (item.BundleName == bundleName)
                    return i;
            }
            return -1;
        }
    }

}
