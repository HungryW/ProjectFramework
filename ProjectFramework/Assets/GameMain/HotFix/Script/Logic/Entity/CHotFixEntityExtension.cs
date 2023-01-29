using Defines;
using HotFixEntry;
using GameFramework;
using GameFrameworkPackage;

namespace HotFixLogic.Entity
{
    public static class CHotFixEntityExtension
    {
        public static int ShowParticle(this CFrameWorkEntity a_FrameWorkEntity, CParticleEntityData a_data)
        {
            DRParticle dr = a_data.GetConfig();
            if (null == dr)
            {
                return -1;
            }
            return a_FrameWorkEntity.ShowEntity(a_data, dr.GroupName, dr.GetAssetFullName(), CHotFixSetting.GetHotFixLogicDllName(), dr.GetFullClassName());
        }


    }

}

