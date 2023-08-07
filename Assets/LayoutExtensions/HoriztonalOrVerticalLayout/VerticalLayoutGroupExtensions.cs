using UnityEngine.UI;

namespace UnityEngine.ILayoutExtensions
{

    [RequireComponent(typeof(VerticalLayoutGroup))]
    public sealed class VerticalLayoutGroupExtensions : XLayoutBase
    {
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (layoutGroup != null)
            {
                var vertical = layoutGroup as VerticalLayoutGroup;
                if (vertical != null)
                {
                    vertical.childForceExpandWidth = true;
                    vertical.childForceExpandHeight = false;
                    vertical.childControlHeight = true;
                    vertical.childControlWidth = true;
                }
            }
        }
#endif
        protected override void OnSetCellSize(RectTransform transf, float size)
        {
            var element = transf.GetComponent<LayoutElement>();
            if (element)
            {
                element.preferredHeight = size;
            }
            else
            {
                Debug.LogError("Item don't have Component:LayoutElement");
            }
        }

        protected override float GetStartOffset()
        {
            return layoutGroup.padding.top;
        }

        protected override Vector2 GetContentSize(Vector2 sizeDelta, float size)
        {
            sizeDelta.y = layoutGroup.padding.vertical + (layoutGroup.spacing + size) * cellCount - layoutGroup.spacing;
            return sizeDelta;
        }

        protected override float GetViewStartPos()
        {
            var anchordPos = scrollRect.content.anchoredPosition;
            return anchordPos.y;
        }

        protected override float GetViewEndPos()
        {
            var startPos = GetViewStartPos();
            return (startPos + scrollRect.viewport.rect.height);
        }

    }
}