using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace HotFixEntry
{
    public class CUIShieldRaycast
    {
        //做个容错 玩家点击mc_nMaxClickTime次后 遮罩自动关闭
        private const int mc_nMaxClickTime = 5;

        private GameObject m_GoShieldRaycast;
        private int m_nClickTime;

        public CUIShieldRaycast(Transform a_tranParent)
        {
            m_nClickTime = 0;

            m_GoShieldRaycast = new GameObject("ImgShieldRaycast");
            m_GoShieldRaycast.transform.SetParent(a_tranParent, false);
            m_GoShieldRaycast.transform.localScale = Vector3.one;
            m_GoShieldRaycast.transform.SetAsLastSibling();
            m_GoShieldRaycast.AddComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            m_GoShieldRaycast.GetComponent<RectTransform>().localPosition = Vector3.zero;
            Image img = m_GoShieldRaycast.AddComponent<Image>();
            img.color = Color.clear;
            Button btn = m_GoShieldRaycast.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(_OnClick);
        }

        public void SetActive(bool a_bIsActive)
        {
            m_GoShieldRaycast.SetActive(a_bIsActive);
            m_nClickTime = 0;
        }

        private void _OnClick()
        {
            m_nClickTime++;
            if (m_nClickTime > mc_nMaxClickTime)
            {
                SetActive(false);
                Log.Info("{0} CUIShieldRaycast Force Close", m_GoShieldRaycast.transform.parent.name);
            }
        }
    }

}
