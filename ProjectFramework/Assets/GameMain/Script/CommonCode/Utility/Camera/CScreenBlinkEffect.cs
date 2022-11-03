using UnityEngine;

namespace GameFrameworkPackage
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CScreenBlinkEffect : MonoBehaviour
    {
        [Range(0f, 1f)]
        [Tooltip("苏醒进度")]
        public float progress;
        [Range(0, 4)]
        [Tooltip("模糊迭代次数")]
        public int blurIterations = 3;
        [Range(.2f, 3f)]
        [Tooltip("每次模糊迭代时的模糊大小扩散")]
        public float blurSpread = .6f;
        // [Range(1, 8)]
        // public int downSample = 2;

        public Shader shader;

        [SerializeField]
        Material material;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            material.SetFloat("_Progress", progress);
            material.SetFloat("_ArchHeight", progress/2f);

            if (progress < 1)
            {
                int rtW = src.width;
                int rtH = src.height;

                var buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
                buffer0.filterMode = FilterMode.Bilinear;

                Graphics.Blit(src, buffer0, material, 0);

                // int iterations = Mathf.RoundToInt(blurIterations - blurIterations * progress);
                float blurSize;
                for (int i = 0; i < blurIterations; i++)
                {
                    // 将progress(0~1)映射到blurSize(blurSize~0)
                    blurSize = 1f + i * blurSpread;
                    blurSize = blurSize - blurSize * progress;
                    material.SetFloat("_BlurSize", blurSize);

                    var buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                    Graphics.Blit(buffer0, buffer1, material, 1);

                    RenderTexture.ReleaseTemporary(buffer0);
                    buffer0 = buffer1;
                    buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                    Graphics.Blit(buffer0, buffer1, material, 2);

                    RenderTexture.ReleaseTemporary(buffer0);
                    buffer0 = buffer1;
                }
                Graphics.Blit(buffer0, dest);
                RenderTexture.ReleaseTemporary(buffer0);
            }
            else
            {
                material.SetFloat("_BlurSize", 0f);
                Graphics.Blit(src, dest);
            }
        }

        public void OnAwakeAnimFinished()
        {

        }

    }

}
