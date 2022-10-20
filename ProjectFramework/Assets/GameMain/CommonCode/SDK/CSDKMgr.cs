using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;

namespace GameFrameworkPackage
{
    //App 上架的渠道
    public enum EAppChannel
    {
        dev = -1,
        appstore,
        googleplay,
    }

    public abstract partial class CSDKBase
    {
        public abstract void Clean();

        public abstract void Init(Action a_fnOnInitSuccess, Action<string> a_fnOnInitFail);
        public abstract EAppChannel GetAppChannel();

        public abstract ELoginSDK GetDefaultLoginSdkType();
        public abstract ELoginSDK GetLoginSdkType();
        public abstract void Login(ELoginSDK a_eSdk, Action a_fnOnLoginSucess, Action<string> a_fnOnLoginFail);
        public abstract void BindPlatform(ELoginSDK a_eSdk, Action a_fnOnLoginSucess, Action<string> a_fnOnLoginFail);
        public abstract bool CheckPlatformBind(ELoginSDK a_eSdk);
        public abstract CSDKLoginInfo GetLoginInfo();
        public abstract void Logout(Action a_fnOnSuccess, Action a_fnOnFail);

        public abstract string GetPayChannel();
        public abstract void RefreshProduce(Action a_fnOnRefreshSuccess, Action<string> a_fnOnRefreshFail);
        public abstract CSDKIAPProductInfo GetProduceInfo(int a_nIAPId);
        public abstract void BuyProduce(int a_nIapId, CIAPBuyExtraInfo a_extraInfo, Action<CIAPDeal> a_fnOnBuySuccess, Action<CIAPDeal, string> a_fnOnBuyFail);
        public abstract void RestoreUnProcessedDeal(Action a_fnOnRestoreDealSuccess, Action<string> a_fnOnRestoreDealFail);
        public abstract List<string> GetAllUnProcessedDeal();
        public abstract void MakeDealsProcessed(List<string> a_listTransactionId);
        public abstract void LogEvent(string category, string action_val, string label_val, Hashtable param);
        public abstract void LogTransaction(string itemName, string transId, int iapId, Hashtable data, List<string> specifiedSDKs);
    }


    public partial class CSDKMgr : ISingleton<CSDKMgr>
    {
        private CSDKBase m_SdkLogic = null;
        private bool m_bIsInit = false;

        public void SetPreInitData(CSDKBase a_logic)
        {
            m_SdkLogic = a_logic;
        }

        public void Clean()
        {
            if (m_SdkLogic != null)
            {
                m_SdkLogic.Clean();
                m_SdkLogic = null;
            }
        }
    }

    public partial class CSDKMgr
    {
        #region 初始化
        public void Init(Action a_fnOnInitSuccess, Action<string> a_fnOnInitFail)
        {
            if (m_bIsInit)
            {
                return;
            }
#if UNITY_ANDROID || UNITY_IOS
            SetPreInitData(new CSDKCIS());
#else
            SetPreInitData(new CSDKEmpty());
#endif
            m_SdkLogic.Init(a_fnOnInitSuccess, a_fnOnInitFail);
            m_bIsInit = true;
            CAnalyticsMainProject.Instance.LogGameProgress("GameStart", "", null);
        }

        public EAppChannel GetAppChannel()
        {
            return m_SdkLogic.GetAppChannel();
        }

        public bool IsHaveSdk()
        {
            return m_SdkLogic.GetAppChannel() != EAppChannel.dev;
        }
        #endregion

        #region 登录

        public ELoginSDK GetDefaultLoginSdkType()
        {
            return m_SdkLogic.GetDefaultLoginSdkType();
        }

        public ELoginSDK GetLoginSdkType()
        {
            return m_SdkLogic.GetLoginSdkType();
        }

        public void Login(ELoginSDK a_eSdk, Action a_fnOnLoginSucess, Action<string> a_fnOnLoginFail)
        {
            m_SdkLogic.Login(a_eSdk, a_fnOnLoginSucess, a_fnOnLoginFail);
        }

        public void BindPlatform(ELoginSDK a_eSdk, Action a_fnOnLoginSucess, Action<string> a_fnOnLoginFail)
        {
            m_SdkLogic.BindPlatform(a_eSdk, a_fnOnLoginSucess, a_fnOnLoginFail);
        }

        public bool CheckPlatformBind(ELoginSDK a_eSdk)
        {
            return m_SdkLogic.CheckPlatformBind(a_eSdk);
        }

        public CSDKLoginInfo GetLoginInfo()
        {
            return m_SdkLogic.GetLoginInfo();
        }

        public void Logout(Action a_fnSuccess, Action a_fnFail)
        {
            m_SdkLogic.Logout(a_fnSuccess, a_fnFail);
        }
        #endregion

        #region 内购

        public string GetPayChannel()
        {
            return m_SdkLogic.GetPayChannel();
        }

        public void RefreshProduce(Action a_fnOnRefreshSuccess, Action<string> a_fnOnRefreshFail)
        {
            m_SdkLogic.RefreshProduce(a_fnOnRefreshSuccess, a_fnOnRefreshFail);
        }

        public CSDKIAPProductInfo GetProduceInfo(int a_nIAPId)
        {
            return m_SdkLogic.GetProduceInfo(a_nIAPId);
        }

        public void BuyProduce(int a_nIapId, CIAPBuyExtraInfo a_extraInfo, Action<CIAPDeal> a_fnOnBuySuccess, Action<CIAPDeal, string> a_fnOnBuyFail)
        {
            m_SdkLogic.BuyProduce(a_nIapId, a_extraInfo, a_fnOnBuySuccess, a_fnOnBuyFail);
        }

        public void RestoreUnProcessedDeal(Action a_fnOnRestoreDealSuccess, Action<string> a_fnOnRestoreDealFail)
        {
            m_SdkLogic.RestoreUnProcessedDeal(a_fnOnRestoreDealSuccess, a_fnOnRestoreDealFail);
        }

        public List<string> GetAllUnProcessedDeal()
        {
            return m_SdkLogic.GetAllUnProcessedDeal();
        }

        public void MakeDealsProcessed(List<string> a_listTransactionId)
        {
            m_SdkLogic.MakeDealsProcessed(a_listTransactionId);
        }
        #endregion

        public void LogEvent(string category, string action_val, string label_val, Hashtable param)
        {
            if (m_SdkLogic == null)
            {
                return;
            }
            m_SdkLogic.LogEvent(category, action_val, label_val, param);
        }

        public void LogTransaction(string itemName, string transId, int a_nIapId, Hashtable data, List<string> specifiedSDKs)
        {
            if (m_SdkLogic == null)
            {
                return;
            }
            m_SdkLogic.LogTransaction(itemName, transId, a_nIapId, data, specifiedSDKs);
        }
    }
}
