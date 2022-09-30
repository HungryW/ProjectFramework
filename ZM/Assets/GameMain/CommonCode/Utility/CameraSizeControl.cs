using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeControl : MonoBehaviour
{
    void Start()
    {
        float screenHeight = Screen.height;
        //this.GetComponent<Camera>().orthographicSize = screenHeight / 200.0f;

        float orthographicSize = this.GetComponent<Camera>().orthographicSize;
        float aspectRatio = Screen.width * 1.0f / Screen.height;
        float cameraWidth = orthographicSize * 2 * aspectRatio;
        if (cameraWidth < CConstDevResolution.Width)
        {
            orthographicSize = CConstDevResolution.Width / (2 * aspectRatio);
            Debug.Log("new orthographicSize = " + orthographicSize);
            this.GetComponent<Camera>().orthographicSize = orthographicSize;
        }
    }
}
