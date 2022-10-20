using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;

namespace GameFrameworkPackageEditor
{
    public class CPostProcessBuildOnFinish
    {
        [PostProcessBuildAttribute(100)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            CBuildInfoTools.AutoIncrementInternalGameVersion();
        }
    }
}
