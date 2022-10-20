using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using Defines.DataTable;
using System;
using UnityEngine.SceneManagement;

namespace GameFrameworkPackage
{
    public class CGameStateChangeScene : CGameStateBase
    {
        private int m_nBackgroundMusicId = 0;
        private EGameStateID m_eNextState;
        private bool m_bIsChangeSceneFinish = false;
        public CGameStateChangeScene() : base(EGameStateID.CGameStateChangeScene)
        {

        }

        protected override void OnEnter(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnEnter(a_fsm);

            m_bIsChangeSceneFinish = false;

            CGameEntryMgr.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, _OnLoadSceneSuc);
            CGameEntryMgr.Event.Subscribe(LoadSceneFailureEventArgs.EventId, _OnLoadSceneFail);
            CGameEntryMgr.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, _OnLoadSceneUpdate);
            CGameEntryMgr.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, _OnLoadSceneDependencyAsset);

            CGameEntryMgr.Sound.StopAllLoadedSounds();
            CGameEntryMgr.Sound.StopAllLoadingSounds();

            CGameEntryMgr.Entity.HideAllLoadedEntities();
            CGameEntryMgr.Entity.HideAllLoadingEntities();

            CGameEntryMgr.Base.ResetNormalGameSpeed();

            DRGameState drState = _GetNextStateConfig(a_fsm);
            if (null == drState)
            {
                return;
            }

            CGameEntryMgr.Scene.LoadScene(CAssestPathUtility.GetSceneAsset(drState.SceneAssetName), CConstAssetPriority.SceneAsset, this);
            m_nBackgroundMusicId = CGameStateMgr.Instance.GetState((EGameStateID)drState.Id).GetBGMId();
            m_eNextState = (EGameStateID)drState.Id;
        }

        protected override void OnLeave(IFsm<CGameStateMgr> a_fsm, bool a_isShutDown)
        {
            m_bIsChangeSceneFinish = false;
            CGameEntryMgr.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, _OnLoadSceneSuc);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, _OnLoadSceneFail);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, _OnLoadSceneUpdate);
            CGameEntryMgr.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, _OnLoadSceneDependencyAsset);

            base.OnLeave(a_fsm, a_isShutDown);
        }

        protected override void OnUpdate(IFsm<CGameStateMgr> a_fsm, float a_fElapseTime, float a_fRealTime)
        {
            base.OnUpdate(a_fsm, a_fElapseTime, a_fRealTime);
            if (!m_bIsChangeSceneFinish)
            {
                return;
            }

            _ChangeToNextState(a_fsm);
        }

        private void _ChangeToNextState(IFsm<CGameStateMgr> a_fsm)
        {
            DRGameState drState = _GetNextStateConfig(a_fsm);
            if (null == drState)
            {
                return;
            }
            string[] arrSzLoadedSceneNames = CGameEntryMgr.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < arrSzLoadedSceneNames.Length; i++)
            {
                if (!arrSzLoadedSceneNames[i].Equals(CAssestPathUtility.GetSceneAsset(drState.SceneAssetName)))
                {
                    CGameEntryMgr.Scene.UnloadScene(arrSzLoadedSceneNames[i]);
                }
            }
            Type tNextState = Type.GetType("GameFrameworkPackage." + drState.GameStateName);
            ChangeState(a_fsm, tNextState);
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
            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);
            m_bIsChangeSceneFinish = true;
            if (m_nBackgroundMusicId > 0)
            {
                CGameEntryMgr.Sound.PlayMusic(m_nBackgroundMusicId, m_eNextState, true);
            }
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateEndChangeSceneArgs>().Fill(EGameStateID.CGameStateChangeScene, m_eNextState));
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

        private DRGameState _GetNextStateConfig(IFsm<CGameStateMgr> a_fsm)
        {
            int nStateId = a_fsm.GetData<VarInt32>(CConstGameStateMgr.szNextState).Value;
            DRGameState drState = CGameEntryMgr.DataTable.GetDataRow<DRGameState>(nStateId);
            if (null == drState)
            {
                Log.Warning("Can not load conf '{0}' from data table.", nStateId.ToString());
            }
            return drState;
        }
    }
}

