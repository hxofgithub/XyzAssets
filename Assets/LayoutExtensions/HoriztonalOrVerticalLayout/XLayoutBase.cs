using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace UnityEngine.ILayoutExtensions
{
    public class CellRange
    {
        public float startPos;
        public float size;
        public float endPos;
    }

    public class XLayoutEvents
    {
        public XLayoutEvents(System.Func<RectTransform> createFunc = null, System.Action<int, RectTransform> updateCellAction = null, System.Action<RectTransform> recycleCellAction = null)
        {
            m_pool = new Stack<RectTransform>();
            m_recycleAcion = recycleCellAction;
            m_createFunc = createFunc;
            m_updateCellAction = updateCellAction;
        }

        public RectTransform Get()
        {
            if (m_pool.Count > 0)
                return m_pool.Pop();
            return m_createFunc != null ? m_createFunc() : null;
        }

        public void UpdateItem(int index, RectTransform element)
        {
            if (m_updateCellAction != null)
                m_updateCellAction(index, element);
        }

        public void Release(RectTransform element)
        {
            if (m_pool.Count > 0 && ReferenceEquals(m_pool.Peek(), element))
            {
                Debug.LogError("You want release element what was released!!!");
                return;
            }

            if (element == null)
            {
                Debug.LogError("You want release element what was null!!!");
                return;
            }

            m_pool.Push(element);
            if (m_recycleAcion != null)
                m_recycleAcion(element);
        }

        public void SetCreateFunc(System.Func<RectTransform> createFunc)
        {
            m_createFunc = createFunc;
        }

        public void SetUpdateCellAction(System.Action<int, RectTransform> updateCellAction)
        {
            m_updateCellAction = updateCellAction;
        }

        public void SetRecycleAction(System.Action<RectTransform> recycleAcion)
        {
            m_recycleAcion = recycleAcion;
        }

        private System.Action<RectTransform> m_recycleAcion;
        private System.Action<int, RectTransform> m_updateCellAction;
        private System.Func<RectTransform> m_createFunc;
        private Stack<RectTransform> m_pool;

    }

    public abstract class XLayoutBase : UIBehaviour
    {
        #region Properties

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }


        [SerializeField]
        private ScrollRect m_ScrollRect;
        public ScrollRect scrollRect
        {
            get
            {
                return m_ScrollRect;
            }

            set
            {
                SetProperty(ref m_ScrollRect, value);
                Clear();
            }
        }

        [SerializeField]
        protected HorizontalOrVerticalLayoutGroup m_layoutGroup;
        public HorizontalOrVerticalLayoutGroup layoutGroup
        {
            get { return m_layoutGroup; }
            set { m_layoutGroup = value; }
        }


        public int cellCount { get; private set; }

        protected CellRange[] m_CellRanges = null;
        private Range m_VisibleRange;
        private Dictionary<int, RectTransform> m_ActiveItemDict = new Dictionary<int, RectTransform>();
        private Transform m_TopPlaceHolder;
        private XLayoutEvents m_LayoutEvents;
        private bool m_RefreshView = false;

        #endregion

        #region Public methods.

        /// <summary>
        /// Set Initialize data.
        /// </summary>
        public void SetInitData(float initCellSize, int totalCellCount, XLayoutEvents callback)
        {
            Clear();
            m_LayoutEvents = callback;
            cellCount = totalCellCount;
            RecalculateData(initCellSize);
            m_RefreshView = true;
        }

        public void ChangeCellSize(int index, float newSize)
        {
            var deltaSize = newSize - m_CellRanges[index].size;
            m_CellRanges[index].size = newSize;
            m_CellRanges[index].endPos += deltaSize;
            for (int i = index + 1; i < cellCount; i++)
            {
                m_CellRanges[i].startPos += deltaSize;
                m_CellRanges[i].endPos += deltaSize;
            }
            m_RefreshView = true;
        }

        public void Clear()
        {
            foreach (var item in m_ActiveItemDict.Values)
            {

                m_LayoutEvents.Release(item);
            }
            m_ActiveItemDict.Clear();
        }

        public bool GetCellPos(int index, out float pos)
        {
            pos = 0;
            if (index >= 0 && m_CellRanges != null)
            {
                if (m_CellRanges.Length > index)
                {
                    pos = m_CellRanges[index].startPos;
                    return true;
                }
            }
            return false;
        }

        public void StopMove()
        {
            m_ScrollRect.StopMovement();
        }

        public void AddCellView(RectTransform transf, int index, bool refresh = false)
        {
            if (m_ActiveItemDict.ContainsKey(index))
                throw new System.ArgumentException();

            m_ActiveItemDict[index] = transf;
            if (refresh)
            {
                m_RefreshView = true;
                m_VisibleRange = new Range(0, 0);
            }
        }

        #endregion

        #region Unity lifetime calls.

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (layoutGroup == null)
                layoutGroup = GetComponentInParent<HorizontalOrVerticalLayoutGroup>();

            if (scrollRect == null)
                scrollRect = GetComponentInParent<ScrollRect>();
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            scrollRect.onValueChanged.AddListener(OnScrollValueChange);
            m_TopPlaceHolder = new GameObject("TopPlaceHolder", typeof(LayoutElement)).transform;
            m_TopPlaceHolder.SetParent(rectTransform, false);
        }

        private void LateUpdate()
        {
            if (m_ScrollRect == null)
                return;

            if (!m_RefreshView)
                return;

            m_RefreshView = false;


            var newVisibleRange = GetVisibleRange();
            if (!m_VisibleRange.IsEqual(newVisibleRange))
            {
                var newStart = newVisibleRange.Start();
                var newEnd = newVisibleRange.End();

                var oldStart = m_VisibleRange.Start();
                var oldEnd = m_VisibleRange.End();

                if (newStart > oldStart)
                {
                    for (int i = oldStart; i < newStart; i++)
                    {
                        HideInvisibleCell(i);
                    }
                }

                if (newEnd < oldEnd)
                {
                    for (int i = newEnd; i < oldEnd; i++)
                    {
                        HideInvisibleCell(i);
                    }
                }

                int siblingIndex = 0;

                for (int i = newStart; i < newEnd; i++)
                {
                    ShowVisibleCell(i, ++siblingIndex);
                }
                m_VisibleRange = newVisibleRange;
                UpdatePlaceHolder();
            }
        }

        #endregion

        #region Private methods.

        private void UpdatePlaceHolder()
        {
            var visibleStart = m_VisibleRange.Start();
            if (visibleStart != 0)
            {
                var size = m_CellRanges[visibleStart - 1].endPos;
                OnSetCellSize(m_TopPlaceHolder as RectTransform, size);
            }
            m_TopPlaceHolder.SetActive(visibleStart != 0);
        }

        private void HideInvisibleCell(int index)
        {
            RectTransform transf;
            if (m_ActiveItemDict.TryGetValue(index, out transf))
            {
                transf.SetActive(false);

                m_LayoutEvents.Release(transf);
                m_ActiveItemDict.Remove(index);
            }
        }

        private void ShowVisibleCell(int index, int siblingIndex)
        {
            RectTransform transf;
            if (!m_ActiveItemDict.TryGetValue(index, out transf))
            {
                transf = m_LayoutEvents.Get();
                if (transf != null)
                {
                    transf.SetActive(true);
                    transf.SetParent(rectTransform, false);
                    m_ActiveItemDict[index] = transf;
                }

            }
            if (transf != null)
            {
                OnSetCellSize(transf, m_CellRanges[index].size);
                transf.SetSiblingIndex(siblingIndex);
                m_LayoutEvents.UpdateItem(index, transf);
            }
        }

        private void OnScrollValueChange(Vector2 value)
        {
            m_RefreshView = true;
        }


        /// <summary>
        /// Calculate const data what we shouldn't change later. 
        /// </summary>
        private void RecalculateData(float size)
        {
            m_CellRanges = new CellRange[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                var range = new CellRange();
                if (i == 0)
                    range.startPos = GetStartOffset();
                else
                    range.startPos = m_CellRanges[i - 1].endPos + layoutGroup.spacing;
                range.size = size;
                range.endPos = range.startPos + size;
                m_CellRanges[i] = range;
            }
            m_VisibleRange = new Range(0, 0);


            var sizeDelta = rectTransform.rect.size;
            sizeDelta = GetContentSize(sizeDelta, size);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);
        }

        private Range GetVisibleRange()
        {
            var startPos = GetViewStartPos();
            var endPos = GetViewEndPos();
            int startIndex = FindCellIndex(startPos, 0, cellCount - 1);
            int endIndex = FindCellIndex(endPos, 0, cellCount - 1);
            return new Range(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>
        /// Binary search.
        /// </summary>
        /// <returns></returns>
        private int FindCellIndex(float f, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
                return startIndex;

            int midIndex = (startIndex + endIndex) / 2;

            var cellRange = m_CellRanges[midIndex];

            if (cellRange.Contains(f))
                return midIndex;
            else if (cellRange.startPos > f)
                return FindCellIndex(f, startIndex, midIndex);
            else
                return FindCellIndex(f, midIndex + 1, endIndex);
        }

        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
        }
        #endregion


        #region abstract methods

        protected abstract void OnSetCellSize(RectTransform transf, float size);
        protected abstract float GetStartOffset();
        protected abstract Vector2 GetContentSize(Vector2 sizeDelta, float size);
        protected abstract float GetViewStartPos();
        protected abstract float GetViewEndPos();

        #endregion
    }
}