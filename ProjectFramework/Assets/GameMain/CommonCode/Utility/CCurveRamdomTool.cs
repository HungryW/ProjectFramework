using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    [DisallowMultipleComponent]
    public class CCurveRamdomTool : MonoBehaviour
    {
        protected static CCurveRamdomTool s_instance;
        public static CCurveRamdomTool Instance
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject PrefabRandomTool = CResourceMgr.Instance.GetRes(CAssestPathUtility.GetResToolPrefab("PrefabCurveRandomTool"));
                    return Instantiate(PrefabRandomTool).GetComponent<CCurveRamdomTool>();
                }
                else
                {
                    return s_instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }


        [Tooltip("使用说明:\n" +
            "1.曲线的起始点和结束点为(0,0)到(1,1)\n" +
            "2.是通过斜率来控制概率的,X轴表示概率，Y轴表示取值范围\n" +
            "3.可通过增加控制点控制不同取值范围的概率\n" +
            "（例如加一个控制点(0.05,0.2)表示前20%的数的概率为5%)在前边除了(0,0)点没有其他控制点的情况下\n" +
            "（例如加一个控制点(0.95,0.8)表示后20%的数的概率为5%)在后边除了(1,1)点没有其他控制点的情况下\n")]
        [Header("概率曲线参考例子")]
        [SerializeField]
        private AnimationCurve CurveExample;

        public int GetRandomVal(int a_nMin, int a_nMax)
        {
            return CurveExample.RandomRange(a_nMin, a_nMax);
        }
        [Header("在下面添加每个逻辑需要的概率曲线")]
        [Space]
        [HideInInspector]
        public bool bNULL;
    }
}

