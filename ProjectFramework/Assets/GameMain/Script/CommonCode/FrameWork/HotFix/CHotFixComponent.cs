using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework;
using GameFramework.Resource;
using System;
using HybridCLR;

namespace GameFrameworkPackage
{
    public class CHotFixComponent : GameFrameworkComponent
    {
        private Assembly m_asHotFixEntry = null;

        public void StartHotFix()
        {
            if (CGameEntryMgr.Base.EditorResourceMode)
            {
                _InitHotFixEntryAssInEditor();
            }
            else
            {
                _LoadHotFixDll();
            }
        }

        private void _LoadHotFixDll()
        {
            LoadAssetCallbacks loadHotFixDllCallbacks = new LoadAssetCallbacks(_OnLoadHotFixAssetSuccess, _OnLoadHotFixAssetFail);
            CLoadGroupAssetesTools.StartLoad(CHotFixSetting.GetAllHotFixResFullPath(), loadHotFixDllCallbacks, _OnHotFixDllLoadEnd);
        }

        private void _OnLoadHotFixAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                Log.Debug($"Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                Assembly asm = Assembly.Load(textAsset.bytes);
                Log.Debug($"Assembly [ {asm.GetName().Name} ] loaded");

                if (asm.GetName().Name.Contains(CHotFixSetting.GetHotFixEntryName()))
                {
                    m_asHotFixEntry = asm;
                }

            }
            catch (Exception e)
            {
                _OnEnterHotFixFail();
                Log.Fatal(e);
                throw;
            }
        }

        private void _OnLoadHotFixAssetFail(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Fatal("Load HotFix Asset Fail AssetName={0}; error Msg = {1}", assetName, errorMessage);
        }

        private void _OnHotFixDllLoadEnd(bool a_bAllSuccess)
        {
            if (a_bAllSuccess)
            {
                _LoadAOTDll();
            }
            else
            {
                _OnEnterHotFixFail();
            }
        }


        private void _LoadAOTDll()
        {
            LoadAssetCallbacks loadAOTDllCallbacks = new LoadAssetCallbacks(_OnLoadAOTAssetSuccess, _OnLoadAOTAssetFail);
            CLoadGroupAssetesTools.StartLoad(CHotFixSetting.GetAllAOTResFullPath(), loadAOTDllCallbacks, _OnAOTDllLoadEnd);
        }

        private void _OnLoadAOTAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            TextAsset dll = asset as TextAsset;
            if (dll == null)
            {
                Log.Debug("_OnLoadAOTAssetSuccess But Asset Not Fit, AssetName = {0}", assetName);
                return;
            }
            try
            {
                byte[] arrDllBytes = dll.bytes;
                unsafe
                {
                    fixed (byte* ptr = arrDllBytes)
                    {
                        LoadImageErrorCode err = (LoadImageErrorCode)RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, arrDllBytes.Length);
                        if (err != LoadImageErrorCode.OK)
                        {
                            Log.Warning("LoadMetadataForAOTAssembly Fail, DllName= {0}, Error = {1}", assetName, err);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Fatal(e);
                _OnEnterHotFixFail();
                throw;
            }
        }

        private void _OnLoadAOTAssetFail(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Fatal("Load AOT Asset Fail AssetName={0}; error Msg = {1}", assetName, errorMessage);
        }

        private void _OnAOTDllLoadEnd(bool a_bSuccess)
        {
            if (a_bSuccess)
            {
                _EnterHotFix();
            }
            else
            {
                _OnEnterHotFixFail();
            }
        }

        private void _EnterHotFix()
        {
            InvokeHotFixEntryStaticMethod("CHotFixEntry", "Enter");
        }

        private void _InitHotFixEntryAssInEditor()
        {
            m_asHotFixEntry = _GetHotFixEntryAssInEditor();
            if (null == m_asHotFixEntry)
            {
                _OnEnterHotFixFail();
            }
            else
            {
                _EnterHotFix();
            }
        }

        private Assembly _GetHotFixEntryAssInEditor()
        {
            Assembly mainLogicAssembly = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Compare(CHotFixSetting.GetHotFixEntryName(), CHotFixSetting.GetDllName(asm.GetName().Name), StringComparison.Ordinal) == 0)
                {
                    mainLogicAssembly = asm;
                    break;
                }
            }
            return mainLogicAssembly;
        }

        private void _OnEnterHotFixFail()
        {

        }

        public void InvokeHotFixEntryStaticMethod(string a_szTypeName, string a_szMethodName)
        {
            if (m_asHotFixEntry == null)
            {
                return;
            }
            Type type = m_asHotFixEntry.GetType(a_szTypeName);
            if (null == type)
            {
                return;
            }
            MethodInfo method = type.GetMethod(a_szMethodName);
            if (null == method)
            {
                return;
            }
            Action fn = (Action)System.Delegate.CreateDelegate(typeof(Action), null, method);
            fn();
        }
    }
}

