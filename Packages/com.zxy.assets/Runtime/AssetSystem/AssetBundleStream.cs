using System.IO;

namespace XyzAssets.Runtime
{
    public sealed class AssetBundleStream : FileStream
    {
        const byte KEY = 142;
        public AssetBundleStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
            : base(path, mode, access, share, bufferSize, useAsync)
        {

        }

        public AssetBundleStream(string path, FileMode mode) : base(path, mode)
        {

        }
        public AssetBundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {

        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);

            for (int i = 0; i < index; i++)
            {
                array[i] ^= KEY;
            }
            return index;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                array[i] ^= KEY;
            }
            base.Write(array, offset, count);
        }

    }
}
