using System.Collections.Generic;
using GameFramework;
using GameFramework.Resource;
using System;

namespace GameFrameworkPackage
{
    public class CLoadGroupAssetesTools : IReference
    {
        private Dictionary<string, bool> m_mapAssetFlag;
        private LoadAssetCallbacks m_fnLoadAssetFixCallback;

        private LoadAssetCallbacks m_fnOnOneAssetLoadCallback;
        private Action<bool> m_fnOnEnd;
        private bool m_bFailFlag;
        private bool m_bLoading;

        public CLoadGroupAssetesTools()
        {
            m_mapAssetFlag = new Dictionary<string, bool>();
            m_fnLoadAssetFixCallback = new LoadAssetCallbacks(_LoadAssetSuccessCallback, _LoadAssetFailureCallback);

            m_fnOnOneAssetLoadCallback = null;
            m_fnOnEnd = null;
            m_bFailFlag = false;
            m_bLoading = false;
        }

        public void Clear()
        {
            m_mapAssetFlag.Clear();
            m_fnOnOneAssetLoadCallback = null;
            m_fnOnEnd = null;
            m_bFailFlag = false;
            m_bLoading = false;
        }

        public static void StartLoad(string[] a_arrAssetName, LoadAssetCallbacks a_fnOnOneAssetCallback, Action<bool> a_fnOnEnd)
        {
            CLoadGroupAssetesTools tool = _Create(a_arrAssetName, a_fnOnOneAssetCallback, a_fnOnEnd);
            tool._StartLoad();
        }

        private static CLoadGroupAssetesTools _Create(string[] a_arrAssetName, LoadAssetCallbacks a_fnOnOneAssetCallback, Action<bool> a_fnOnEnd)
        {
            CLoadGroupAssetesTools tool = ReferencePool.Acquire<CLoadGroupAssetesTools>();
            foreach (var name in a_arrAssetName)
            {
                tool.m_mapAssetFlag.Add(name, false);
            }
            tool.m_fnOnOneAssetLoadCallback = a_fnOnOneAssetCallback;
            tool.m_fnOnEnd = a_fnOnEnd;
            return tool;
        }

        private void _StartLoad()
        {
            if (m_bLoading)
            {
                return;
            }
            m_bLoading = true;
            foreach (var asset in m_mapAssetFlag)
            {
                CGameEntryMgr.Resource.LoadAsset(asset.Key, m_fnLoadAssetFixCallback);
            }
        }

        private void _LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            if (m_fnOnOneAssetLoadCallback != null)
            {
                m_fnOnOneAssetLoadCallback.LoadAssetSuccessCallback?.Invoke(assetName, asset, duration, userData);
            }
            _SetFlag(assetName);
        }

        private void _LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            if (m_fnOnOneAssetLoadCallback != null)
            {
                m_fnOnOneAssetLoadCallback.LoadAssetFailureCallback?.Invoke(assetName, status, errorMessage, userData);
            }
            m_bFailFlag = true;
            _SetFlag(assetName);
        }

        private void _SetFlag(string a_szAssetName)
        {
            if (m_mapAssetFlag.ContainsKey(a_szAssetName))
            {
                m_mapAssetFlag[a_szAssetName] = true;
            }
            _TryEnd();
        }

        private void _TryEnd()
        {
            if (!_CheckAllLoadEnd())
            {
                return;
            }
            m_fnOnEnd?.Invoke(!m_bFailFlag);
            ReferencePool.Release(this);
        }

        private bool _CheckAllLoadEnd()
        {
            foreach (var flag in m_mapAssetFlag)
            {
                if (!flag.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

