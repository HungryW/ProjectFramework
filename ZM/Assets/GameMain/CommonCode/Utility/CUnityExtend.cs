using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public static class CUnityExtend
    {
        public static Vector3 RandomWithinWorldPos(this RectTransform a_tran)
        {
            Vector2 v2Size = a_tran.sizeDelta;
            float fRandomX = MathUtility.TrueRandom((int)-(v2Size.x / 2), (int)(v2Size.x) / 2);
            float fRandomY = MathUtility.TrueRandom((int)-(v2Size.y / 2), (int)(v2Size.y) / 2);
            return a_tran.TransformPoint(new Vector2(fRandomX, fRandomY));
        }

        public static void SafeInvoke(this UnityAction a_fn)
        {
            if (a_fn != null)
            {
                a_fn.Invoke();
            }
        }
        //动画 获得动画片段时间长度
        public static float GetClipLength(this Animator animator, string a_szClipName)
        {
            AnimationClip clip = animator.GetClip(a_szClipName);
            if (null == clip)
            {
                return 0;
            }
            return clip.length;
        }

        public static AnimationClip GetClip(this Animator animator, string clip)
        {
            if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
                return null;
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            AnimationClip[] tAnimationClips = ac.animationClips;
            if (null == tAnimationClips || tAnimationClips.Length <= 0) return null;
            AnimationClip tAnimationClip;
            for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
            {
                tAnimationClip = ac.animationClips[tCounter];
                if (null != tAnimationClip && tAnimationClip.name == clip)
                    return tAnimationClip;
            }
            return null;
        }


        public static void PlayAnim(this Animation animation, string a_szAnimName, Action a_fnEndCallback)
        {
            AnimationClip anim = animation.GetClip(a_szAnimName);
            if (null == anim)
            {
                a_fnEndCallback.SafeInvoke();
                return;

            }
            float fLen = anim.length;
            bool b = animation.Play(a_szAnimName, PlayMode.StopAll);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(fLen)
                .AppendCallback(() => { a_fnEndCallback.SafeInvoke(); });
        }

        public static void PlayAnim(this Animator animator, string a_szAnimName, Action a_fnEndCallback)
        {
            float fLen = animator.GetClipLength(a_szAnimName);
            if (MathUtility.IsEqual(fLen, 0f))
            {
                a_fnEndCallback.SafeInvoke();
                return;
            }
            animator.Play(a_szAnimName, 0);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(fLen)
                .AppendCallback(() => { a_fnEndCallback.SafeInvoke(); });
        }

        public static void ResetDefaultState(this Animator anim, string a_szDefaultStateName)
        {
            anim.Play(a_szDefaultStateName);
            anim.Update(0f);
        }

        //Text 当文本超出范围时才设置为bestFit
        public static void SetTextWithBestFit(this Text textComponent, string value)
        {
            textComponent.resizeTextForBestFit = false;
            TextGenerator generator = new TextGenerator();
            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
            TextGenerationSettings settings = textComponent.GetGenerationSettings(rectTransform.rect.size);
            generator.Populate(value, settings);
            int characterCountVisible = generator.characterCountVisible;
            textComponent.resizeTextForBestFit = value.Length > characterCountVisible;
            textComponent.text = value;
        }

        //Transform 根据
        public static Transform GetChildByName(this Transform a_tranParent, string a_szName)
        {
            foreach (Transform child in a_tranParent)
            {
                if (child.name.Equals(a_szName))
                {
                    return child;
                }
            }
            return null;
        }

        public static void SetPivotHoldWorldPos(this RectTransform tran, Vector2 a_v2)
        {
            Vector3 v3LocalPos = tran.localPosition;
            Vector2 v2Offset = a_v2 - tran.pivot;
            v3LocalPos.x += v2Offset.x * tran.rect.width;
            v3LocalPos.y += v2Offset.y * tran.rect.height;
            tran.pivot = a_v2;
            tran.localPosition = v3LocalPos;
        }

        public static void SetAnchoredPosY(this RectTransform transform, float a_fVal)
        {
            Vector2 v = transform.anchoredPosition;
            v.y = a_fVal;
            transform.anchoredPosition = v;
        }

        public static void SetAnchoredPosX(this RectTransform transform, float a_fVal)
        {
            Vector2 v = transform.anchoredPosition;
            v.x = a_fVal;
            transform.anchoredPosition = v;
        }

        public static void AddAnchoredPosX(this RectTransform transform, float a_fVal)
        {
            Vector2 v = transform.anchoredPosition;
            v.x += a_fVal;
            transform.anchoredPosition = v;
        }

        public static void AddAnchoredPosY(this RectTransform transform, float a_fVal)
        {
            Vector2 v = transform.anchoredPosition;
            v.y += a_fVal;
            transform.anchoredPosition = v;
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            if (null == obj)
                return;
            Transform[] allTran = obj.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < allTran.Length; i++)
            {
                allTran[i].gameObject.layer = layer;
            }
        }


        #region 特效相关

        public static float GetMaxValue(this AnimationCurve animCurve)
        {
            Keyframe[] keyframes = animCurve.keys;
            if (keyframes.Length == 0)
            {
                return 0f;
            }
            return Mathf.Min(keyframes[0].value, keyframes[keyframes.Length - 1].value);
        }

        public static float GetMinValue(this AnimationCurve animCurve)
        {
            Keyframe[] keyframes = animCurve.keys;
            if (keyframes.Length == 0)
            {
                return 0f;
            }
            return Mathf.Max(keyframes[0].value, keyframes[keyframes.Length - 1].value);
        }

        public static float GetMaxValue(this ParticleSystem.MinMaxCurve minMaxCurve)
        {
            switch (minMaxCurve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return minMaxCurve.constant;
                case ParticleSystemCurveMode.Curve:
                    return minMaxCurve.curve.GetMaxValue();
                case ParticleSystemCurveMode.TwoConstants:
                    return minMaxCurve.constantMax;
                case ParticleSystemCurveMode.TwoCurves:
                    var ret1 = minMaxCurve.curveMin.GetMaxValue();
                    var ret2 = minMaxCurve.curveMax.GetMaxValue();
                    return ret1 > ret2 ? ret1 : ret2;
            }
            return -1f;
        }

        public static float GetMinValue(this ParticleSystem.MinMaxCurve minMaxCurve)
        {
            switch (minMaxCurve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return minMaxCurve.constant;
                case ParticleSystemCurveMode.Curve:
                    return minMaxCurve.curve.GetMinValue();
                case ParticleSystemCurveMode.TwoConstants:
                    return minMaxCurve.constantMin;
                case ParticleSystemCurveMode.TwoCurves:
                    var ret1 = minMaxCurve.curveMin.GetMinValue();
                    var ret2 = minMaxCurve.curveMax.GetMinValue();
                    return Mathf.Min(ret1, ret2);
            }
            return -1f;
        }

        public static float GetDuration(this ParticleSystem particle)
        {
            if (!particle.emission.enabled)
            {
                return 0f;
            }
            if (particle.main.loop)
            {
                return -1f;
            }
            if (particle.emission.rateOverTime.GetMinValue() <= 0)
            {
                return particle.main.startDelay.GetMaxValue() + particle.main.startLifetime.GetMaxValue();
            }
            else
            {
                return particle.main.startDelay.GetMaxValue() + Mathf.Max(particle.main.duration, particle.main.startLifetime.GetMaxValue());
            }
        }

        #endregion
    }
}

