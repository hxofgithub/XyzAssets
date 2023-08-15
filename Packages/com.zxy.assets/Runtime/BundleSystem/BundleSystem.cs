using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal class BundleRef
    {
        public AssetBundle AssetBundle;
        public int RefCnt;
    }

    internal class BundleSystem
    {
        public static void AddAssetBundle(int bundleId, AssetBundle assetBundle)
        {
            ThrowIfOutOfRange(bundleId);
            if (m_BundleRefDict[bundleId] != null)
                throw new Exception("Dumplicated AssetBundle. ID:" + bundleId);

            m_BundleRefDict[bundleId] = new BundleRef() { AssetBundle = assetBundle, RefCnt = 1 };
        }

        public static bool HasAssetBundle(int bundleId)
        {
            return m_BundleRefDict[bundleId] != null;
        }

        public static AssetBundle GetAssetBundle(int bundleId)
        {
            ThrowIfOutOfRange(bundleId);
            ThrowDataIsNull(bundleId);
            return m_BundleRefDict[bundleId].AssetBundle;
        }

        public static void Reference(int bundleId)
        {
            ThrowIfOutOfRange(bundleId);
            ThrowDataIsNull(bundleId);
            m_BundleRefDict[bundleId].RefCnt++;
        }

        public static void Release(int bundleId)
        {
            ThrowIfOutOfRange(bundleId);
            ThrowDataIsNull(bundleId);
            if (--m_BundleRefDict[bundleId].RefCnt < 0)
            {
                m_BundleRefDict[bundleId].AssetBundle.Unload(true);
                m_BundleRefDict[bundleId] = null;
            }
        }
        public static void InitializeBundleRef(int bundleCnt)
        {
            m_BundleRefDict = new BundleRef[bundleCnt];
        }

        private static void ThrowIfOutOfRange(int bundleId)
        {
            if (bundleId >= m_BundleRefDict.Length || bundleId < 0)
                throw new ArgumentOutOfRangeException("bundleId");
        }

        private static void ThrowDataIsNull(int bundleId)
        {
            if (m_BundleRefDict[bundleId] == null)
                throw new Exception("AssetBundle is null or empty. ID:" + bundleId);
        }

        private static BundleRef[] m_BundleRefDict;
    }
}
