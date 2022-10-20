using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class LayerMask
    {
        public const int mc_nLayerEverything = ~0;
        public const int mc_nLayerNothing = 0;
        public const int mc_nLayerDefault = 1 << 0;
        public const int mc_nLayerUI = 1 << 5;
    }

    public static class CameraCullMaskExtend
    {
        public static void SetCullMaskEverything(this Camera camera)
        {
            camera.cullingMask = LayerMask.mc_nLayerEverything;
        }

        public static void SetCullMaskUI(this Camera camera)
        {
            camera.cullingMask = LayerMask.mc_nLayerUI;
        }

        public static void SetCullMask(this Camera camera)
        {
            camera.cullingMask = LayerMask.mc_nLayerDefault;
        }
    }

}
