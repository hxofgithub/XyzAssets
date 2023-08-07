using System.Collections.Generic;

namespace XyzAssets.Runtime
{
    public abstract class ProviderBase
    {
        public enum EStatus
        {
            None = 0,
            CheckBundle,
            Loading,
            Checking,
            Succeed,
            Failed,
        }

        /// <summary>
		/// 资源提供者唯一标识符
		/// </summary>
		public string ProviderGUID { private set; get; }

        /// <summary>
		/// 所属资源系统
		/// </summary>
		public AssetSystemImpl Impl { private set; get; }

        /// <summary>
        /// 资源信息
        /// </summary>
        public AssetInfo MainAssetInfo { private set; get; }

        /// <summary>
		/// 获取的资源对象
		/// </summary>
		public UnityEngine.Object AssetObject { protected set; get; }

        /// <summary>
        /// 获取的资源对象集合
        /// </summary>
        public UnityEngine.Object[] AllAssetObjects { protected set; get; }

        /// <summary>
		/// 获取的场景对象
		/// </summary>
		public UnityEngine.SceneManagement.Scene SceneObject { protected set; get; }

        /// <summary>
		/// 原生文件路径
		/// </summary>
		public string RawFilePath { protected set; get; }

        /// <summary>
		/// 当前的加载状态
		/// </summary>
        public EStatus Status { get; protected set; } = EStatus.None;
        /// <summary>
		/// 最近的错误信息
		/// </summary>
        public string Error { get; protected set; }
        /// <summary>
		/// 加载进度
		/// </summary>
        public float Progress { get; protected set; } = 0f;
        /// <summary>
		/// 引用计数
		/// </summary>
		public int RefCount { private set; get; } = 0;
        /// <summary>
		/// 是否已经销毁
		/// </summary>
        internal bool IsDestroyed { get; private set; } = false;
        /// <summary>
		/// 是否完毕（成功或失败）
		/// </summary>
        internal bool IsDone { get { return Status == EStatus.Succeed || Status == EStatus.Failed; } }

        public ProviderBase(AssetSystemImpl impl, string providerGUID, AssetInfo assetInfo)
        {
            Impl = impl;
            ProviderGUID = providerGUID;
            MainAssetInfo = assetInfo;
            _handles = new List<OperationHandleBase>();
        }


        public abstract void Update();

        public void Destroy()
        {
            IsDestroyed = true;

        }

        internal bool CanDestroy()
        {
            if (IsDone == false) return false;
            return RefCount <= 0;
        }

        public bool IsSceneProvider()
        {
            return false;
        }

        public T CreateHandle<T>() where T : OperationHandleBase
        {
            RefCount++;

            OperationHandleBase handle;
            //if (typeof(T) == typeof(AssetOperationHandle))
            //    handle = new AssetOperationHandle(this);
            //else if (typeof(T) == typeof(SceneOperationHandle))
            //    handle = new SceneOperationHandle(this);
            //else if (typeof(T) == typeof(SubAssetsOperationHandle))
            //    handle = new SubAssetsOperationHandle(this);
            //else if (typeof(T) == typeof(AllAssetsOperationHandle))
            //    handle = new AllAssetsOperationHandle(this);
            //else if (typeof(T) == typeof(RawFileOperationHandle))
            //    handle = new RawFileOperationHandle(this);
            //else
                throw new System.NotImplementedException();

            _handles.Add(handle);
            return handle as T;
        }


        public System.Threading.Tasks.Task Task
        {
            get { return null; }
        }

        internal void ReleaseHandle(OperationHandleBase handle)
        {
            if(_handles.Remove(handle))
            {
                RefCount--;
            }                
        }

        public void WaitForAsyncComplete()
        {
            IsWaitForAsyncComplete = true;
            Update();
        }

        protected void ProcessCacheBundleException()
        {

        }


        protected bool IsWaitForAsyncComplete { get; private set; } = false;
        private List<OperationHandleBase> _handles;

    }
}
