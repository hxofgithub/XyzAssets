using UnityEngine;
using UnityEngine.Events;

namespace XyzAniCheat.Runtime
{

    public class AniCheatDetector : MonoBehaviour
    {
        public float floatEpsilon = 0.001f;
        public float vector2Epsilon = 0.1f;
        public float vector3Epsilon = 0.1f;
        public float quaternionEpsilon = 0.1f;

        public static AniCheatDetector Instance { get; private set; }

        public static void Initialize()
        {
            if (Instance == null)
            {
                var g = new GameObject("Cheating Detector Driver");
                Instance = g.AddComponent<AniCheatDetector>();
            }
        }

        public static void StartDetection()
        {
            if (Instance != null)
            {
                Instance.StartDetection(null);
            }
        }

        public bool ExistsAndIsRunning
        {
            get
            {
                return Instance != null && Instance.m_IsRunning;
            }
        }

        public void StartDetection(UnityAction action)
        {
            if (m_IsRunning)
            {
                return;
            }

            if (!enabled)
                return;

            if (action == null)
            {
                enabled = false;
                return;
            }

            m_DetectAction = action;

            m_IsRunning = true;
        }


        internal void OnCheatingDetected()
        {
            m_DetectAction?.Invoke();
        }


        [SerializeField]
        private UnityAction m_DetectAction;

        private bool m_IsRunning;

    }

}
