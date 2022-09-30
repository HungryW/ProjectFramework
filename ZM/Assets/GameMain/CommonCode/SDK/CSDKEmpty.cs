using Defines.DataTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CSDKEmpty : CSDKBase
    {
        public override void BindPlatform(ELoginSDK a_eSdk, Action a_fnOnLoginSucess, Action<string> a_fnOnLoginFail)
        {
            a_fnOnLoginSucess.SafeInvoke();
        }

        public override void BuyProduce(int a_nIapId, CIAPBuyExtraInfo a_extraInfo, Action<CIAPDeal> a_fnOnBuySuccess, Action<CIAPDeal, string> a_fnOnBuyFail)
        {
        }

        public override bool CheckPlatformBind(ELoginSDK a_eSdk)
        {
            return false;
        }

        public override void Clean()
        {

        }

        public override List<string> GetAllUnProcessedDeal()
        {
            return new List<string>();
        }

        public override EAppChannel GetAppChannel()
        {
            return EAppChannel.dev;
        }

        public override ELoginSDK GetDefaultLoginSdkType()
        {
            return ELoginSDK.DeviceLoginSDKInfo;
        }

        public override CSDKLoginInfo GetLoginInfo()
        {
            return new CSDKLoginInfo("", "");
        }

        public override ELoginSDK GetLoginSdkType()
        {
            return ELoginSDK.DeviceLoginSDKInfo;
        }

        public override string GetPayChannel()
        {
            return GetAppChannel().ToString();
        }

        public override CSDKIAPProductInfo GetProduceInfo(int a_nIAPId)
        {
            DRIapId dr = CGameEntryMgr.DataTable.GetDataRow<DRIapId>(a_nIAPId);
            CSDKIAPProductInfo produce = new CSDKIAPProductInfo(dr.testLink, dr.name, dr.name, dr.cost, "$", EIAPProductType.Consumable); ;
            return produce;
        }

        public override void Init(Action a_fnOnInitSuccess, Action<string> a_fnOnInitFail)
        {
            a_fnOnInitSuccess.SafeInvoke();
        }

        public override void InitIap()
        {
        }

        public override void LogEvent(string category, string action, string label, Hashtable data)
        {

        }

        public override void Login(ELoginSDK a_eSdk, Action a_fnOnLoginSucess, Action<string> a_fnOnLoginFail)
        {
            a_fnOnLoginSucess.SafeInvoke();
        }

        public override void Logout(Action a_fnOnSuccess, Action a_fnOnFail)
        {
            a_fnOnSuccess.SafeInvoke();
        }

        public override void LogTransaction(string itemName, string transId, int iapId, Hashtable data, List<string> specifiedSDKs)
        {
            
        }

        public override void MakeDealsProcessed(List<string> a_listTransactionId)
        {

        }

        public override void RefreshProduce(Action a_fnOnRefreshSuccess, Action<string> a_fnOnRefreshFail)
        {
            a_fnOnRefreshSuccess.SafeInvoke();
        }

        public override void RestoreUnProcessedDeal(Action a_fnOnRestoreDealSuccess, Action<string> a_fnOnRestoreDealFail)
        {

        }
    }
}
