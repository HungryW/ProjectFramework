using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CTimeStampMgr : SingletonWithGameObject<CTimeStampMgr>
    {
        private long m_lLastSynTime;
        private float m_fLastSynRunTime;

        private bool m_bIsInit = false;

        public void Init()
        {
            if (m_bIsInit)
            {
                return;
            }
            m_bIsInit = true;
            TimeSpan st = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            SetCurTimeStamp(Convert.ToInt64(st.TotalMilliseconds));
        }

        public void Clean()
        {
            if (!m_bIsInit)
            {
                return;
            }
            m_bIsInit = false;
        }

        public void SetCurTimeStamp(long a_lStamp)
        {
            m_lLastSynTime = a_lStamp;
            m_fLastSynRunTime = Time.realtimeSinceStartup;
        }

        public long GetCurTimeStampSed()
        {
            return GetCurTimeStampMillSed() / 1000;
        }

        public long GetCurTimeStampMillSed()
        {
            long fDiff1000 = (long)((Time.realtimeSinceStartup - m_fLastSynRunTime) * 1000f);
            return fDiff1000 + m_lLastSynTime;
        }
    }
}
