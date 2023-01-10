using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace HotFixLogic
{

    public partial class CGameStateChangeSceneTool
    {
        public delegate void fnOnChangeStateEnd(EGameStateID a_eState);
        private EGameStateID m_eCurState;
        private EGameStateID m_eNextState = (EGameStateID)(-1);
        private int m_nUnloadSceneNum = 0;
        private fnOnChangeStateEnd m_fnOnChangeSecneEnd;

        public CGameStateChangeSceneTool()
        {
            m_eCurState = (EGameStateID)(-1);
            m_eNextState = (EGameStateID)(-1);
            m_fnOnChangeSecneEnd = null;
        }

        public Type GetNextStateType()
        {
            DRGameState drState = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)m_eNextState);
            return Type.GetType("GameFrameworkPackage." + drState.GameStateName);
        }

        public void StartChangeState(EGameStateID a_eCurStateId, EGameStateID a_eNextStateId, fnOnChangeStateEnd a_fnOnChangeSceneEnd)
        {
            DRGameState drCurState = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)a_eCurStateId);
            DRGameState drState = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)a_eNextStateId);
            if (null == drState || drCurState == null)
            {
                Log.Warning("Can not load conf '{0},{1}' from data table.", (int)a_eCurStateId, (int)a_eNextStateId);
                return;
            }
            m_eCurState = a_eCurStateId;
            m_eNextState = a_eNextStateId;
            m_fnOnChangeSecneEnd = a_fnOnChangeSceneEnd;
            //在Ui中控制BGM
            //CGameEntryMgr.Sound.StopAllLoadedSounds();
            //CGameEntryMgr.Sound.StopAllLoadingSounds();
            CGameEntryMgr.Entity.HideAllLoadedEntities();
            CGameEntryMgr.Entity.HideAllLoadingEntities();
            CGameEntryMgr.Base.ResetNormalGameSpeed();

            CGameEntryMgr.Event.Subscribe(UnloadSceneSuccessEventArgs.EventId, _OnUnLoadSceneSuc);
            CGameEntryMgr.Event.Subscribe(UnloadSceneFailureEventArgs.EventId, _OnUnLoadSceneFail);
            CGameEntryMgr.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, _OnLoadSceneSuc);
            CGameEntryMgr.Event.Subscribe(LoadSceneFailureEventArgs.EventId, _OnLoadSceneFail);
            CGameEntryMgr.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, _OnLoadSceneUpdate);
            CGameEntryMgr.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, _OnLoadSceneDependencyAsset);

            CGameEntryMgr.Event.FireNow(null, ReferencePool.Acquire<CEventGameImmediatelyStateStartChangeSceneArgs>().Fill(m_eCurState, m_eNextState));
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateStartChangeSceneArgs>().Fill(m_eCurState, m_eNextState));
            _UnloadUnUseScene();
        }

        private void _UnloadUnUseScene()
        {
            m_nUnloadSceneNum = 0;
            DRGameState drState = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)m_eNextState);
            string[] arrSzLoadedSceneNames = CGameEntryMgr.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < arrSzLoadedSceneNames.Length; i++)
            {
                if (!arrSzLoadedSceneNames[i].Equals(CAssestPathUtility.GetSceneAsset(drState.SceneAssetName)))
                {
                    m_nUnloadSceneNum++;
                    CGameEntryMgr.Scene.UnloadScene(arrSzLoadedSceneNames[i], this);
                }
            }

            if (m_nUnloadSceneNum == 0)
            {
                _LoadNextScene();
            }
        }

        private void _OnUnLoadSceneSuc(object a_oSender, GameEventArgs e)
        {
            UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            m_nUnloadSceneNum--;
            if (m_nUnloadSceneNum == 0)
            {
                _LoadNextScene();
            }
            Log.Info("UnLoad scene '{0}' OK m_nUnloadSceneNum = {1}.", ne.SceneAssetName, m_nUnloadSceneNum);
        }

        private void _OnUnLoadSceneFail(object a_oSender, GameEventArgs e)
        {
            UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            Log.Error("UnLoad scene '{0}' failure,.", ne.SceneAssetName);
        }

        private void _LoadNextScene()
        {
            DRGameState drState = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)m_eNextState);
            CGameEntryMgr.Scene.LoadScene(CAssestPathUtility.GetSceneAsset(drState.SceneAssetName), CConstAssetPriority.SceneAsset, this);
        }

        private void _OnLoadSceneSuc(object a_oSender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }
            Scene scene = SceneManager.GetSceneByName(SceneComponent.GetSceneName(ne.SceneAssetName));
            SceneManager.SetActiveScene(scene);
            _EndChangeScene();
            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);
        }

        private void _EndChangeScene()
        {
            CGameEntryMgr.Event.Unsubscribe(UnloadSceneSuccessEventArgs.EventId, _OnUnLoadSceneSuc);
            CGameEntryMgr.Event.Unsubscribe(UnloadSceneFailureEventArgs.EventId, _OnUnLoadSceneFail);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, _OnLoadSceneSuc);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, _OnLoadSceneFail);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, _OnLoadSceneUpdate);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, _OnLoadSceneDependencyAsset);
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateEndChangeSceneArgs>().Fill(m_eCurState, m_eNextState));
            if (m_fnOnChangeSecneEnd != null)
            {
                m_fnOnChangeSecneEnd.Invoke(m_eNextState);
            }
        }

        private void _OnLoadSceneFail(object a_oSender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
        }

        private void _OnLoadSceneUpdate(object a_oSender, GameEventArgs e)
        {
            LoadSceneUpdateEventArgs ne = (LoadSceneUpdateEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' update, progress '{1}'.", ne.SceneAssetName, ne.Progress.ToString("P2"));
        }

        private void _OnLoadSceneDependencyAsset(object a_oSender, GameEventArgs e)
        {
            LoadSceneDependencyAssetEventArgs ne = (LoadSceneDependencyAssetEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' dependency asset '{1}', count '{2}/{3}'.", ne.SceneAssetName, ne.DependencyAssetName, ne.LoadedCount.ToString(), ne.TotalCount.ToString());
        }
    }
}

