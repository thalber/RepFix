using System;
using System.IO;


namespace WaspPile.RepFix
{
    public class CustomLogs
    {

    }

    public static class EnumExt_RF
    {
        public static MouseAI.Behavior FollowFriend;
    }

    public class GarbageScript : UnityEngine.MonoBehaviour
    {
        public GarbageScript()
        {
            UnityEngine.Debug.Log("GARBAGE SCRIPT UP");
        }

        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A) && cd <= 0)
            {
                RainWorld rw = UnityEngine.Object.FindObjectOfType<RainWorld>();
                
                
                if (rw != null)
                {
                    byte[] btw = rw.maze.EncodeToJPG();
                    File.WriteAllBytes(@"C:\users\thalber\documents\maze.jpg", btw);
                    cd = 300;
                    UnityEngine.Debug.Log($"CD: {cd}");
                }
            }
            if (cd > 0) cd--;
        }

        private int cd = 0;

        public void Start()
        {

        }
    }
}
