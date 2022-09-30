using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public static class CUISpriteMgr
    {
        private static Dictionary<string, GameObject> m_mapPrfabs = new Dictionary<string, GameObject>();
        private static Dictionary<string, Dictionary<string, Sprite>> m_mapSprite = new Dictionary<string, Dictionary<string, Sprite>>();

        public static void Clean()
        {
            m_mapPrfabs.Clear();
            m_mapSprite.Clear();
        }

        public static void AddSpritePrefab(string a_szName, GameObject a_oPrefab)
        {
            if (!m_mapPrfabs.ContainsKey(a_szName))
            {
                m_mapPrfabs.Add(a_szName, a_oPrefab);
                m_mapSprite.Add(a_szName, new Dictionary<string, Sprite>());
            }
        }

        public static Sprite GetSprite(string a_szPath)
        {
            string[] arrPath = a_szPath.Split('/');
            if (arrPath.Length != 2)
            {
                return null;
            }
            return GetSprite(arrPath[0], arrPath[1]);
        }

        public static bool CheckPrefabPreload(string a_szPrefabName)
        {
            return m_mapPrfabs.ContainsKey(a_szPrefabName);
        }

        public static Sprite GetSprite(string a_szPrefabName, string a_szSpriteName)
        {
            if (!m_mapSprite.ContainsKey(a_szPrefabName))
            {
                Log.Debug("GetSprite fail prefab={0} not exist", a_szPrefabName);
                return null;
            }

            if (!m_mapSprite[a_szPrefabName].ContainsKey(a_szSpriteName))
            {
                Transform tran = m_mapPrfabs[a_szPrefabName].transform.Find(a_szSpriteName);
                if (tran == null)
                {
                    Log.Debug("GetSprite fail prefab={0}, a_szSpriteName ={1} not exist", a_szPrefabName, a_szSpriteName);
                    return null;
                }
                SpriteRenderer sprite = tran.GetComponent<SpriteRenderer>();
                if (sprite == null)
                {
                    Log.Debug("GetSprite fail prefab={0}, a_szSpriteName ={1} SpriteRenderer not exist", a_szPrefabName, a_szSpriteName);
                    return null;
                }
                m_mapSprite[a_szPrefabName].Add(a_szSpriteName, sprite.sprite);
            }
            return m_mapSprite[a_szPrefabName][a_szSpriteName];
        }
    }
}


