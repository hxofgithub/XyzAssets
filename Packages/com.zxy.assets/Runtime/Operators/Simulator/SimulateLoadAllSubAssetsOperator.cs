using System;
using System.Collections.Generic;

namespace XyzAssets.Runtime
{
    internal sealed class SimulateLoadAllSubAssetsOperator : BaseLoadAllAssetsOperator
    {
        internal SimulateLoadAllSubAssetsOperator(List<KeyValuePair<string, Type>> valuePairs)
        {
#if UNITY_EDITOR
            m_WaitLoadValuPairs = new List<KeyValuePair<string, Type>>(valuePairs.Count);
            m_WaitLoadValuPairs.AddRange(valuePairs);
#endif
        }

        internal SimulateLoadAllSubAssetsOperator(IEnumerable<string> collection, Type type)
        {

#if UNITY_EDITOR
            m_WaitLoadValuPairs = new List<KeyValuePair<string, Type>>(16);
            var en = collection.GetEnumerator();
            while (en.MoveNext())
            {
                m_WaitLoadValuPairs.Add(new KeyValuePair<string, Type>(en.Current, type));
            }
#endif
        }

        protected override void OnExecute()
        {
#if UNITY_EDITOR
            if (m_CurrentLoadedCnt < m_TotalNeedLoadCnt)
            {
                var data = m_WaitLoadValuPairs[m_CurrentLoadedCnt];
                var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(data.Key, data.Value);
                if (obj == null)
                {
                    Error = StringUtility.Format("Failed Load:{0} - {1}", data.Key, data.Value);
                    Status = EOperatorStatus.Failed;
                }
                else
                {
                    AllAssetsObject[m_CurrentLoadedCnt] = obj;
                    m_CurrentLoadedCnt++;
                }
            }
            else
            {
                Status = EOperatorStatus.Success;
            }

#endif
        }

        protected override void OnDispose()
        {

#if UNITY_EDITOR
            if (m_WaitLoadValuPairs != null)
                m_WaitLoadValuPairs.Clear();
            m_WaitLoadValuPairs = null;

            AllAssetsObject = null;
#endif
        }

        protected override void OnStart()
        {
#if UNITY_EDITOR
            if (m_WaitLoadValuPairs == null || m_WaitLoadValuPairs.Count == 0)
                Status = EOperatorStatus.Success;
            else
            {
                m_TotalNeedLoadCnt = m_WaitLoadValuPairs.Count;
                m_CurrentLoadedCnt = 0;
                AllAssetsObject = new UnityEngine.Object[m_TotalNeedLoadCnt];
            }
#endif
        }

#if UNITY_EDITOR
        private List<KeyValuePair<string, Type>> m_WaitLoadValuPairs;
        private int m_TotalNeedLoadCnt = 0;
        private int m_CurrentLoadedCnt = 0;
#endif
    }
}