using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{

    public static class CConstDevResolution
    {
        public const float Width = 19.2f;
        public const float W_H_Radio = 1.0f * 16 / 9;
        public const float High = Width / W_H_Radio;
        public const float DefaultCameraSize = High / 2;

        public const float mc_fUIResolutionW = 1920;
        public const float mc_fUIResolutionH = 1080;
    }

    public static class CConstAssetPriority
    {
        public const int ConfigAsset = 100;
        public const int DataTableAsset = 100;
        public const int DictionaryAsset = 100;
        public const int FontAsset = 50;
        public const int MusicAsset = 20;
        public const int SceneAsset = 0;
        public const int SoundAsset = 30;
        public const int UIFormAsset = 50;
        public const int UISoundAsset = 30;
        public const int Entity = 30;
    }

    public static class CConstProcedureData
    {
        public const string szNextScene = "NextScene";
        public const string szProcedureNamespace = "GameProcedure.";
    }

}

