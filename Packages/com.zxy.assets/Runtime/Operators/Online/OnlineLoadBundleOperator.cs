using System.IO;
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineLoadBundleOperator : LoadBundleOperator
    {
        private enum EStep
        {
            None,

            VerifyExternal,
            LoadFromExternal,

            VerifyStreaming,
            LoadFromStreaming,

            WaitLoadCompletion,
        }

        internal OnlineLoadBundleOperator(int bundleId, bool async) : base(bundleId, async)
        {
        }
        protected override void OnExecute()
        {
            if (_step == EStep.VerifyExternal)
            {
                if (VerifyExternal())
                    _step = EStep.LoadFromExternal;
                else
                    _step = EStep.VerifyStreaming;
            }
            else if (_step == EStep.LoadFromExternal)
            {
                LoadBundle(AssetsPathHelper.ExternalPath);
            }
            else if (_step == EStep.VerifyStreaming)
            {
                if (VerifyStreaming())
                    _step = EStep.LoadFromStreaming;
                else
                {
                    //
                    Error = "Load bundle error. File did not found. " + _mBundleName;
                    Status = EOperatorStatus.Failed;
                    InvokeCompletion();
                }
            }
            else if (_step == EStep.LoadFromStreaming)
            {
                LoadBundle(Application.streamingAssetsPath);
            }
            else if (_step == EStep.WaitLoadCompletion)
            {
                Progress = _mRequest.progress;
                if (!_mRequest.isDone) return;
                if (_mRequest.assetBundle == null)
                {
                    Error = "Load Bundle error.";
                    Status = EOperatorStatus.Failed;
                    InvokeCompletion();
                }
                else
                {
                    BundleSystem.AddAssetBundle(MainBundleId, _mRequest.assetBundle);
                    Status = EOperatorStatus.Succeed;
                    InvokeCompletion();
                }
            }
        }

        private bool VerifyExternal()
        {
            if (File.Exists(AssetsPathHelper.GetFileExternalPath(_mBundleName)))
                return true;
            else
                return false;
        }

        private bool VerifyStreaming()
        {
            if (StreamingAssetsHelper.FileExists(_mBundleName))
                return true;
            else
                return false;
        }


        private void LoadBundle(string bundleRoot)
        {
            var bundlePath = Path.Combine(bundleRoot, _mBundleName);
            if (_mBundleInfo.EncryptType == BundleEncryptType.None || _mBundleInfo.EncryptType == BundleEncryptType.FileOffset)
            {
                if (_mAsync)
                {
                    _mRequest = AssetBundle.LoadFromFileAsync(bundlePath, _mBundleInfo.Crc, _mBundleInfo.Offset);
                    _step = EStep.WaitLoadCompletion;
                }
                else
                {
                    var bundle = AssetBundle.LoadFromFile(bundlePath, _mBundleInfo.Crc, _mBundleInfo.Offset);
                    if (bundle == null)
                    {
                        Error = $"Load Bundle error. path:{bundlePath} crc:{_mBundleInfo.Crc}";
                        Status = EOperatorStatus.Failed;
                        InvokeCompletion();
                    }
                    else
                    {
                        BundleSystem.AddAssetBundle(MainBundleId, bundle);
                        Status = EOperatorStatus.Succeed;
                        InvokeCompletion();
                    }
                }
            }
            else if (_mBundleInfo.EncryptType == BundleEncryptType.Stream)
            {
                if (_mAsync)
                {
                    var stream = BundleStreamManager.GetStream(bundlePath);
                    _mRequest = AssetBundle.LoadFromStreamAsync(stream, _mBundleInfo.Crc);
                    _step = EStep.WaitLoadCompletion;
                }
                else
                {

                    var stream = BundleStreamManager.GetStream(bundlePath);
                    var bundle = AssetBundle.LoadFromStream(stream, _mBundleInfo.Crc);
                    if (bundle == null)
                    {
                        Error = $"Load Bundle error. path:{bundlePath} crc:{_mBundleInfo.Crc}";
                        Status = EOperatorStatus.Failed;
                        InvokeCompletion();
                    }
                    else
                    {
                        BundleSystem.AddAssetBundle(MainBundleId, bundle);
                        Status = EOperatorStatus.Succeed;
                        InvokeCompletion();
                    }
                }
            }
        }

        protected override void InvokeCompletion()
        {
            base.InvokeCompletion();
            _mRequest = null;
            _mBundleInfo = null;
            _mBundleName = null;

        }

        protected override void OnStart()
        {
            _mBundleInfo = AssetSystem.GetBundleInfo(MainBundleId);
            _mBundleName = _mBundleInfo.NameType == BundleFileNameType.BundleName ? _mBundleInfo.BundleName : _mBundleInfo.Version;
            _step = EStep.VerifyExternal;
        }


        private EStep _step = EStep.None;
        private string _mBundleName;
        private AssetBundleCreateRequest _mRequest;
        private BundleInfo _mBundleInfo;
    }
}
