using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using AssemblyCSharp;


namespace RepFix
{
    public class CustomLogs
    {

    }

    public class GarbageScript : UnityEngine.MonoBehaviour
    {
        public GarbageScript()
        {

        }

        public void Start()
        {
            UnityEngine.Debug.LogException(new Exception());
        }
    }
}
