using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;
using GameFrameworkPackage;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public class CUIToolAnimSound : MonoBehaviour
    {
        public void PlaySound( ESoundID a_eSound )
        {
            CGameEntryMgr.Sound.PlaySound((int)a_eSound);
        }
    }

}
