using Defines;
using HotFixEntry;
using HotFixLogic.Entity;
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
            BtnConfirm.onClick.AddListener(_OnConfirmClick);
            BtnCancel.onClick.AddListener(_OnCnacelClick);
            BtnClose.onClick.AddListener(() => { LbContent.text = "关闭点击了"; });
        }

        private void _OnConfirmClick()
        {
            LbContent.text = "确认点击了";
            _PlayUIPar(EParticleID.Chouka_star);
        }

        private void _OnCnacelClick()
        {

            DRIapId dr = CHotFixEntry.DataTable.GetDataTable<DRIapId>().MinIdDataRow;
            LbContent.text = "取消点击了成功吧热更?我是安卓啦啦啦啦啦" + dr.iosLink;
            _PlayUIPar(EParticleID.Huizhang_shengji01);
        }

        private void _PlayUIPar(EParticleID a_eParId)
        {
            CParticleUIEntity.PlayPar(a_eParId, LbContent.transform, null);
        }
    }
}

