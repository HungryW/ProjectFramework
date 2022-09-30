using System;
using System.Linq;
using System.Collections.Generic;

namespace GameFrameworkPackage
{
    public static class IdFactory
    {
        private static int ms_nTempIdSeed = 0;
        private static long ms_lStoreIdSeed = 0;

        public static void Init(long a_lStoreIdBase)
        {
            ms_lStoreIdSeed = a_lStoreIdBase;
        }

        public static int NewTempId()
        {
            ms_nTempIdSeed++;
            return ms_nTempIdSeed;
        }

        public static long NewStoredId()
        {
            ms_lStoreIdSeed++;
            return ms_lStoreIdSeed;
        }
    }
}