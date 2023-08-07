using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace UnityEngine.ILayoutExtensions
{
    public static class XLayoutExtensions
    {
        public static float GetPreferredWidth(this RectTransform trans)
        {
            var element = trans.GetComponent<LayoutElement>();
            if (element != null)
                return element.preferredWidth;
            else
            {
                return trans.sizeDelta.x;
            }
        }

        public static void SetPreferredWidth(this RectTransform trans, float newSize)
        {
            var element = trans.GetComponent<LayoutElement>();
            if (element != null)
                element.preferredWidth = newSize;
            else
            {
                var sizeDelta = trans.sizeDelta;
                sizeDelta.x = newSize;
                trans.sizeDelta = sizeDelta;
            }
        }

        public static float GetPreferredHeight(this RectTransform trans)
        {
            var element = trans.GetComponent<LayoutElement>();
            if (element != null)
                return element.preferredHeight;
            else
            {
                return trans.sizeDelta.y;
            }
        }

        public static void SetPreferredHeight(this RectTransform trans, float newSize)
        {
            var element = trans.GetComponent<LayoutElement>();
            if (element != null)
                element.preferredHeight = newSize;
            else
            {
                var sizeDelta = trans.sizeDelta;
                sizeDelta.y = newSize;
                trans.sizeDelta = sizeDelta;
            }
        }

        public static void ActiveGameObject(this GameObject g, bool state)
        {
            if (g.activeSelf != state)
                g.SetActive(state);
        }

        public static void SetActive(this Transform transf, bool state)
        {
            transf.gameObject.ActiveGameObject(state);
        }

    }

    public static class CellRangExtensions
    {
        public static bool Contains(this CellRange range, float pos)
        {
            return range.startPos <= pos && range.endPos >= pos;
        }
    }

    public static class RangeExtensions
    {
        public static int Start(this Range range)
        {
            return range.from;
        }

        public static int End(this Range range)
        {
            return range.from + range.count;
        }

        public static bool IsEqual(this Range left, Range right)
        {
            return left.from == right.from && left.count == right.count;
        }
    }
}