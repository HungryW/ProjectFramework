using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// </summary>
namespace GameFrameworkPackage
{
    public class CUILoadingBg : MonoBehaviour
    {
        private Canvas m_Canvas;
        void Start()
        {
            m_Canvas = gameObject.GetOrAddComponent<Canvas>();
            m_Canvas.worldCamera = CCamera.GetUICamera();
        }

        public void PlayAnim()
        {
        }

        public void StopAnim()
        {
        }
    }
}



