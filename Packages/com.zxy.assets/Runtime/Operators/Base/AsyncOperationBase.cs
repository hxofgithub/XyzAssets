using System;
using System.Collections;

namespace XyzAssets.Runtime
{
    public abstract class AsyncOperationBase : IEnumerator, IDisposable
    {
        public AsyncOperationBase(bool autoAddToOpSystem)
        {
            if (autoAddToOpSystem)
                XyzOperatorSystem.AddAssetOperator(this);
        }

        public AsyncOperationBase() : this(true) { }

        public event Action<float> OnProgress;

        public bool IsDone { get => Status == EOperatorStatus.Succeed || Status == EOperatorStatus.Failed; }
        public virtual EOperatorStatus Status
        {
            get => m_Status;
            protected set
            {
                m_Status = value;
            }
        }

        public string Error { get; protected set; }

        public float Progress
        {
            get => m_Progress;
            protected set
            {
                if (m_Progress != value)
                {
                    m_Progress = value;
                    OnProgress?.Invoke(value);
                }
            }
        }
        private float m_Progress;

        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;
                OnDispose();
            }
        }
        public void Execute()
        {
            if (IsDone) return;
            if (m_IsDisposed) return;
            if (m_IsStarted)
                OnExecute();
            else
                Start();
        }

        private void Start()
        {
            if (Status != EOperatorStatus.None) return;

            Progress = 0;
            m_IsStarted = true;
            OnStart();
        }


        protected abstract void OnDispose();
        protected abstract void OnExecute();
        protected abstract void OnStart();

        #region IEnumerator
        bool IEnumerator.MoveNext() => !IsDone;
        object IEnumerator.Current => null;
        void IEnumerator.Reset() { }


        #endregion

        protected bool m_IsDisposed { get; private set; }
        private bool m_IsStarted;
        private EOperatorStatus m_Status;
    }
}