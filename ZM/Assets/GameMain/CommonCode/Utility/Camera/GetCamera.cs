using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class CCamera
    {
        private static Camera ms_CameraUI;
        private static Camera ms_CameraScene;
        private static Camera ms_CameraBlur;
        
        public static Camera GetUICamera()
        {
            if (ms_CameraUI == null)
            {
                ms_CameraUI = CGameEntryMgr._Instance.transform.Find("UICamera").GetComponent<Camera>();
            }
            return ms_CameraUI;
        }

        public static Camera GetSceneCamera()
        {
            if (ms_CameraScene == null)
            {
                ms_CameraScene = CGameEntryMgr._Instance.transform.Find("MainCamera").GetComponent<Camera>();
            }
            return ms_CameraScene;
        }

        public static Camera GetBlurCamera()
        {
            if (ms_CameraBlur == null)
            {
                ms_CameraBlur = CGameEntryMgr._Instance.transform.Find("BlurCamera").GetComponent<Camera>();
            }
            return ms_CameraBlur;
        }
    }
}
