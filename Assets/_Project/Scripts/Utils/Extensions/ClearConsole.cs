using System.Reflection;
using UnityEditor;

namespace Utils
{
    public static class ClearConsole
    {
        static MethodInfo _clearConsoleMethod;
        static MethodInfo ClearConsoleMethod
        {
            get
            {
                if (_clearConsoleMethod != null) return _clearConsoleMethod;
                Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                _clearConsoleMethod = logEntries.GetMethod("Clear");
                return _clearConsoleMethod;
            }
        }

        public static void ClearLogConsole()
        {
            ClearConsoleMethod.Invoke(new object(), null);
        }
    }
}