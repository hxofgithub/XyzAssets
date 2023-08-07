using System;
using System.Collections;
using System.Threading.Tasks;

namespace XyzAssets.Runtime
{
    

    public abstract class AsyncOperationBase : IEnumerator
    {

        private Action<AsyncOperationBase> _callback;

        public event Action<float> OnProgress;

        public bool IsDone { get => Status == EOperatorStatus.Success || Status == EOperatorStatus.Failed; }
        public EOperatorStatus Status { get; protected set; } = EOperatorStatus.Success;

        public string Error { get; protected set; }

        public float Progress { get; protected set; }

        public event Action<AsyncOperationBase> Completed
        {
            add
            {
                if (IsDone)
                    value.Invoke(this);
                else
                    _callback += value;
            }
            remove
            {
                _callback -= value;
            }
        }

        public Task Task
        {
            get
            {
                if (_taskCompletionSource == null)
                {
                    _taskCompletionSource = new TaskCompletionSource<object>();
                    if (IsDone)
                        _taskCompletionSource.SetResult(null);
                }
                return _taskCompletionSource.Task;
            }
        }

        internal abstract void Start();
        internal abstract void Update();
        internal void Finish()
        {
            Progress = 1f;
            _callback?.Invoke(this);
            if (_taskCompletionSource != null)
                _taskCompletionSource.TrySetResult(null);
        }

        protected void ClearCompletedCallback()
        {
            _callback = null;
        }


        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }

        void IEnumerator.Reset()
        {

        }
        object IEnumerator.Current => null;
        private TaskCompletionSource<object> _taskCompletionSource;
    }
}