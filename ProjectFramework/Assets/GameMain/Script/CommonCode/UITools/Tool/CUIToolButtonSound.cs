using UnityEngine;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public enum EBtnSound
    {
        Null = 0,
        BtnClose = 1,
        BtnClick = 2,
    }

    [RequireComponent(typeof(Button))]
    public class CUIToolButtonSound : MonoBehaviour
    {

        public EBtnSound clickSound = EBtnSound.Null;
        void ClickSound()
        {
            if(clickSound == EBtnSound.Null)
            {
                return;
            }
            //CGameEntryMgr.Sound.PlayUISound((int)clickSound);
        }

        private void Awake()
        {
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(ClickSound);
            }
        }
    }

}
