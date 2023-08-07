using System.Collections.Generic;
using System.Diagnostics;
namespace XyzAssets.Runtime
{
    internal class XyzOperatorSystem
    {
        internal static bool IsBusy
        {
            get
            {
                return _watch.ElapsedMilliseconds - _lastFrameTime >= MaxTimeSlice;
            }
        }
        internal static long MaxTimeSlice { get; set; } = long.MaxValue;

        internal static void Initialize()
        {
            _watch = Stopwatch.StartNew();
        }

        internal static void Execute()
        {
            if (_watch == null) return;

            _lastFrameTime = _watch.ElapsedMilliseconds;

            if (_addList.Count > 0)
            {
                _operations.AddRange(_addList);
                _addList.Clear();
            }

            for (int i = 0, cnt = _operations.Count; i < cnt; i++)
            {
                if (IsBusy) break;
                var operation = _operations[i];
                operation.Update();
                if (operation.IsDone)
                {
                    _removeList.Add(i);
                    operation.Finish();
                }                    
            }

            if (_removeList.Count > 0)
            {
                for (int i = _removeList.Count - 1; i >= 0; i--)
                {
                    _operations.RemoveAt(_removeList[i]);
                }
                _removeList.Clear();
            }
        }

        public static void StartOperation(AsyncOperationBase operation)
        {
            _addList.Add(operation);
            operation.Start();
        }

		public static void DestroyAll()
        {
            _operations.Clear();
            _addList.Clear();
            _removeList.Clear();
            _watch = null;
            _lastFrameTime = 0;
            MaxTimeSlice = long.MaxValue;
        }


        private static readonly List<AsyncOperationBase> _operations = new List<AsyncOperationBase>(100);
        private static readonly List<AsyncOperationBase> _addList = new List<AsyncOperationBase>(100);
        private static readonly List<int> _removeList = new List<int>(100);

        private static Stopwatch _watch;
        private static long _lastFrameTime = 0;
    }
}