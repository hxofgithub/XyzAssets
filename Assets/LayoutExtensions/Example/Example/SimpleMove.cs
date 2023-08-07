using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简易的移动器
/// </summary>

namespace UnityEngine.ILayoutExtensions
{
    public class SimpleMove : MonoBehaviour
    {

        public static SimpleMove Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("SimpleMove").AddComponent<SimpleMove>();
                    Object.DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static SimpleMove _instance;

        public void Do<T>(System.Func<T> getter, System.Action<T> setter, T endValue, float duration, System.Func<T, T, float, T> lerpFunc)
        {
            StartCoroutine(DoMotion(getter, setter, endValue, duration, lerpFunc));
        }

        private IEnumerator DoMotion<T>(System.Func<T> getter, System.Action<T> setter, T endValue, float duration, System.Func<T, T, float, T> lerpFunc)
        {
            if (getter == null || setter == null || lerpFunc == null)
                yield break;
            var endOfFrame = new WaitForEndOfFrame();
            T currentValue = getter();
            float currentTime = 0;
            while (true)
            {
                setter(lerpFunc(currentValue, endValue, currentTime));
                currentTime += Time.deltaTime;
                if (currentTime >= duration)
                {
                    setter(endValue);
                    yield break;
                }
                yield return endOfFrame;
            }
        }



    }
}