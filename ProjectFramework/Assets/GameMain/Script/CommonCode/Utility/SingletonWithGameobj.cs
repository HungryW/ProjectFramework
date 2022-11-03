using UnityEngine;
namespace GameFrameworkPackage
{
    public class SingletonWithGameObject<T> : MonoBehaviour where T : SingletonWithGameObject<T>
    {
        public static T Instance
        {
            get
            {
                if (m_bIsQuit)
                {
                    return null;
                }
                if (instance == null)
                {
                    T[] managers = Object.FindObjectsOfType(typeof(T)) as T[];
                    if (managers.Length != 0)
                    {
                        if (managers.Length == 1)
                        {
                            instance = managers[0];
                            instance.gameObject.name = typeof(T).Name;
                            return instance;
                        }
                        else
                        {
                            Debug.LogError("Class " + typeof(T).Name + " exists multiple times in violation of singleton pattern. Destroying all copies");
                            foreach (T manager in managers)
                            {
                                Destroy(manager.gameObject);
                            }
                        }
                    }
                    var go = new GameObject(typeof(T).Name, typeof(T));
                    instance = go.GetComponent<T>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
            set
            {
                instance = value as T;
            }
        }
        private static T instance;
        private static bool m_bIsQuit = false;

        private void OnDestroy()
        {
            m_bIsQuit = true;
        }

    }

}

