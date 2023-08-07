
using XyzAssets.Runtime;
using System.IO;

namespace XyzAssets.Editor
{
    public static class ResourceSystemEncryptTools
    {
        public static EncryptOutput EncryptItem(string srcPath, IGameEncryptService service = null)
        {
            if (service != null)
            {
                return service.Encrypt(srcPath);
            }
            else
            {
                return new EncryptOutput()
                {
                    Buffer = File.ReadAllBytes(srcPath),
                    EncryptType = BundleEncryptType.None
                };
            }
        }

    }
}
