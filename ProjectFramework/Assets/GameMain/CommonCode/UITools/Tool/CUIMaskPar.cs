using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameworkPackage;

namespace GameFrameworkPackage
{
    public class CUIMaskPar : Mask
    {
        public void Init()
        {
            Vector3[] corners = new Vector3[4];
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.GetWorldCorners(corners);
            float minX = corners[0].x;
            float minY = corners[0].y;
            float maxX = corners[2].x;
            float maxY = corners[2].y;
            List<ParticleSystem> listPar = new List<ParticleSystem>();
            GetComponentsInChildren(true, listPar);
            foreach (ParticleSystem par in listPar)
            {
                par.GetComponent<Renderer>().GetMaterial().SetFloat("_MinX", minX);
                par.GetComponent<Renderer>().GetMaterial().SetFloat("_MinY", minY);
                par.GetComponent<Renderer>().GetMaterial().SetFloat("_MaxX", maxX);
                par.GetComponent<Renderer>().GetMaterial().SetFloat("_MaxY", maxY);
            }
        }
    }
}
