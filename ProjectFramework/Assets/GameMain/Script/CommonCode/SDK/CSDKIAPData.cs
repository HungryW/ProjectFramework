
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using System.Threading.Tasks;
using LitJson;
using System;
using System.Reflection;

namespace GameFrameworkPackage
{
    public enum EIAPProductType
    {
        Consumable = 0,
        NonConsumable = 1,
        Subscription = 2,
    }

    public class CSDKIAPProductInfo
    {
        public static CSDKIAPProductInfo ms_empty = new CSDKIAPProductInfo(null, null, null, 0, null, EIAPProductType.NonConsumable);

        public string sku { get; private set; }
        public string name { get; private set; }
        public string desc { get; private set; }
        public double price { get; private set; }
        public string currencyCode { get; private set; }
        public EIAPProductType type { get; private set; }

        public CSDKIAPProductInfo(string a_szSku, string a_szName, string a_szDesc, double a_dPrice, string a_szCurrencyCode, EIAPProductType a_eProduceType)
        {
            sku = a_szSku;
            name = a_szName;
            desc = a_szDesc;
            price = a_dPrice;
            currencyCode = a_szCurrencyCode;
            type = a_eProduceType;
        }
    }

    public class CIAPBuyExtraInfo
    {
        public int group_id { get; private set; }
        public int shop_id { get; private set; }
        public int goods_id { get; private set; }
        public string sz_extra { get; private set; }

        public CIAPBuyExtraInfo(int a_group_id, int a_shop_id, int a_goods_id, string a_szExtra)
        {
            group_id = a_group_id;
            shop_id = a_shop_id;
            goods_id = a_goods_id;
            sz_extra = a_szExtra;
        }

        public CIAPBuyExtraInfo()
        {
            group_id = -1;
            shop_id = -1;
            goods_id = -1;
            sz_extra = null;
        }
    }


    public static class CIAPConfigMgr
    {
        //public static string GetSkuIdByPlatform(this DRIapId a_dr, string a_szSkuFiledName)
        //{
        //    Type t = a_dr.GetType();
        //    PropertyInfo field = t.GetProperty(a_szSkuFiledName);
        //    object var = field.GetValue(a_dr);
        //    return var.ToString();
        //}
    }

    public class CIAPDeal
    {
        public string TransactionID;
        public string PayItem;//sku*number
        public CIAPBuyExtraInfo ExtraInfo;
        public string CISOrderID;
        /// <summary>
        /// 订单是否有效
        /// </summary>
        public int Validated { get; set; } = 1;
        /// <summary>
        /// 订单支付时间，in second
        /// </summary>
        public long PurchaseAt;
        public string clientID;
        public string urlCode; //QR url

        public long CancellationDataMs;
    }
}

