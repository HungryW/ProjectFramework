using GameFramework;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;

namespace HotFixEntry
{
    public partial class CGameStatePreloadResource : CGameStateBase
    {
        public override EGameStateID GetStateId()
        {
            return EGameStateID.CGameStatePreloadResource;
        }

        private enum EPreloadState
        {
            BeforeStart = 0,
            LoadingPreRes = 1,
            LoadingNormalRes = 2,
            LoadingAfterRes = 3,
            End = 4,
        }
        private Dictionary<string, bool> m_LoadedFlag;
        private EPreloadState m_eLoadState;

        public CGameStatePreloadResource()
        {
            m_LoadedFlag = new Dictionary<string, bool>();
            m_eLoadState = EPreloadState.BeforeStart;
        }

        private void _SetLoadState(EPreloadState a_eState)
        {
            m_eLoadState = a_eState;
            m_LoadedFlag.Clear();
        }

        private EPreloadState _GetLoadState()
        {
            return m_eLoadState;
        }

        protected override void OnInit(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnInit(a_fsm);
        }
        protected override void OnEnter(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnEnter(a_fsm);
            CGameEntryMgr.PreloadComponent.CreateLoadingUI();
            _SetLoadState(EPreloadState.BeforeStart);
            _StartLoadRes();

        }
        protected override void _SubscribeEvent()
        {
            base._SubscribeEvent();
            CGameEntryMgr.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            CGameEntryMgr.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            CGameEntryMgr.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            CGameEntryMgr.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
            CGameEntryMgr.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            CGameEntryMgr.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
        }
        protected override void OnLeave(IFsm<CGameStateMgr> a_fsm, bool isShutdown)
        {
            base.OnLeave(a_fsm, isShutdown);
            CGameEntryMgr.PreloadComponent.DelLoadingUI();
        }
        protected override void _UnSubsribeEvent()
        {
            base._UnSubsribeEvent();
            CGameEntryMgr.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            CGameEntryMgr.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            CGameEntryMgr.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            CGameEntryMgr.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
            CGameEntryMgr.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            CGameEntryMgr.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
        }

        protected override void OnUpdate(IFsm<CGameStateMgr> a_fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(a_fsm, elapseSeconds, realElapseSeconds);
            if (_GetLoadState() == EPreloadState.BeforeStart || _GetLoadState() == EPreloadState.End)
            {
                return;
            }
            if (!_CheckCurStateResLoadEnd())
            {
                return;
            }

            if (_GetLoadState() == EPreloadState.LoadingPreRes)
            {
                _StartLoadNormalRes();
                return;
            }

            if (_GetLoadState() == EPreloadState.LoadingNormalRes)
            {
                _StartLoadAfterRes();
                return;
            }

            if (_GetLoadState() == EPreloadState.LoadingAfterRes)
            {
                _SetLoadState(EPreloadState.End);
                ChangeState<CGameStateLogin>(a_fsm);
            }
        }
        private void _StartLoadRes()
        {
            _StartLoadPreRes();
        }

        private void _StartLoadPreRes()
        {
            _SetLoadState(EPreloadState.LoadingPreRes);
        }

        private void _StartLoadNormalRes()
        {
            _SetLoadState(EPreloadState.LoadingNormalRes);
            _LoadConfigs();
            _LoadDictionarys();
            _LoadDataTables();
            _LoadFonts();
            _LoadUISprites();
        }

        private void _StartLoadAfterRes()
        {
            _SetLoadState(EPreloadState.LoadingAfterRes);
        }
        private float _ClacLoadProgress()
        {
            int nLoadedNum = 0;
            foreach (var item in m_LoadedFlag)
            {
                if (item.Value)
                {
                    nLoadedNum++;
                }
            }
            if (_GetLoadState() == EPreloadState.LoadingNormalRes)
            {
                return 0.99f * nLoadedNum / m_LoadedFlag.Count;
            }
            else if (_GetLoadState() == EPreloadState.LoadingAfterRes)
            {
                return 0.99f + 0.01f * nLoadedNum / m_LoadedFlag.Count;
            }
            else if (_GetLoadState() == EPreloadState.End)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private bool _AddLoadingFlag(string a_szFlagKey)
        {
            if (m_LoadedFlag.ContainsKey(a_szFlagKey))
            {
                return false;
            }
            m_LoadedFlag.Add(a_szFlagKey, false);
            return true;
        }

        private void _SetLoaded(string a_szFlagKey)
        {
            if (m_LoadedFlag.ContainsKey(a_szFlagKey))
            {
                m_LoadedFlag[a_szFlagKey] = true;
            }
            CGameEntryMgr.PreloadComponent.UpdateLoadingUI(_ClacLoadProgress(), "Preloading");
        }

        private bool _CheckCurStateResLoadEnd()
        {
            IEnumerator<bool> iter = m_LoadedFlag.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                if (!iter.Current)
                {
                    return false;
                }
            }

            return true;
        }

        private void _LoadDataTables()
        {
            _LoadConstantDataTable("Init");
            string[] arrName = _GetDataTableNameList();
            foreach (var szName in arrName)
            {
                _LoadDataTable(szName);
            }
        }


        private void _LoadConfigs()
        {
        }

        private void _LoadDictionarys()
        {
            string[] arrName = _GetDictionaryNameList();
            foreach (var szName in arrName)
            {
                _LoadDictionary(szName);
            }
        }

        private void _LoadFonts()
        {
            _LoadFont("main.ttf");
            _LoadFont("light.ttf");
        }

        private void _LoadUISprites()
        {
            CUISpriteMgr.Clean();
        }
        private void _LoadFont(string fontName)
        {
            m_LoadedFlag.Add(string.Format("Font.{0}", fontName), false);
            CGameEntryMgr.Resource.LoadAsset(CAssestPathUtility.GetFontAsset(fontName),
                new LoadAssetCallbacks(
                    (assetName, asset, duration, userData) =>
                    {
                        _SetLoaded(Utility.Text.Format("Font.{0}", fontName));
                        CLogicUI.SetMainFont((Font)asset);
                        Log.Info("Load font '{0}' OK.", fontName);
                    },
                      (assetName, status, errorMessage, userData) =>
                      {
                          Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", fontName, assetName, errorMessage);
                      }
                    ));
        }


        private void _LoadCustomFont(CustomFont a_eFontName, string a_strFontName)
        {
            CGameEntryMgr.Resource.LoadAsset(CAssestPathUtility.GetCustomFontAsset(a_strFontName),
                new LoadAssetCallbacks(
                    (assetName, asset, duration, userData) =>
                    {
                        CUIFontMgr.Instance.AddFont(a_eFontName, (Font)asset);
                        Log.Info("Load font '{0}' OK.", a_strFontName);
                    },
                        (assetName, status, errorMessage, userData) =>
                        {
                            Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", a_strFontName, assetName, errorMessage);
                        }
                    ));
        }

        private void _LoadUISprite(string a_szName)
        {
            if (string.IsNullOrEmpty(a_szName))
            {
                return;
            }
            m_LoadedFlag.Add(Utility.Text.Format("UISprite.{0}", a_szName), false);
            CGameEntryMgr.Resource.LoadAsset(CAssestPathUtility.GetUISpriteAsset(a_szName),
                new LoadAssetCallbacks(
                    (assetName, asset, duration, userData) =>
                    {
                        _SetLoaded(Utility.Text.Format("UISprite.{0}", a_szName));
                        CUISpriteMgr.AddSpritePrefab(a_szName, (GameObject)asset);
                        Log.Info("Load UISprite '{0}' OK.", a_szName);
                    },
                      (assetName, status, errorMessage, userData) =>
                      {
                          Log.Error("Can not load UISprite '{0}' from '{1}' with error message '{2}'.", a_szName, assetName, errorMessage);
                      }
                    ));
        }


        private void _LoadConfig(string configName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("Config.{0}", configName), false);
        }

        private void _LoadDictionary(string dictionaryName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("Dictionary.{0}", dictionaryName), false);
            CGameEntryMgr.Localization.LoadDictionary(dictionaryName, this);
        }


        private void _LoadDataTable(string dataTableName)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                return;
            }
            m_LoadedFlag.Add(Utility.Text.Format("DataTable.{0}", dataTableName), false);
            string szAssetName = CAssestPathUtility.GetDataTableAsset(dataTableName, false);
            CGameEntryMgr.DataTable.LoadDataTable(dataTableName, szAssetName, this);
        }

        private void _LoadConstantDataTable(string a_szName)
        {
            if (string.IsNullOrEmpty(a_szName))
            {
                return;
            }
            m_LoadedFlag.Add(Utility.Text.Format("ConstantDataTable.{0}", a_szName), false);
            CGameEntryMgr.Resource.LoadAsset(CAssestPathUtility.GetDataTableAsset(a_szName, false),
                new LoadAssetCallbacks(
                    (assetName, asset, duration, userData) =>
                    {
                        _SetLoaded(Utility.Text.Format("ConstantDataTable.{0}", a_szName));
                        CConstantsTableMgr.Instance.Load(((TextAsset)asset).text);
                        Log.Info("Load ConstantDataTable '{0}' OK.", a_szName);
                    },
                      (assetName, status, errorMessage, userData) =>
                      {
                          Log.Error("Can not load ConstantDataTable '{0}' from '{1}' with error message '{2}'.", a_szName, assetName, errorMessage);
                      }
                    ));
        }


        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            LoadDataTableSuccessEventArgs ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            _SetLoaded(Utility.Text.Format("DataTable.{0}", ne.DataTableAssetName));
            Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
        }

        private void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            LoadDataTableFailureEventArgs ne = (LoadDataTableFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName, ne.DataTableAssetName, ne.ErrorMessage);
        }


        private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        {
            LoadConfigSuccessEventArgs ne = (LoadConfigSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            _SetLoaded(Utility.Text.Format("Config.{0}", ne.ConfigAssetName));
            Log.Info("Load config '{0}' OK.", ne.ConfigAssetName);
        }

        private void OnLoadConfigFailure(object sender, GameEventArgs e)
        {
            LoadConfigFailureEventArgs ne = (LoadConfigFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", ne.ConfigAssetName, ne.ConfigAssetName, ne.ErrorMessage);
        }

        private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            _SetLoaded(Utility.Text.Format("Dictionary.{0}", ne.DictionaryAssetName));
            Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        }

        private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName, ne.DictionaryAssetName, ne.ErrorMessage);
        }
    }
}