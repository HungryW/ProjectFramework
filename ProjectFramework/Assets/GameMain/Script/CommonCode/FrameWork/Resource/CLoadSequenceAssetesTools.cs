using System.Collections.Generic;
using GameFramework;
using GameFramework.Resource;
using System;

namespace GameFrameworkPackage
{
    public class CLoadSequenceAssetesTools : IReference
    {
        private List<string> m_listLoadAssetName;
        private LoadAssetCallbacks m_fnLoadAssetFixCallback;

        private LoadAssetCallbacks m_fnOnOneAssetLoadCallback;
        private Action<bool> m_fnOnEnd;
        private bool m_bLoading;

        public CLoadSequenceAssetesTools()
        {
            m_listLoadAssetName = new List<string>();
            m_fnLoadAssetFixCallback = new LoadAssetCallbacks(_LoadAssetSuccessCallback, _LoadAssetFailureCallback);

            m_fnOnOneAssetLoadCallback = null;
            m_fnOnEnd = null;
            m_bLoading = false;
        }

        public void Clear()
        {
            m_listLoadAssetName.Clear();
            m_fnOnOneAssetLoadCallback = null;
            m_fnOnEnd = null;
            m_bLoading = false;
        }

        public static void StartLoad(string[] a_arrAssetName, LoadAssetCallbacks a_fnOnOneAssetCallback, Action<bool> a_fnOnEnd)
        {
            CLoadSequenceAssetesTools tool = _Create(a_arrAssetName, a_fnOnOneAssetCallback, a_fnOnEnd);
            tool._StartLoad();
        }

        private static CLoadSequenceAssetesTools _Create(string[] a_arrAssetName, LoadAssetCallbacks a_fnOnOneAssetCallback, Action<bool> a_fnOnEnd)
        {
            CLoadSequenceAssetesTools tool = ReferencePool.Acquire<CLoadSequenceAssetesTools>();
            foreach (var name in a_arrAssetName)
            {
                tool.m_listLoadAssetName.Add(name);
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
            _TryStartCur();
        }

        private void _TryStartCur()
        {
            if (m_listLoadAssetName.Count > 0)
            {
                CGameEntryMgr.Resource.LoadAsset(m_listLoadAssetName[0], m_fnLoadAssetFixCallback);
            }
            else
            {
                _OnLoadEnd(true);
            }
        }

        private void _OnLoadEnd(bool a_bSuccess)
        {
            m_fnOnEnd?.Invoke(a_bSuccess);
            ReferencePool.Release(this);
        }

        private void _LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            if (m_fnOnOneAssetLoadCallback != null)
            {
                m_fnOnOneAssetLoadCallback.LoadAssetSuccessCallback?.Invoke(assetName, asset, duration, userData);
            }
            m_listLoadAssetName.RemoveAt(0);
            _TryStartCur();
        }

        private void _LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            if (m_fnOnOneAssetLoadCallback != null)
            {
                m_fnOnOneAssetLoadCallback.LoadAssetFailureCallback?.Invoke(assetName, status, errorMessage, userData);
            }
            _OnLoadEnd(false);
        }
    }
}

