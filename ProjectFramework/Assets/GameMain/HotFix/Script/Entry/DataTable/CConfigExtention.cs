using Defines;
using GameFramework;
using GameFrameworkPackage;
using System;
using System.Collections;

namespace HotFixEntry
{
    public static class CConfigExtention
    {
        public static string GetAssetFullName(this DRUIForm a_dr)
        {
            return CAssestPathUtility.GetUIFormAsset(a_dr.AssetName);
        }

        public static string GetClassName(this DRUIForm a_dr)
        {
            return Utility.Text.Format("{0}.UI.C{1}", a_dr.DllName, a_dr.AssetName);
        }

        public static Type GetStateType(this DRGameState a_dr)
        {
            return Type.GetType(Utility.Text.Format("HotFixEntry.{0}", a_dr.GameStateName));
        }

        public static string GetAssetFullName(this DRParticle a_dr)
        {
            return CAssestPathUtility.GetParticleAsset(a_dr.GroupName, a_dr.AssetName);
        }
        public static string GetFullClassName(this DRParticle a_dr)
        {
            return Utility.Text.Format("{0}.Entity.{1}", CHotFixSetting.GetHotFixLogicDllName(), a_dr.ClassName);
        }
    }
}