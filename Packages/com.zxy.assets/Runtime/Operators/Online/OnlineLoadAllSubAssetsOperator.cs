using System;
using System.Collections.Generic;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineLoadAllSubAssetsOperator : BaseLoadAllAssetsOperator
    {
        internal OnlineLoadAllSubAssetsOperator(IAssetsSystemImpl impl, List<KeyValuePair<string, Type>> valuePairs)
        {
            m_Impl = impl;
            m_WaitLoadValuPairs = new List<KeyValuePair<string, Type>>(valuePairs.Count);
            m_WaitLoadValuPairs.AddRange(valuePairs);
        }

        internal OnlineLoadAllSubAssetsOperator(IAssetsSystemImpl impl, IEnumerable<string> collection, Type type)
        {
            m_Impl = impl;
            m_WaitLoadValuPairs = new List<KeyValuePair<string, Type>>(16);
            var en = collection.GetEnumerator();
            while (en.MoveNext())
            {
                m_WaitLoadValuPairs.Add(new KeyValuePair<string, Type>(en.Current, type));
            }
        }

        protected override void OnExecute()
        {
            if (m_LoadAssetOpera != null)
            {
                Progress = m_LoadAssetOpera.Progress + m_CurrentLoadedCnt / m_TotalNeedLoadCnt;
                if (!m_LoadAssetOpera.IsDone) return;

                m_LoadedOperatorQueue.Enqueue(m_LoadAssetOpera);

                if (m_LoadAssetOpera.Status == OperatorStatus.Success)
                {
                    AllAssetsObject[m_CurrentLoadedCnt] = m_LoadAssetOpera.AssetObject;
                    m_CurrentLoadedCnt += 1;
                    m_LoadAssetOpera = null;
                }
                else
                {
                    Error = m_LoadAssetOpera.Error;
                    Status = OperatorStatus.Failed;
                    m_LoadAssetOpera = null;
                }
            }
            else
            {
                if (m_WaitLoadValuPairs.Count == 0)
                {
                    Status = OperatorStatus.Success;
                }
                else
                {
                    var value = m_WaitLoadValuPairs[0];
                    m_WaitLoadValuPairs.RemoveAt(0);
                    m_LoadAssetOpera = m_Impl.LoadAssetAsync(value.Key, value.Value);
                }
            }
        }

        protected override void OnDispose()
        {

            if (m_WaitLoadValuPairs != null)
                m_WaitLoadValuPairs.Clear();
            m_WaitLoadValuPairs = null;

            if (m_LoadedOperatorQueue != null)
            {
                while (m_LoadedOperatorQueue.Count > 0)
                {
                    var op = m_LoadedOperatorQueue.Dequeue();
                    op.Dispose();
                }
                m_LoadedOperatorQueue = null;
            }

            AllAssetsObject = null;

            m_LoadAssetOpera = null;

            m_Impl = null;
        }

        protected override void OnStart()
        {
            if (m_WaitLoadValuPairs == null || m_WaitLoadValuPairs.Count == 0)
                Status = OperatorStatus.Success;
            else
            {
                m_TotalNeedLoadCnt = m_WaitLoadValuPairs.Count;
                m_CurrentLoadedCnt = 0;
                AllAssetsObject = new UnityEngine.Object[m_TotalNeedLoadCnt];
                m_LoadedOperatorQueue = new Queue<BaseLoadAssetOperator>(m_TotalNeedLoadCnt);

                var value = m_WaitLoadValuPairs[0];
                m_WaitLoadValuPairs.RemoveAt(0);
                m_LoadAssetOpera = m_Impl.LoadAssetAsync(value.Key, value.Value);
            }
        }

        private Queue<BaseLoadAssetOperator> m_LoadedOperatorQueue;
        private BaseLoadAssetOperator m_LoadAssetOpera;
        private IAssetsSystemImpl m_Impl;
        private List<KeyValuePair<string, Type>> m_WaitLoadValuPairs;
        private int m_TotalNeedLoadCnt = 0;
        private int m_CurrentLoadedCnt = 0;

    }
}