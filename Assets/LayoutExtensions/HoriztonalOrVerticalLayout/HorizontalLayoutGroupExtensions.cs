using UnityEngine.UI;

namespace UnityEngine.ILayoutExtensions
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public sealed class HorizontalLayoutGroupExtensions : XLayoutBase
    {
        protected override void OnSetCellSize(RectTransform transf, float size)
        {
            var element = transf.GetComponent<LayoutElement>();
            if (element)
            {
                element.preferredWidth = size;
            }
            else
            {
                Debug.LogError("Item don't have Component:LayoutElement");
            }
        }

        protected override float GetStartOffset()
        {
            return layoutGroup.padding.left;
        }

        protected override Vector2 GetContentSize(Vector2 sizeDelta, float size)
        {
            sizeDelta.x = layoutGroup.padding.horizontal + (layoutGroup.spacing + size) * cellCount - layoutGroup.spacing;
            return sizeDelta;
        }

        protected override float GetViewStartPos()
        {
            var anchordPos = scrollRect.content.anchoredPosition;
            return -anchordPos.x;
        }

        protected override float GetViewEndPos()
        {
            var startPos = GetViewStartPos();
            return (startPos + scrollRect.viewport.rect.width);
        }
    }
}