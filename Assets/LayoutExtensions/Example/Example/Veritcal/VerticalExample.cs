
using System.Collections.Generic;

namespace UnityEngine.ILayoutExtensions
{
    public class VerticalExample : MonoBehaviour
    {

        public RectTransform temp;
        public XLayoutBase layout;
                

        private void Start()
        {
            LoadData();
        }

        private void ChangeButtonSize(RectTransform t, float newSize)
        {
            t.SetPreferredHeight(newSize);
            layout.ChangeCellSize(t.GetComponent<ICellView>().index, newSize);
        }

        private void LoadData()
        {
            XLayoutEvents callback = new XLayoutEvents();
            callback.SetCreateFunc(() =>
            {
                var t = GameObject.Instantiate(temp);

                t.Find("Add").GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    ChangeButtonSize(t, t.GetPreferredHeight() + 10);
                });

                t.Find("Reduce").GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    ChangeButtonSize(t, t.GetPreferredHeight() - 10);
                });
                return t;
            });
            callback.SetUpdateCellAction((index, t) =>
            {
                t.Find("Text").GetComponentInChildren<UnityEngine.UI.Text>().text = "Item_" + index;
                t.GetComponent<ICellView>().index = index;
            });
            layout.SetInitData(temp.GetPreferredHeight(), 100, callback);
        }

        public void MoveToEnd()
        {
            float pos;
            if (layout.GetCellPos(50, out pos))
            {
                layout.StopMove();
                SimpleMove.Instance.Do(
                    () => { return layout.scrollRect.content.anchoredPosition.y; },
                    f =>
                    {
                        Vector2 p = layout.scrollRect.content.anchoredPosition;
                        p.y = f;
                        layout.scrollRect.content.anchoredPosition = p;
                    }, pos, 0.3f, Mathf.Lerp);
            }
        }
    }

}

