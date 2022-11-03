using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrameworkPackage
{
    public class CResourceMgr : ISingleton<CResourceMgr>
    {
        private Dictionary<string, Object> m_mapRes = new Dictionary<string, Object>();

        public GameObject GetRes(string a_szResPath)
        {
            if (!m_mapRes.ContainsKey(a_szResPath))
            {
                Object obj = Resources.Load(a_szResPath);
                if(null == obj)
                {
                    return null;
                }
                m_mapRes.Add(a_szResPath, obj);
            }
            return m_mapRes[a_szResPath] as GameObject;
        }

        public void CleanUnUseRes()
        {
            //Resources.UnloadUnusedAssets();
        }


    }

    public static class CResExtend
    {
        public static Material GetMaterial(this Renderer a_render)
        {
#if UNITY_EDITOR
            return a_render.material;
#else
        return a_render.sharedMaterial;
#endif
        }
    }

}
