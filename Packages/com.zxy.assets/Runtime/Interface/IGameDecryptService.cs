using System;
using System.IO;

namespace XyzAssets.Runtime
{
    public interface IGameDecryptService
    {
        int GetFileOffset(string bundleName);
        Stream GetDecryptStream(string bundleName);
    }
}