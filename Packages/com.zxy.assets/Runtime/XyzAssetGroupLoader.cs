
using System.Collections.Generic;
using UObject = UnityEngine.Object;

namespace XyzAssets.Runtime
{
    public class XyzAssetGroupLoader : AsyncOperationBase
    {
        public event System.Action OnComplete;
        internal XyzAssetGroupLoader() { }
        public T Get<T>(string assetName) where T : UObject
        {
            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName);
            var key = GetKey(typeof(T), assetName);
            if (m_result != null && m_result.ContainsKey(key))
                return m_result[key] as T;
            return null;

        }
        public override EOperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                base.Status = value;
                if (OnComplete != null && value == EOperatorStatus.Succeed)
                {
                    OnComplete();
                    OnComplete = null;
                }
            }
        }
        public XyzAssetGroupLoader Add<T>(string assetPath)
        {
            if (m_IsBegin)
                throw new System.Exception("XyzAssetGroupLoader is not allowed add when is loading.");
            if (string.IsNullOrEmpty(assetPath))
                throw new System.ArgumentNullException("assetPath");
            var type = typeof(T);
            if (type == null)
                throw new System.ArgumentNullException("type");

            m_AssetPathList.Add(assetPath, type);
            return this;
        }
        public XyzAssetGroupLoader AddRange(IEnumerable<string> collection)
        {
            return AddRange(collection, typeof(UObject));
        }
        public XyzAssetGroupLoader AddRange<T>(IEnumerable<string> collection)
        {
            return AddRange(collection, typeof(T));
        }
        public XyzAssetGroupLoader AddRange(IEnumerable<string> collection, System.Type type)
        {
            if (m_IsBegin)
                throw new System.Exception("XyzAssetGroupLoader is not allowed add when is loading.");
            if (collection == null)
                throw new System.ArgumentNullException("collection");
            if (type == null)
                throw new System.ArgumentNullException("type");
            using (IEnumerator<string> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    m_AssetPathList.Add(en.Current, type);
                }
            }
            return this;
        }
        public void Begin()
        {
            if (m_IsBegin) return;
            m_IsBegin = true;
            m_TotalNeedLoadCnt = m_AssetPathList.Count;
            m_CurrentLoadCnt = 0;
            m_result = new Dictionary<string, UObject>(m_TotalNeedLoadCnt);
            m_AssetLoadOperatorDict = new List<AsyncOperationBase>(m_TotalNeedLoadCnt);
            foreach (var item in m_AssetPathList)
            {
                var ope = XyzAsset.LoadAssetAsync(item.Key, item.Value);
                ope.OnComplete += XyzAssetGroupLoader_OnComplete;
                m_AssetLoadOperatorDict.Add(ope);
            }
        }

        private void XyzAssetGroupLoader_OnComplete(BaseLoadAssetOperator obj)
        {
            m_result.Add(GetKey(obj.AssetObject.GetType(), obj.AssetObject.name), obj.AssetObject);
            m_CurrentLoadCnt++;
            if (m_CurrentLoadCnt == m_TotalNeedLoadCnt)
            {
                Status = EOperatorStatus.Succeed;
            }
        }
        private string GetKey(System.Type type, string name)
        {
            return $"{type.Name}_{name}";
        }
        #region override
        protected override void OnDispose()
        {
            foreach (var item in m_AssetLoadOperatorDict)
            {
                item.Dispose();
            }
            m_AssetPathList.Clear();
            m_AssetLoadOperatorDict = null;
            m_result = null;
            m_IsBegin = false;
        }
        protected override void OnExecute()
        {
            Progress = m_CurrentLoadCnt * 1f / m_TotalNeedLoadCnt;
        }
        protected override void OnStart()
        {

        }
        #endregion

        private bool m_IsBegin = false;
        private readonly Dictionary<string, System.Type> m_AssetPathList = new Dictionary<string, System.Type>(32);
        private List<AsyncOperationBase> m_AssetLoadOperatorDict;
        private Dictionary<string, UObject> m_result;
        private int m_TotalNeedLoadCnt, m_CurrentLoadCnt;
    }
}