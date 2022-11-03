using UnityEngine;

namespace GameFrameworkPackage
{
    public partial class CGameEntryMgr : MonoBehaviour
    {
        public static CGameEntryMgr _Instance { get; private set; }
        private void Start()
        {
            _Instance = this;
            InitBuiltinComponents();
            InitCustomComponents();
        }
    }
}

