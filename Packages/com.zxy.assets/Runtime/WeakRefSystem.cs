using System.Collections.Generic;

namespace XyzAssets.Runtime
{
    internal static class WeakRefSystem
    {
        private class WeakRefData
        {
            internal UnityEngine.Object holder;
            internal AsyncOperationBase tartget;
        }

        internal static void AddWeakReference(UnityEngine.Object holder, AsyncOperationBase target)
        {
            m_WeakReferences.Add(new WeakRefData() { holder = holder, tartget = target });
        }

        internal static void Execute()
        {
            for (int i = m_WeakReferences.Count - 1; i >= 0; i--)
            {
                if (m_WeakReferences[i].holder == null)
                {
                    m_WeakReferences[i].tartget.Dispose();
                    m_WeakReferences.RemoveAt(i);
                }
            }
        }
        internal static void Dispose()
        {
            m_WeakReferences.Clear();
            m_RemoveList.Clear();
            XyzLogger.Log("XyzWeakRefSystem Dispose");
        }

        private readonly static List<WeakRefData> m_WeakReferences = new List<WeakRefData>();
        private readonly static List<int> m_RemoveList = new List<int>();


    }

}
