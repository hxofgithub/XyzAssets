
using XyzAssets.Runtime;
using System.Collections.Generic;
using System.IO;

namespace XyzAssets.Editor
{
    public static class ResourceSystemEncryptTools
    {
        public static void EncryptItem(string srcPath, string dstPath, BundleEncryptType encryptType)
        {
            IGameDecryptService service = null;
            if (encryptType == BundleEncryptType.Stream)
            {
                using (var dstStream = service.GetEncryptStream(dstPath))
                {
                    var buffer = File.ReadAllBytes(srcPath);
                    dstStream.Write(buffer, 0, buffer.Length);
                    dstStream.Dispose();
                }
            }
            else if (encryptType == BundleEncryptType.FileOffset)
            {
                var fileName = Path.GetFileName(srcPath);
                var datas = File.ReadAllBytes(srcPath);

                var header = new byte[service.GetFileOffset(fileName)];
                for (int i = 0; i < header.Length; i++)
                {
                    header[i] = (byte)(i * header.Length + i);
                }
                var m_datas = new byte[datas.Length + header.Length];
                System.Array.Copy(header, m_datas, header.Length);
                System.Array.Copy(datas, 0, m_datas, header.Length, datas.Length);
                File.WriteAllBytes(dstPath, m_datas);
            }
            else
            {
                File.Copy(srcPath, dstPath, true);
            }

        }

        public static void EncryptBuffer(string dstPath, byte[] buffer, BundleEncryptType encryptType)
        {
            if (string.IsNullOrEmpty(dstPath)) return;
            if (buffer == null || buffer.Length == 0) return;


            IGameDecryptService service = null;

            if (encryptType == BundleEncryptType.Stream)
            {
                using (var dstStream = service.GetEncryptStream(dstPath))
                {
                    dstStream.Write(buffer, 0, buffer.Length);
                    dstStream.Dispose();
                }
            }
            else if (encryptType == BundleEncryptType.FileOffset)
            {
                var fileName = Path.GetFileName(dstPath);
                var header = new byte[service.GetFileOffset(fileName)];
                for (int i = 0; i < header.Length; i++)
                {
                    header[i] = (byte)(i * header.Length + i);
                }
                var m_datas = new byte[buffer.Length + header.Length];
                System.Array.Copy(header, m_datas, header.Length);
                System.Array.Copy(buffer, 0, m_datas, header.Length, buffer.Length);
                File.WriteAllBytes(dstPath, m_datas);
            }
            else
            {
                File.WriteAllBytes(dstPath, buffer);
            }
        }

        public static void EncryptString(string dstPath, string data, BundleEncryptType encryptType)
        {
            if (string.IsNullOrEmpty(dstPath)) return;
            if (string.IsNullOrEmpty(data)) return;


            IGameDecryptService service = null;

            if (encryptType == BundleEncryptType.Stream)
            {
                using (var dstStream = service.GetEncryptStream(dstPath))
                {
                    var buffer = System.Text.Encoding.UTF8.GetBytes(data);
                    dstStream.Write(buffer, 0, buffer.Length);
                    dstStream.Dispose();
                }
            }
            else if (encryptType == BundleEncryptType.FileOffset)
            {
                if (string.IsNullOrEmpty(dstPath)) return;
                if (string.IsNullOrEmpty(data)) return;

                var fileName = Path.GetFileName(dstPath);
                var buffer = System.Text.Encoding.UTF8.GetBytes(data);

                var header = new byte[service.GetFileOffset(fileName)];
                for (int i = 0; i < header.Length; i++)
                {
                    header[i] = (byte)(i * header.Length + i);
                }
                var m_datas = new byte[buffer.Length + header.Length];
                System.Array.Copy(header, m_datas, header.Length);
                System.Array.Copy(buffer, 0, m_datas, header.Length, buffer.Length);
                File.WriteAllBytes(dstPath, m_datas);
            }
            else
            {
                File.WriteAllText(dstPath, data);
            }

        }

        public static void EncryptDirectoryItem(string srcDirPath, string dstDirPath, IList<string> excludeNames, BundleEncryptType encryptType)
        {

            if (!Directory.Exists(srcDirPath))
                throw new System.Exception("srcDirPath is error.  " + srcDirPath);

            if (!Directory.Exists(dstDirPath))
                Directory.CreateDirectory(dstDirPath);

            var files = Directory.GetFiles(srcDirPath, "*.*", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                var fileName = Path.GetFileName(item);
                if (excludeNames.IndexOf(fileName) == -1)
                {
                    var newPath = item.Replace("\\", "/");
                    EncryptItem(item, newPath.Replace(srcDirPath, dstDirPath), encryptType);
                }
            }
        }
    }
}
