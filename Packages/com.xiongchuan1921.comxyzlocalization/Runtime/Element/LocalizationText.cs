using UnityEngine;
using UnityEngine.UI;

namespace XyzLocalization.Runtime
{
    [RequireComponent(typeof(Text))]
    public class LocalizationText : BaseLocalizationItem
    {
        public string text { get { return _mTextComp.text; } set { _mTextComp.text = value; } }

        private Text _mTextComp;

        protected override void Start()
        {
            _mTextComp = GetComponent<Text>();
            base.Start();
        }

        internal override void OnLocalization()
        {
            _mTextComp.text = LocalizationManager.GetValue(Key);
        }

        public void SetKey(string key, params string[] args)
        {
            _mTextComp.text = string.Format(LocalizationManager.GetValue(key), args);
        }
    }
}
