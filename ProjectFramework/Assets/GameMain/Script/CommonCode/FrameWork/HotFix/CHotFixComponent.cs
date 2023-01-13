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
    public class CHotFixAssembly
    {
        public Assembly m_ass;
        public int m_nCrc;

        public CHotFixAssembly(Assembly ass, int nCrc)
        {
            m_ass = ass;
            m_nCrc = nCrc;
        }
    }

    public class CHotFixComponent : GameFrameworkComponent
    {
        private static Dictionary<string, CHotFixAssembly> ms_mapHotFixAss = new Dictionary<string, CHotFixAssembly>();
        private static bool ms_bIsHandledAotDll = false;

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
            CLoadSequenceAssetesTools.StartLoad(CHotFixSetting.GetAllHotFixResFullPath(), loadHotFixDllCallbacks, _OnHotFixDllLoadEnd);
        }

        private void _OnLoadHotFixAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                _OnEnterHotFixFail($"Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                CHotFixAssembly assLoaded = _GetAssByAssetName(assetName);
                if (null == assLoaded)
                {
                    Assembly asm = Assembly.Load(textAsset.bytes);
                    string szName = asm.GetName().Name;
                    ms_mapHotFixAss.Add(szName, new CHotFixAssembly(asm, Utility.Verifier.GetCrc32(textAsset.bytes)));
                    Log.Debug("Assembly [{0}] loaded", szName);
                }
                else
                {
                    int nNewDllHashCode = Utility.Verifier.GetCrc32(textAsset.bytes);
                    if (nNewDllHashCode != assLoaded.m_nCrc)
                    {
                        //如果重启的过程中更新了Dll,则直接退出游戏
                        _ToForceRestartGame();
                    }
                }

            }
            catch (Exception e)
            {
                _OnEnterHotFixFail(e.Message);
                throw;
            }
        }

        private void _ToForceRestartGame()
        {
            COpenParamCommonConfirm param = new COpenParamCommonConfirm(CGameEntryMgr.Localization.GetStringEX("HotFixDllReloadError"));
            param.SetShowCancel(false);
            param.SetConfirmCallback(() =>
            {
                GameEntry.Shutdown(ShutdownType.Quit);
            });
            CUIPreloadConfirm.Tip(param);
        }

        private CHotFixAssembly _GetAssByAssetName(string a_szAssetName)
        {
            foreach (var item in ms_mapHotFixAss)
            {
                if (a_szAssetName.Contains(item.Key))
                {
                    return item.Value;
                }
            }
            return null;
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
            if (ms_bIsHandledAotDll)
            {
                _OnAOTDllLoadEnd(true);
            }
            else
            {
                LoadAssetCallbacks loadAOTDllCallbacks = new LoadAssetCallbacks(_OnLoadAOTAssetSuccess, _OnLoadAOTAssetFail);
                CLoadGroupAssetesTools.StartLoad(CHotFixSetting.GetAllAOTResFullPath(), loadAOTDllCallbacks, _OnAOTDllLoadEnd);
            }
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
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(arrDllBytes, HomologousImageMode.SuperSet);
                    if (err != LoadImageErrorCode.OK)
                    {
                        Log.Warning("LoadMetadataForAOTAssembly Fail, DllName= {0}, Error = {1}", assetName, err);
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
                ms_bIsHandledAotDll = true;
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
                if (null != asm && !ms_mapHotFixAss.ContainsKey(szDllName))
                {
                    ms_mapHotFixAss.Add(szDllName, new CHotFixAssembly(asm, 0));
                }
                else
                {
                    Log.Info("_InitHotFixDllInEditor Fail Dll = {0}", szDllName);
                }
            }
            _EnterHotFix();

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
        public Assembly GetAsmByName(string a_szDllName)
        {
            if (ms_mapHotFixAss.ContainsKey(a_szDllName))
            {
                return ms_mapHotFixAss[a_szDllName].m_ass;
            }
            else
            {
                return null;
            }
        }



        public void InvokeHotFixStaticMethod(string a_szDllName, string a_szClassFullName, string a_szMethodName)
        {
            Assembly asm = GetAsmByName(a_szDllName);
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
            Assembly asm = GetAsmByName(a_szDllName);
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

