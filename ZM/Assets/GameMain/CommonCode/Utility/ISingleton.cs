using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public abstract class ISingleton<T> where T : new()
    {
        private static T ms_instance;

        public static T Instance
        {
            get
            {
                if (null == ms_instance)
                {
                    ms_instance = new T();
                }

                return ms_instance;
            }
        }

    }
}
