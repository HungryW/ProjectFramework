using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFrameworkPackage
{
    public class CUILongPressDownEvent : CUILongPressOrClickEventTrigger
    {
        public Action<int> m_fnLongPressDown;
        public bool IsSpeedUp = false;
        private bool IsPressDown = false;
        public float durationSpeedUp = 3f;
        public float tickTime = 0.2f;
        private int m_step;

        public void SetConstantSpeed(Action<int> a_fnDown ,float a_delayEnter = 1f,float a_tickTime = 0.2f)
        {
            Init(a_fnDown,a_delayEnter,a_tickTime);
            IsSpeedUp = false;
        }

        public void SetSpeedUp(Action<int> a_fnDown ,float a_delayEnter = 1f, float a_delaySpeedUp = 3f, float a_tickTime = 0.2f)
        {
            Init(a_fnDown, a_delayEnter, a_tickTime);
            durationSpeedUp = a_delaySpeedUp;
            IsSpeedUp = true;
        }

        private void Init(Action<int> a_fnDown, float a_delayEnter = 1f, float a_tickTime = 0.2f)
        {
            m_step = 1;
            onLongPress.AddListener(_OnLongPress);
            onLongPressEnd.AddListener(_OnLongPressEnd);
            m_fnLongPressDown = a_fnDown;
            durationThreshold = a_delayEnter;
            tickTime = a_tickTime;
        }

        private void _OnLongPress()
        {
            m_step = 1;
            IsPressDown = true;
            StartCoroutine(_PressDown());
            if (IsSpeedUp)
            {
                StartCoroutine(_SpeedUp());
            }
        }

        private IEnumerator _PressDown()
        {
            WaitForSeconds waitSec = new WaitForSeconds(tickTime);
            while (IsPressDown)
            {
                m_fnLongPressDown.Invoke(m_step);
                yield return waitSec;
            }
        }

        private IEnumerator _SpeedUp()
        {
            WaitForSeconds waitSec = new WaitForSeconds(durationSpeedUp);
            float pow = 1f;
            while (true)
            {
                if (pow < 5)
                {
                    pow+=0.3f;
                    m_step = m_step*(int)pow;
                }
                else
                {
                    yield break;
                }
                yield return waitSec;
            }
        }

        private void _OnLongPressEnd()
        {
            StopAllCoroutines();
            IsPressDown = false;
            
        }
    }
}
