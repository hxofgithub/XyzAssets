
using UnityEngine;
namespace UnityEngine.ILayoutExtensions
{
    public class GridLayoutExample : MonoBehaviour
    {
        public RectTransform temp;
        public XGridLayoutGroup layout;
        public void Start()
        {

            XLayoutEvents e = new XLayoutEvents();
            e.SetUpdateCellAction(UpdateView);
            e.SetCreateFunc(() => 
            {
                var g = GameObject.Instantiate(temp);
                g.SetParent(layout.transform, false);
                return g;
            });
            e.SetRecycleAction((t) => { GameObject.Destroy(t.gameObject); });
            layout.SetInitData(100, e);
            layout.BeginUpdate(true);
        }

        public void MoveToMiddle()
        {
            layout.MoveToIndex(49, 0.3f);
        }

        private void UpdateView(int index, RectTransform transf)
        {
            transf.Find("Text").GetComponentInChildren<UnityEngine.UI.Text>().text = "Item_" + index;
        }

    }
}