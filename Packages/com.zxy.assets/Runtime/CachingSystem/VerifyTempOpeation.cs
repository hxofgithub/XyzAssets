
using System.IO;

namespace XyzAssets.Runtime
{
    internal class VerifyTempOpeation : AsyncOperationBase
    {
        private enum EStep
        {
            None,
            CheckFileExist,
            CheckFileSize,
            CheckVersion,
            Done,

        }

        public EVerifyResult Result { get; private set; } = EVerifyResult.None;

        public long FileSize { get; private set; } = 0;

        private EStep _step = EStep.None;
        protected override void OnDispose()
        {
            _step = EStep.None;
        }

        protected override void OnExecute()
        {
            if (_step == EStep.None) return;
            if (IsDone) return;

            if (_step == EStep.CheckFileExist)
            {
                FileSize = 0;
                if (!File.Exists(_filePath))
                {
                    Result = EVerifyResult.CacheNotFound;
                    Status = EOperatorStatus.Failed;
                }
                else
                    _step = EStep.CheckFileSize;
            }
            else if (_step == EStep.CheckFileSize)
            {
                FileInfo info = new FileInfo(_filePath);
                var len = info.Length;
                FileSize = len;
                if (_verifyFileSize > len)
                {
                    Result = EVerifyResult.FileNotComplete;
                    Status = EOperatorStatus.Failed;
                }
                else if (_verifyFileSize < len)
                {
                    Result = EVerifyResult.FileOverflow;
                    Status = EOperatorStatus.Failed;
                }
                else
                {
                    _step = EStep.CheckVersion;
                }
            }
            else if (_step == EStep.CheckVersion)
            {
                var version = AssetsUtility.CalculateMD5(_filePath);
                if (version == _version)
                {
                    Result = EVerifyResult.Succeed;
                    Status = EOperatorStatus.Succeed;
                }
                else
                {
                    Result = EVerifyResult.FileCrcError;
                    Status = EOperatorStatus.Failed;
                }
            }
        }

        protected override void OnStart()
        {
            _step = EStep.CheckFileExist;
        }

        public static VerifyTempOpeation Create(string filePath, string version, long fileSize)
        {
            var element = new VerifyTempOpeation
            {
                _filePath = filePath,
                _version = version,
                _verifyFileSize = fileSize,
                Result = EVerifyResult.None,
                FileSize = 0
            };
            return element;

        }

        private string _filePath;
        private string _version;
        private long _verifyFileSize;
    }
}
