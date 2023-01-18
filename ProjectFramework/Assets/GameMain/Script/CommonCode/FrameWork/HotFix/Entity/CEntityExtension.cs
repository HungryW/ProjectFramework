using GameFramework;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public static class CEntityExtension
    {
        public static void ShowEntity(this EntityComponent entityComponent, string a_szEntityGroup, string a_szAssetFullPath, string a_szTypeDllName, string a_szTypeClassName, int a_nId, object a_oUserData)
        {
            CHotFixLogicEntityData param = CHotFixLogicEntityData.Create(a_szTypeDllName, a_szTypeClassName, a_oUserData);
            entityComponent.ShowEntity(a_nId, typeof(CHotFixLogicEntity), a_szAssetFullPath, a_szEntityGroup, CConstAssetPriority.Entity, param);
        }

        public static void HideEntity(this EntityComponent entityComponent, EntityLogic a_entityLogic, object a_oUserData = null)
        {
            entityComponent.HideEntity(a_entityLogic.Entity, a_oUserData);
        }
    }
}


