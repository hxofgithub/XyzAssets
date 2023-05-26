
using System.IO;

namespace XyzAssets.Runtime
{
    internal static class ManifestSerialize
    {
        #region Deserialize
        internal static XyzAssetsRuntimeManifest DeserializeFromBinary(byte[] buffer)
        {
            XyzAssetsRuntimeManifest result;

            using (var ms = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(ms, System.Text.Encoding.UTF8))
                {
                    result = DeserializeFromReader(reader);
                    reader.Close();
                }
                ms.Close();
            }
            return result;
        }

        internal static XyzAssetsRuntimeManifest DeserializeFromReader(BinaryReader reader)
        {
            XyzAssetsRuntimeManifest inst = new XyzAssetsRuntimeManifest();
            int len = reader.ReadInt32();
            inst.BundleList = new BundleInfo[len];
            for (int i = 0; i < len; i++)
            {
                var bundleInfo = new BundleInfo()
                {
                    BundleName = reader.ReadString(),
                    Version = reader.ReadString(),
                    Crc = reader.ReadUInt32(),
                    FileSize = reader.ReadInt64(),
                    ModeName = reader.ReadString(),
                    EncryptType = (BundleEncryptType)reader.ReadByte(),
                    NameType = (BundleFileNameType)reader.ReadByte(),
                };


                inst.BundleList[i] = bundleInfo;
            }
            len = reader.ReadInt32();
            inst.AssetInfoList = new AssetInfo[len];
            for (int i = 0; i < len; i++)
            {
                var assetInfo = new AssetInfo()
                {
                    AssetPath = reader.ReadString(),
                    MainId = reader.ReadInt32(),
                    Dependencies = new int[reader.ReadInt32()],
                };
                for (int m = 0; m < assetInfo.Dependencies.Length; m++)
                {
                    assetInfo.Dependencies[m] = reader.ReadInt32();
                }
                inst.AssetInfoList[i] = assetInfo;
            }
            return inst;

        }
        #endregion

        #region Serialize
        internal static byte[] SerializeToBinary(XyzAssetsRuntimeManifest manifest)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {

                    SerializeToWriter(manifest, writer);
                    result = ms.ToArray();
                    writer.Close();
                }
                ms.Close();
            }
            return result;
        }

        internal static void SerializeToWriter(XyzAssetsRuntimeManifest manifest, BinaryWriter writer)
        {
            writer.Write(manifest.BundleList.Length);
            for (int i = 0; i < manifest.BundleList.Length; i++)
            {
                var item = manifest.BundleList[i];

                writer.Write(item.BundleName);
                writer.Write(item.Version);
                writer.Write(item.Crc);
                writer.Write(item.FileSize);
                writer.Write(item.ModeName);
                writer.Write((byte)item.EncryptType);
                writer.Write((byte)item.NameType);
            }

            writer.Write(manifest.AssetInfoList.Length);
            for (int i = 0; i < manifest.AssetInfoList.Length; i++)
            {
                var item = manifest.AssetInfoList[i];
                writer.Write(item.AssetPath);
                writer.Write(item.MainId);
                writer.Write(item.Dependencies.Length);
                for (int m = 0; m < item.Dependencies.Length; m++)
                {
                    writer.Write(item.Dependencies[m]);
                }
            }
        }
        #endregion

    }
}