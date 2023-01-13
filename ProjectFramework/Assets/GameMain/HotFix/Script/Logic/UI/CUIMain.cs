using Defines;
using GameFrameworkPackage;
using HotFixEntry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFixLogic.UI
{
    public class CUIMain : CUIBase
    {
        private GameObject GoContent;
        private Button BtnClose;
        private Image ImgBg;
        private Text LbContent;
        private Button BtnConfirm;
        private Text LbConfirm;
        private Button BtnCancel;
        private Text LbCancel;
        protected override void _InitComponents()
        {
            GoContent = transform.Find("GoContent").gameObject;
            BtnClose = transform.Find("GoContent/BtnClose").GetComponent<Button>();
            ImgBg = transform.Find("GoContent/ImgBg").GetComponent<Image>();
            LbContent = transform.Find("GoContent/LbContent").GetComponent<Text>();
            BtnConfirm = transform.Find("GoContent/BtnConfirm").GetComponent<Button>();
            LbConfirm = transform.Find("GoContent/BtnConfirm/LbConfirm").GetComponent<Text>();
            BtnCancel = transform.Find("GoContent/BtnCancel").GetComponent<Button>();
            LbCancel = transform.Find("GoContent/BtnCancel/LbCancel").GetComponent<Text>();
        }

        protected override void _OnInit(object userData)
        {
            base._OnInit(userData);
            BtnConfirm.onClick.AddListener(() =>
            {
                LbContent.text = "确认点击了";
                CHotFixEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Restart);
            });
            BtnCancel.onClick.AddListener(() =>
            {
                DRUIForm dr = CHotFixEntry.DataTable.GetDataTable<DRUIForm>().MaxIdDataRow;
                LbContent.text = "取消点击了成功吧热更?" + dr.AssetName;

            });
            BtnClose.onClick.AddListener(() => { LbContent.text = "关闭点击了"; });
        }
    }
}

