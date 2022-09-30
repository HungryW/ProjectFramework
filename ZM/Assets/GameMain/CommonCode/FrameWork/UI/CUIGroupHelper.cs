using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class CUIGroupHelper : UIGroupHelperBase
    {
        public const int MC_nDepthFactor = 10000;
        private int m_nDepth = 0;
        private Canvas m_Canvas = null;

        public override void SetDepth(int a_nDepth)
        {
            m_nDepth = a_nDepth;
            m_Canvas.overrideSorting = true;
            m_Canvas.sortingOrder = MC_nDepthFactor * a_nDepth;
        }

        private void Awake()
        {
            m_Canvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            m_Canvas.overrideSorting = true;
            m_Canvas.sortingOrder = MC_nDepthFactor * m_nDepth;

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
        }
    }
}

