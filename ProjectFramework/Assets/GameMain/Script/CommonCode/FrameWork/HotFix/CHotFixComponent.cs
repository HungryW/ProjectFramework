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
        private Dictionary<string, Assembly> m_mapHotFixAss = new Dictionary<string, Assembly>();

        public void LoadHotFix()
        {
            if (CGameEntryMgr.Base.EditorResourceMode)
            {
                _InitHotFixDllInEditor();
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
                string szName = asm.GetName().Name;
                m_mapHotFixAss.Add(szName, asm);
                Log.Debug("Assembly [{0}] loaded", szName);
            }
            catch (Exception e)
            {
                _OnEnterHotFixFail(e.Message);
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
                _OnEnterHotFixFail("_OnHotFixDllLoadEnd Not All HotFix Dll Load Success");
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
                _OnEnterHotFixFail(e.Message);
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
                _OnEnterHotFixFail("_OnAOTDllLoadEnd Not All AOT Dll Load Success");
            }
        }

        private void _EnterHotFix()
        {
            InvokeHotFixStaticMethod(CHotFixSetting.GetHotFixEntryDllName(), CHotFixSetting.GetHotFixEntryClassFullName("CHotFixEntry"), "Enter");
        }

        private void _InitHotFixDllInEditor()
        {
            foreach (string szDllName in CHotFixSetting.ms_arrHotFixDllName)
            {
                Assembly asm = _GetHotFixAssInEditor(szDllName);
                if (null != asm)
                {
                    m_mapHotFixAss.Add(szDllName, asm);
                }
                else
                {
                    string szErrorMsg = Utility.Text.Format("_InitHotFixDllInEditor Fail Dll = {0}", szDllName);
                    _OnEnterHotFixFail(szErrorMsg);
                }
            }
        }

        private Assembly _GetHotFixAssInEditor(string a_szDllName)
        {
            Assembly[] arrAllAss = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in arrAllAss)
            {
                if (string.Compare(a_szDllName, asm.GetName().Name, StringComparison.Ordinal) == 0)
                {
                    return asm;
                }
            }
            return null;
        }

        private void _OnEnterHotFixFail(string a_szErrorMsg)
        {
            Log.Fatal("EnterHotFixFail Msg = {0}", a_szErrorMsg);
        }
        private Assembly _GetAsmByName(string a_szDllName)
        {
            if (m_mapHotFixAss.ContainsKey(a_szDllName))
            {
                return m_mapHotFixAss[a_szDllName];
            }
            else
            {
                return null;
            }
        }

       

        public void InvokeHotFixStaticMethod(string a_szDllName, string a_szClassFullName, string a_szMethodName)
        {
            Assembly asm = _GetAsmByName(a_szDllName);
            if (asm == null)
            {
                Log.Warning("InvokeHotFixStaticMethod Fail Dll is Null DllName={0} ClassFullName={1} a_szMethodName = {2}", a_szDllName, a_szClassFullName, a_szMethodName);
                return;
            }
            Type type = asm.GetType(a_szClassFullName);
            if (null == type)
            {
                Log.Warning("InvokeHotFixStaticMethod Fail Type is Null DllName={0} ClassFullName={1} a_szMethodName = {2}", a_szDllName, a_szClassFullName, a_szMethodName);
                return;
            }
            MethodInfo method = type.GetMethod(a_szMethodName);
            if (null == method)
            {
                Log.Warning("InvokeHotFixStaticMethod Fail method is Null DllName={0} ClassFullName={1} a_szMethodName = {2}", a_szDllName, a_szClassFullName, a_szMethodName);
                return;
            }
            Action fn = (Action)System.Delegate.CreateDelegate(typeof(Action), null, method);
            fn();
        }

        public object CreateHotFixInstance(string a_szDllName, string a_szClassFullName, params object[] args)
        {
            Assembly asm = _GetAsmByName(a_szDllName);
            if (asm == null)
            {
                Log.Warning("CreateHotFixInstance Fail Dll is Null DllName={0} ClassFullName={1}", a_szDllName, a_szClassFullName);
                return null;
            }
            Type type = asm.GetType(a_szClassFullName);
            if (null == type)
            {
                Log.Warning("CreateHotFixInstance Fail Type is Null DllName={0} ClassFullName={1}", a_szDllName, a_szClassFullName);
                return null;
            }
            return Activator.CreateInstance(type, args);
        }
    }
}

