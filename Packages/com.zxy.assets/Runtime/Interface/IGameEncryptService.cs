
namespace XyzAssets.Runtime
{
    public struct EncryptOutput
    {
        public BundleEncryptType EncryptType;
        public byte[] Buffer;
    }

    public interface IGameEncryptService
    {
        public EncryptOutput Encrypt(string path);
    }
}