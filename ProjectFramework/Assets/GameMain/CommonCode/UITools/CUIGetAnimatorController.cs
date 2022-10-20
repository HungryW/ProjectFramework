using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CUIGetAnimatorController:CLogicUI
    {
        [SerializeField]
        private RuntimeAnimatorController m_controller;

        public RuntimeAnimatorController GetController()
        {
            return m_controller;
        }
    }
}
