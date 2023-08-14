using System;
using System.Collections;

namespace XyzAssets.Runtime
{
    public abstract class AsyncOperationBase : IEnumerator, IDisposable
    {
        public AsyncOperationBase(bool autoAddToOpSystem)
        {
            if (autoAddToOpSystem)
                OperatorSystem.AddAssetOperator(this);
        }

        public AsyncOperationBase() : this(true) { }

        public bool IsDone { get => Status == EOperatorStatus.Succeed || Status == EOperatorStatus.Failed; }
        public EOperatorStatus Status { get; protected set; }

        public string Error { get; protected set; }

        public float Progress { get; protected set; }

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

            if (IsDone)
                InvokeCompletion();
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

        protected virtual void InvokeCompletion() { }

        #region IEnumerator
        bool IEnumerator.MoveNext() => !IsDone;
        object IEnumerator.Current => null;
        void IEnumerator.Reset() { }


        #endregion

        private bool m_IsDisposed;
        private bool m_IsStarted;
    }
}