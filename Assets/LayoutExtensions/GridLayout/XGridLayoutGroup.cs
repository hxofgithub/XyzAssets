
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.ILayoutExtensions
{
    public sealed class XGridLayoutGroup : UIBehaviour, UI.ILayoutGroup
    {
        #region Properties
        public enum Axis { Horizontal = 0, Vertical = 1 }
        public enum Constraint { Flexible = 0, FixedColumnCount = 1, FixedRowCount = 2 }

        [SerializeField]
        private RectOffset m_Padding = new RectOffset();
        public RectOffset padding
        {
            get { return m_Padding; }
            set
            {
                SetProperty(ref m_Padding, value);
            }
        }

        [SerializeField]
        private Vector2 m_CellSize = new Vector2(100, 100);
        public Vector2 cellSize
        {
            get { return m_CellSize; }
            set { SetProperty(ref m_CellSize, value); }
        }

        [SerializeField]
        private Vector2 m_Spacing = Vector2.zero;
        public Vector2 spacing
        {
            get { return m_Spacing; }
            set { SetProperty(ref m_Spacing, value); }
        }

        [SerializeField]
        private Axis m_StartAxis = Axis.Vertical;
        public Axis startAxis
        {
            get { return m_StartAxis; }
            set { SetProperty(ref m_StartAxis, value); }
        }

        [FormerlySerializedAs("m_Alignment")]
        [SerializeField]
        private TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;
        public TextAnchor childAlignment { get { return m_ChildAlignment; } set { SetProperty(ref m_ChildAlignment, value); } }

        [SerializeField]
        private Constraint m_Constraint = Constraint.Flexible;

        public Constraint constraint
        {
            get { return m_Constraint; }
            set { SetProperty(ref m_Constraint, value); }
        }

        [SerializeField]
        private int m_ConstraintCount = 2;

        public int constraintCount
        {
            get { return m_ConstraintCount; }
            set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); }
        }

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
        private UnityEngine.UI.ScrollRect m_ScrollRect;
        public UnityEngine.UI.ScrollRect scrollRect
        {
            get
            {
                return m_ScrollRect;
            }
            set { SetProperty(ref m_ScrollRect, value); }
        }

        private bool m_Update = false;
        private int m_CellCount = 0;
        private int m_LastUpdateIndex = -1;
        private int m_CellsPerMainAxis, m_ActualCellCountX, m_ActualCellCountY;
        private int m_CellViewCountX, m_CellViewCountY, m_CellViewCountNum;
        private Vector2 m_StartOffet = Vector2.zero;
        private Dictionary<int, RectTransform> m_ActiveItemDict = new Dictionary<int, RectTransform>();
        private XLayoutEvents m_layoutEvents;

        private void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            SetDirty();            
        }
        #endregion

        #region Public methods.

        /// <summary>
        /// Set Initialize data.
        /// </summary>
        /// <param name="itemList">Set the item template. </param>
        /// <param name="callbackFunc">Set func for updating item.</param>
        /// <param name="totalCellCount"></param>
        public void SetInitData(int totalCellCount, XLayoutEvents layoutEvents)
        {
            Clear();
            m_layoutEvents = layoutEvents;            
            m_CellCount = totalCellCount;            
            SetDirty();            
        }

        public void BeginUpdate(bool forceUpdate = true)
        {
            m_Update = true;
            if (forceUpdate)
                m_LastUpdateIndex = -1;

            SetComponentEnable(true);
        }

        public RectTransform IndexToItem(int index)
        {
            var itemIndex = index % m_CellViewCountNum;
            return m_ActiveItemDict[itemIndex];
        }
        public void MoveToStart(float duration)
        {
            MoveToIndex(0, duration);
        }

        public void MoveToEnd(float duration)
        {
            MoveToIndex(m_CellCount - 1, duration);
        }

        public void MoveToIndex(int i, float duartion)
        {
            if (i >= m_CellCount || i < 0)
                return;

            var viewCount = GetRealViewCount();

            //TODO::
            if (m_CellCount <= viewCount)
                return;

            if (!m_ScrollRect.horizontal && !m_ScrollRect.vertical)
                return;

            Vector2 endPos;
            if (i + viewCount > m_CellCount)
            {
                endPos = rectTransform.sizeDelta;
                if (m_ScrollRect.horizontal)
                {
                    endPos.x = endPos.x - m_ScrollRect.viewport.rect.size.x;
                    endPos.x = -Mathf.Max(0, endPos.x);
                }
                else
                {
                    endPos.y = endPos.y - m_ScrollRect.viewport.rect.size.y;
                    endPos.y = Mathf.Max(0, endPos.y);
                }
            }
            else
            {
                int positionX;
                int positionY;
                if (startAxis == Axis.Horizontal)
                {
                    positionX = i / m_CellViewCountY;
                    positionY = i % m_CellViewCountY;
                }
                else
                {
                    positionX = i % m_CellViewCountX;
                    positionY = i / m_CellViewCountX;
                }

                endPos = new Vector2(m_StartOffet.x + (cellSize[0] + spacing[0]) * positionX, m_StartOffet.y + (cellSize[1] + spacing[1]) * positionY);
                endPos += new Vector2(-padding.left, -padding.top);
                endPos.x = -endPos.x;
                if (m_ScrollRect.horizontal)
                    endPos.y = rectTransform.anchoredPosition.y;
                else if (m_ScrollRect.vertical)
                    endPos.x = rectTransform.anchoredPosition.x;
            }

            //DG.Tweening.DOTween.To(() => rectTransform.anchoredPosition, pos => rectTransform.anchoredPosition = pos, endPos, duartion);
            StopMove();
            SimpleMove.Instance.Do(() => rectTransform.anchoredPosition, pos => rectTransform.anchoredPosition = pos, endPos, duartion, Vector2.Lerp);
        }

        public void StopMove()
        {
            m_ScrollRect.StopMovement();
        }


        public void RefreshViewWhenDataInsertAtFront(int newLen)
        {
            var endPos = rectTransform.anchoredPosition;
            var _addLen = newLen - m_CellCount;
            Vector2 offset;
            if (startAxis == Axis.Horizontal)
            {
                offset = Vector2.left * (_addLen * (cellSize[0] + spacing[0]));
            }
            else
            {
                offset = Vector2.up * (_addLen * (cellSize[1] + spacing[1]));
            }

            m_CellCount = newLen;
            RecalculateConstData();

            endPos += offset;

        }

        public void Clear()
        {
            RecycleAllItem();

            m_CellCount = 0;
            m_ActiveItemDict.Clear();

            SetDirty();
        }

        #endregion

        #region UIBehaviour Lifetime calls


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (startAxis == Axis.Horizontal)
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x == 0 ? 100 : rectTransform.offsetMax.x, 0);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y == 0 ? 100 : rectTransform.offsetMax.y);
            }
            rectTransform.pivot = new Vector2(0, 1);
        }
#endif

        protected override void Start()
        {
            base.Start();
            SetDirty();
        }

        private void Update()
        {
            if (m_ScrollRect == null || !m_Update || m_CellCount <= 0)
            {
                SetComponentEnable(false);
                return;
            }
            
            var viewStartIndex = GetItemStartIndexInView();
            if (viewStartIndex != m_LastUpdateIndex)
            {
                m_LastUpdateIndex = viewStartIndex;
                var endIndex = 0;
                var startIndex = 0;
                
                if (m_ScrollRect.vertical)
                {
                    startIndex = m_CellViewCountX * m_LastUpdateIndex;
                    endIndex = (m_CellViewCountY + m_LastUpdateIndex) * m_CellViewCountX;
                }
                else
                {
                    startIndex = m_CellViewCountY * m_LastUpdateIndex;
                    endIndex = m_CellViewCountY * (m_CellViewCountX + m_LastUpdateIndex);
                }
                endIndex = Mathf.Min(endIndex, m_CellCount);
                for (; startIndex < endIndex; startIndex += 1)
                {
                    var item = GetItemByRealIndex(startIndex);
                    if (item != null)
                    {
                        m_layoutEvents.UpdateItem(startIndex, item);
                        CalculateAndSetItemPos(startIndex, item);
                    }
                }
            }
        }



#if UNITY_EDITOR
        [ContextMenu("Reposition")]
        public void Reposition()
        {
            if (Application.isPlaying)
                return;

            m_CellCount = rectTransform.childCount;
            if (m_ScrollRect == null || m_CellCount == 0)
                return;

            RecalculateConstData();
            var index = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                var temp = transform.GetChild(i);
                if (temp.gameObject.activeSelf)
                {
                    CalculateAndSetItemPos(index++, temp as RectTransform);
                }
            }
        }
#endif
        #endregion

        #region Private methods.

        private void SetDirty()
        {
            if (m_ScrollRect == null || m_CellCount <= 0)
            {
                SetComponentEnable(false);
                return;
            }
            RecalculateConstData();
            m_LastUpdateIndex = -1;
            SetComponentEnable(true);
        }

        /// <summary>
        /// Calculate const data what we shouldn't change later. 
        /// </summary>
        private void RecalculateConstData()
        {
            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;
            if (m_Constraint == Constraint.FixedColumnCount)
            {
                m_CellViewCountX = m_ConstraintCount;
                m_CellViewCountY = Mathf.CeilToInt(m_CellCount / (float)m_CellViewCountX - 0.001f);
            }
            else if (m_Constraint == Constraint.FixedRowCount)
            {
                m_CellViewCountY = m_ConstraintCount;
                m_CellViewCountX = Mathf.CeilToInt(m_CellCount / (float)m_CellViewCountY - 0.001f);
            }
            else
            {
                if (cellSize.x + spacing.x <= 0)
                    m_CellViewCountX = int.MaxValue;
                else
                    m_CellViewCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

                if (cellSize.y + spacing.y <= 0)
                    m_CellViewCountY = int.MaxValue;
                else
                    m_CellViewCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
            }

            if (startAxis == Axis.Vertical)
            {
                m_CellsPerMainAxis = m_CellViewCountX;
                m_ActualCellCountX = Mathf.Clamp(m_CellViewCountX, 1, m_CellCount);
                m_ActualCellCountY = Mathf.Clamp(m_CellViewCountY, 1, Mathf.CeilToInt(m_CellCount / (float)m_CellsPerMainAxis));
            }
            else
            {
                m_CellsPerMainAxis = m_CellViewCountY;
                m_ActualCellCountY = Mathf.Clamp(m_CellViewCountY, 1, m_CellCount);
                m_ActualCellCountX = Mathf.Clamp(m_CellViewCountX, 1, Mathf.CeilToInt(m_CellCount / (float)m_CellsPerMainAxis));
            }

            Vector2 requiredSpace = new Vector2(
                    m_ActualCellCountX * cellSize.x + (m_ActualCellCountX - 1) * spacing.x,
                    m_ActualCellCountY * cellSize.y + (m_ActualCellCountY - 1) * spacing.y
                    );
            m_StartOffet = new Vector2(GetStartOffset(0, requiredSpace.x), GetStartOffset(1, requiredSpace.y));

            var size = rectTransform.rect.size;
            if (m_ScrollRect.vertical)
            {
                m_CellViewCountY = Mathf.CeilToInt((m_ScrollRect.viewport.rect.size.y - m_StartOffet.y) / (cellSize[1] + spacing[1])) + 1;

                size.y = Mathf.CeilToInt(m_CellCount * 1.0f / m_CellViewCountX) * (cellSize[1] + spacing[1]) + padding.top + padding.bottom;
                size.y -= spacing.y;


            }
            else if (m_ScrollRect.horizontal)
            {
                m_CellViewCountX = Mathf.CeilToInt((m_ScrollRect.viewport.rect.size.x - m_StartOffet.x) / (cellSize[0] + spacing[0])) + 1;
                size.x = Mathf.CeilToInt(m_CellCount / m_CellViewCountY) * (cellSize[0] + spacing[0]) + padding.left + padding.right;
                size.x -= spacing.x;
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            m_CellViewCountNum = m_CellViewCountX * m_CellViewCountY;
        }

        /// <summary>
        /// Calculate and set item pos.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="item"></param>
        private void CalculateAndSetItemPos(int i, RectTransform item)
        {
            int positionX;
            int positionY;
            if (startAxis == Axis.Horizontal)
            {
                positionX = i / m_CellViewCountY;
                positionY = i % m_CellViewCountY;
            }
            else
            {
                positionX = i % m_CellViewCountX;
                positionY = i / m_CellViewCountX;
            }

            SetChildAlongAxis(item, 0, m_StartOffet.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
            SetChildAlongAxis(item, 1, m_StartOffet.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
        }

        /// <summary>
        ///  Get item by real index.
        /// </summary>
        /// <param name="i">Real index.</param>
        /// <returns></returns>
        private RectTransform GetItemByRealIndex(int i)
        {
            i = i % (m_CellViewCountNum);
            RectTransform val;
            if (!m_ActiveItemDict.TryGetValue(i, out val))
            {
                val = Get();
                if (val != null)
                {
                    m_ActiveItemDict[i] = val;
                    if (!val.gameObject.activeSelf)
                        val.gameObject.SetActive(true);
                }
            }
            return val;
        }

        /// <summary>
        /// GetItemStartIndexInView
        /// </summary>
        /// <returns> Real Index </returns>
        private int GetItemStartIndexInView()
        {
            var anchoredPosition = rectTransform.anchoredPosition;
            float startIndex = 0;
            if (m_ScrollRect.horizontal)
            {
                startIndex = (anchoredPosition.x - m_StartOffet.x) / (cellSize[0] + spacing[0]);
                return -Mathf.Min(0, (int)startIndex);
            }
            else if (m_ScrollRect.vertical)
            {
                startIndex = (anchoredPosition.y - m_StartOffet.y) / (cellSize[1] + spacing[1]);
                return Mathf.Max(0, (int)startIndex);
            }
            return 0;
        }

        /// <summary>
        /// Get the start offset.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="requiredSpaceWithoutPadding"></param>
        /// <returns></returns>
        private float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);
            float availableSpace = rectTransform.rect.size[axis];
            float surplusSpace = availableSpace - requiredSpace;
            float alignmentOnAxis = 0;
            if (axis == 0)
                alignmentOnAxis = ((int)childAlignment % 3) * 0.5f;
            else
                alignmentOnAxis = ((int)childAlignment / 3) * 0.5f;
            return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
        }

        private void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
        {
            if (rect == null)
                return;

            rect.SetInsetAndSizeFromParentEdge(axis == 0 ? RectTransform.Edge.Left : RectTransform.Edge.Top, pos, size);
        }

        private void SetComponentEnable(bool state)
        {
            if (enabled != state)
                enabled = state;
        }

        /// <summary>
        /// Recycle all items.
        /// </summary>
        private void RecycleAllItem()
        {
            if (m_ActiveItemDict.Count > 0)
            {
                foreach (var item in m_ActiveItemDict.Values)
                {
                    m_layoutEvents.Release(item);
                }
                m_ActiveItemDict.Clear();
            }
        }

        private RectTransform Get()
        {
            return m_layoutEvents.Get();
        }

        private int GetRealViewCount()
        {
            int x = 0, y = 0;
            if (m_ScrollRect.horizontal)
            {
                x = m_CellViewCountX - 1;
                y = m_CellViewCountY;
            }
            else if (m_ScrollRect.vertical)
            {
                y = m_CellViewCountY - 1;
                x = m_CellViewCountX;
            }

            return x * y;
        }

        private void AddItemView(RectTransform itemView)
        {
            int index = m_ActiveItemDict.Count;
            if (index >= m_CellCount)
            {
                m_layoutEvents.Release(itemView);
                return;
            }
            itemView.SetParent(rectTransform, false);
            m_ActiveItemDict[index] = itemView;

            //Update current Item.
            var endIndex = 0;
            var startIndex = 0;

            if (m_ScrollRect.vertical)
            {
                startIndex = m_CellViewCountX * m_LastUpdateIndex;
                endIndex = (m_CellViewCountY + m_LastUpdateIndex) * m_CellViewCountX;
            }
            else
            {
                startIndex = m_CellViewCountY * m_LastUpdateIndex;
                endIndex = m_CellViewCountY * (m_CellViewCountX + m_LastUpdateIndex);
            }
            endIndex = Mathf.Min(endIndex, m_CellCount);
            for (; startIndex < endIndex; startIndex += 1)
            {
                var itemIndex = startIndex % m_CellViewCountNum;
                if (itemIndex == index)
                {
                    m_layoutEvents.UpdateItem(startIndex, itemView);
                    CalculateAndSetItemPos(startIndex, itemView);
                    break;
                }
            }

        }
        #endregion

        #region ILayoutGroup methods.
        public void SetLayoutHorizontal() { }

        public void SetLayoutVertical()
        {
            ///We only use this func in editor model.
#if UNITY_EDITOR
            Reposition();
#endif
        }

        #endregion
        
    }

}
