using Defines;
using HotFixEntry;
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
                DRIapId dr = CHotFixEntry.DataTable.GetDataTable<DRIapId>().MinIdDataRow;
                LbContent.text = "取消点击了成功吧热更?我是安卓啦啦啦啦啦" + dr.iosLink;

            });
            BtnClose.onClick.AddListener(() => { LbContent.text = "关闭点击了"; });
        }
    }
}

