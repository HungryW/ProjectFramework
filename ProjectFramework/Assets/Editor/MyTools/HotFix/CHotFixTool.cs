using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;


class MyCustomBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("MyCustomBuildProcessor.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
    }
}
