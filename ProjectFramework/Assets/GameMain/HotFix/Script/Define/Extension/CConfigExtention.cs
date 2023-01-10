using GameFramework;
using GameFrameworkPackage;
using System;
using System.Collections;

namespace Defines
{
    public static class CConfigExtention
    {
        public static string GetAssetFullName(this DRUIForm a_dr)
        {
            return CAssestPathUtility.GetUIFormAsset(a_dr.AssetName);
        }

        public static Type GetStateType(this DRGameState a_dr)
        {
            return Type.GetType(Utility.Text.Format("HotFixEntry.{0}", a_dr.GameStateName));
        }
    }
}