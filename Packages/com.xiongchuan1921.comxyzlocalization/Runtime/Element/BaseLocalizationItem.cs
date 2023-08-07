using UnityEngine;
namespace XyzLocalization.Runtime
{
    public abstract class BaseLocalizationItem : MonoBehaviour
    {
        public string Key { get; private set; }
        public virtual void SetKey(string key)
        {
            Key = key;
            OnLocalization();
        }

        protected void Awake()
        {
            LocalizationManager.AddItem(this);
        }
        protected virtual void Start()
        {
            OnLocalization();
        }

        protected void OnDestroy()
        {
            LocalizationManager.RemoveItem(this);
        }
        internal abstract void OnLocalization();
    }

}
