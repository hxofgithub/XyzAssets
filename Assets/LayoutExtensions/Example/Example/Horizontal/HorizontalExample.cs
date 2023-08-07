using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.ILayoutExtensions
{

    public class HorizontalExample : MonoBehaviour
    {
        /// <summary>
        /// Template item
        /// </summary>
        public RectTransform temp;

        /// <summary>
        /// 
        /// </summary>
        public XLayoutBase layout;

        public void Start()
        {
            XLayoutEvents callback = new XLayoutEvents();
            callback.SetCreateFunc(() =>
            {
                var t = GameObject.Instantiate(temp);

                t.Find("Add").GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    ChangeButtonSize(t, t.GetPreferredWidth() + 10);
                });

                t.Find("Reduce").GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    ChangeButtonSize(t, t.GetPreferredWidth() - 10);
                });
                return t;
            });

            callback.SetUpdateCellAction((index, t) =>
            {
                t.Find("Text").GetComponentInChildren<UnityEngine.UI.Text>().text = "Item_" + index;
                t.GetComponent<ICellView>().index = index;
            });
            layout.SetInitData(temp.GetPreferredWidth(), 100, callback);
        }

        private void ChangeButtonSize(RectTransform t, float newSize)
        {
            t.SetPreferredWidth(newSize);
            layout.ChangeCellSize(t.GetComponent<ICellView>().index, newSize);
        }

        public void MoveToMiddle()
        {
            float pos;
            if (layout.GetCellPos(50, out pos))
            {
                pos = -pos;
                layout.StopMove();
                SimpleMove.Instance.Do(
                    () => { return layout.scrollRect.content.anchoredPosition.x; },
                    f =>
                    {
                        Vector2 p = layout.scrollRect.content.anchoredPosition;
                        p.x = f;
                        layout.scrollRect.content.anchoredPosition = p;
                    }, pos, 0.3f, Mathf.Lerp);
            }
        }
    }
}