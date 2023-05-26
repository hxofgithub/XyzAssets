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
                return m_Watch.ElapsedMilliseconds - m_LastFrameTime >= MaxTimeSlice;
            }
        }
        internal static long MaxTimeSlice { get; set; } = long.MaxValue;

        internal static void Initialize()
        {
            m_Watch = Stopwatch.StartNew();
            m_Initialize = true;
        }

        internal static void AddAssetOperator(ResourceBaseOperator op)
        {
            m_AssetOperations.Add(op);
        }

        internal static void Dispose()
        {
            m_Initialize = false;
            m_AssetOperations.Clear();
            m_RemoveIndex.Clear();
            XyzLogger.Log("XyzOperatorSystem Dispose");
        }

        internal static void Execute()
        {
            if (!m_Initialize) return;

            m_RemoveIndex.Clear();

            m_LastFrameTime = m_Watch.ElapsedMilliseconds;
            for (int i = 0; i < m_AssetOperations.Count; i++)
            {
                if (IsBusy)
                    break;

                var opera = m_AssetOperations[i];
                opera.Execute();
                if (opera.IsDone)
                    m_RemoveIndex.Add(i);
            }
            for (int i = m_RemoveIndex.Count - 1; i >= 0; i--)
            {
                var index = m_RemoveIndex[i];
                m_AssetOperations.RemoveAt(index);
            }
        }

        private static readonly List<ResourceBaseOperator> m_AssetOperations = new List<ResourceBaseOperator>(128);
        private static readonly List<int> m_RemoveIndex = new List<int>();
        private static Stopwatch m_Watch;
        private static long m_LastFrameTime = 0;
        private static bool m_Initialize = false;
    }
}