using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrameworkPackage
{
    public enum EPosDirect
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Center = 4,
        leftUp = 5,
        RightUp = 6,
        LeftDown = 7,
        RightDown = 8,
    }

    public class CUIToolFitPosParamItem
    {
        public EPosDirect m_ePivotDirect
        {
            get;
            private set;
        }

        public Vector3 m_v3WorldPos
        {
            get;
            private set;
        }

        public CUIToolFitPosParamItem(EPosDirect a_eDirectType, Vector3 a_v3WorldPos)
        {
            m_ePivotDirect = a_eDirectType;
            m_v3WorldPos = a_v3WorldPos;
        }

        public Vector2 CalcPivotPos()
        {
            Vector2 v2 = Vector2.zero;
            if (m_ePivotDirect == EPosDirect.Center)
            {
                v2 = new Vector2(0.5f, 0.5f);
            }
            else if (m_ePivotDirect == EPosDirect.Up)
            {
                v2 = new Vector2(0.5f, 1f);
            }
            else if (m_ePivotDirect == EPosDirect.Down)
            {
                v2 = new Vector2(0.5f, 0f);
            }
            else if (m_ePivotDirect == EPosDirect.Left)
            {
                v2 = new Vector2(0f, 0.5f);
            }
            else if (m_ePivotDirect == EPosDirect.Right)
            {
                v2 = new Vector2(1f, 0.5f);
            }
            else if (m_ePivotDirect == EPosDirect.leftUp)
            {
                v2 = new Vector2(0f, 1f);
            }
            else if (m_ePivotDirect == EPosDirect.LeftDown)
            {
                v2 = new Vector2(0f, 0f);
            }
            else if (m_ePivotDirect == EPosDirect.RightUp)
            {
                v2 = new Vector2(1f, 1f);
            }
            else if (m_ePivotDirect == EPosDirect.RightDown)
            {
                v2 = new Vector2(1f, 0f);
            }
            return v2;
        }

        public bool CheckPosInvalid(Vector2 a_v2ContentSize, Transform a_tranParent, Camera a_uiCamera)
        {
            if (null == a_uiCamera)
            {
                return false;
            }
            Vector2 v2ScreenRect = new Vector2(CConstDevResolution.mc_fUIResolutionW, CConstDevResolution.mc_fUIResolutionH);
            Vector2 v2PoivePos = CalcPivotPos();
            Vector2 v2ScreenPos = a_uiCamera.WorldToScreenPoint(m_v3WorldPos);
            Vector2 v2UIPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(a_tranParent as RectTransform, v2ScreenPos, a_uiCamera, out v2UIPos);
            if (v2UIPos.x > 0)
            {
                if (Mathf.Abs((v2ScreenRect.x / 2 - a_v2ContentSize.x * (1 - v2PoivePos.x))) < Mathf.Abs(v2UIPos.x))
                {
                    return false;
                }
            }
            else
            {
                if (Mathf.Abs((v2ScreenRect.x / 2 - a_v2ContentSize.x * (v2PoivePos.x))) < Mathf.Abs(v2UIPos.x))
                {
                    return false;
                }
            }
            if (v2UIPos.y > 0)
            {
                if (Mathf.Abs((v2ScreenRect.y / 2 - a_v2ContentSize.y * (1 - v2PoivePos.y))) < Mathf.Abs(v2UIPos.y))
                {
                    return false;
                }
            }
            else
            {
                if (Mathf.Abs((v2ScreenRect.y / 2 - a_v2ContentSize.y * (v2PoivePos.y))) < Mathf.Abs(v2UIPos.y))
                {
                    return false;
                }
            }
            return true;
        }


        public CUIToolFitPosParamItem CreateFixPosParam(Vector2 a_v2ContentSize, Transform a_tranParent, Camera a_uiCamera)
        {
            if (null == a_uiCamera)
            {
                return this;
            }
            Vector2 v2ScreenRect = new Vector2(CConstDevResolution.mc_fUIResolutionW, CConstDevResolution.mc_fUIResolutionH);
            Vector2 v2PoivePos = CalcPivotPos();
            Vector2 v2ScreenPos = a_uiCamera.WorldToScreenPoint(m_v3WorldPos);
            Vector2 v2UIPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(a_tranParent as RectTransform, v2ScreenPos, a_uiCamera, out v2UIPos);
            float fFixUIPosX = v2UIPos.x;
            float fFixUIPosY = v2UIPos.y;
            if (v2UIPos.x > 0)
            {
                fFixUIPosX = Mathf.Min(fFixUIPosX, v2ScreenRect.x / 2 - a_v2ContentSize.x * (1 - v2PoivePos.x));
            }
            else
            {
                fFixUIPosX = Mathf.Max(fFixUIPosX, -(v2ScreenRect.x / 2 - a_v2ContentSize.x * (v2PoivePos.x)));
            }
            if (v2UIPos.y > 0)
            {
                fFixUIPosY = Mathf.Min(fFixUIPosY, v2ScreenRect.y / 2 - a_v2ContentSize.y * (1 - v2PoivePos.y));
            }
            else
            {
                fFixUIPosY = Mathf.Max(fFixUIPosY, -(v2ScreenRect.y / 2 - a_v2ContentSize.y * (v2PoivePos.y)));
            }
            Vector3 v3FixUIPos = new Vector3(fFixUIPosX, fFixUIPosY, 0);
            Vector3 v3WorldPos = a_tranParent.TransformPoint(v3FixUIPos);
            CUIToolFitPosParamItem param = new CUIToolFitPosParamItem(this.m_ePivotDirect, v3WorldPos);
            return param;
        }

    }

    public class CUIToolFitPosParamMgr
    {
        private List<CUIToolFitPosParamItem> m_listParam;

        public CUIToolFitPosParamMgr(List<EPosDirect> a_listDirect, List<Vector3> a_listWorldPos)
        {
            m_listParam = new List<CUIToolFitPosParamItem>();
            int nLen = Mathf.Min(a_listDirect.Count, a_listWorldPos.Count);
            for (int i = 0; i < nLen; i++)
            {
                m_listParam.Add(new CUIToolFitPosParamItem(a_listDirect[i], a_listWorldPos[i]));
            }
        }

        public bool IsVaild()
        {
            return m_listParam.Count > 0;
        }

        public CUIToolFitPosParamItem GetFitPosParam(Vector2 a_v2ContentSize, Transform a_tranParent, Camera a_uiCamera)
        {
            if (!IsVaild())
            {
                return null;
            }
            foreach (var posParam in m_listParam)
            {
                if (posParam.CheckPosInvalid(a_v2ContentSize, a_tranParent, a_uiCamera))
                {
                    return posParam;
                }
            }
            return m_listParam[0].CreateFixPosParam(a_v2ContentSize, a_tranParent, a_uiCamera);
        }
    }


    public class CUIToolFitPos : MonoBehaviour
    {
        [SerializeField]
        public List<Transform> ListTranPos;
        [SerializeField]
        public List<EPosDirect> ListEPivotDirect;

        private CUIToolFitPosParamMgr m_param;

        public CUIToolFitPosParamMgr GetParam()
        {
            if (m_param == null)
            {
                List<Vector3> listWorldPos = new List<Vector3>();
                foreach (var tran in ListTranPos)
                {
                    listWorldPos.Add(tran.position);
                }
                m_param = new CUIToolFitPosParamMgr(ListEPivotDirect, listWorldPos);
            }
            return m_param;
        }
    }
}

