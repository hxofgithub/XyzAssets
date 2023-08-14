using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace XyzAssets.Runtime
{
    public enum EResourcePlayMode
    {
        EditorMode,

        OnlineMode,
    }
    public static class XyzAsset
    {
        public static void SetLogger(ILogger logger)
        {
            XyzLogger.SetLogger(logger);
        }

        public static void SetLoggerEnable(bool value)
        {
            XyzLogger.SetEnable(value);
        }

        public static void SetMaxTimeSlice(long maxElapsedMilliseconds)
        {
            if (maxElapsedMilliseconds < 10)
                maxElapsedMilliseconds = 10;
            OperatorSystem.MaxTimeSlice = maxElapsedMilliseconds;
        }

        public static InitializeOperator Initialize(InitializeParameters initializeParameters)
        {
            if (initializeParameters == null) throw new ArgumentNullException("initializeParameters");

            if (initializeParameters is SimulateInitializeParameters)
                m_Impl = new SimulateSystemImpl();
            else if (initializeParameters is OnlineInitializeParameters)
                m_Impl = new OnlineSystemImpl();

            InitializeOperator initializeOperator = m_Impl.Initialize(initializeParameters);
            XyzAssetDriver.TryCreateInstance();
            OperatorSystem.Initialize();
            return initializeOperator;
        }

        public static UpdateManifestOperator UpdateManifest()
        {
            ThrowImplIsNull();
            return m_Impl.UpdateManifest();
        }

        public static DownloadOperator CreateDownloader(params string[] modeNames)
        {
            ThrowImplIsNull();
            return m_Impl.CreateDownloaderByModes(modeNames);
        }

        public static LoadSceneOperator LoadSceneAsync(string assetPath, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            ThrowImplIsNull();
            return m_Impl.LoadSceneOperator(assetPath, loadSceneMode);
        }

        public static BaseLoadAssetOperator LoadAssetAsync(string assetPath, Type type)
        {
            ThrowImplIsNull();
            return m_Impl.LoadAssetAsync(assetPath, type);
        }

        public static BaseLoadAssetOperator LoadAssetAsync<T>(string assetPath)
        {
            ThrowImplIsNull();
            return m_Impl.LoadAssetAsync<T>(assetPath);
        }

        public static BaseLoadAllAssetsOperator LoadAssetsAsync(List<KeyValuePair<string, Type>> valuePairs)
        {
            ThrowImplIsNull();
            return m_Impl.LoadAssetsAsync(valuePairs);
        }

        public static BaseLoadAllAssetsOperator LoadAssetsAsync<T>(IEnumerable<string> collection)
        {
            ThrowImplIsNull();
            return m_Impl.LoadAssetsAsync<T>(collection);
        }

        public static ExtractResourcesOperator ExtractResources(string extractPath, params string[] modeName)
        {
            ThrowImplIsNull();
            return m_Impl.ExtractResources(extractPath, modeName);
        }
        internal static void Execute()
        {
            if (m_Impl == null) return;
            OperatorSystem.Execute();
            WeakRefSystem.Execute();
        }

        internal static void Dispose()
        {
            OperatorSystem.Dispose();
            WeakRefSystem.Dispose();
            if (m_Impl != null)
            {
                m_Impl.Dispose();
                m_Impl = null;
            }
            XyzLogger.Log("XyzAssets Dispose");
        }

        internal static void ThrowImplIsNull()
        {
            if (m_Impl == null)
                throw new Exception("Error call.You should call XyzAsset.Initialize first.");
        }

        private static IAssetsSystemImpl m_Impl;

    }
}