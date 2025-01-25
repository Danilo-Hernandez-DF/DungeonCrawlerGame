using UnityEditor;
using System.Reflection;
using System;

namespace UtilsModule {
    public static class MyUtils {
		static MethodInfo _clearConsoleMethod;
		static MethodInfo ClearConsoleMethod {
			get {
				if(_clearConsoleMethod != null) return _clearConsoleMethod;
				Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
				Type logEntries = assembly.GetType ("UnityEditor.LogEntries");
				_clearConsoleMethod = logEntries.GetMethod ("Clear");
				return _clearConsoleMethod;
			}
		}

		public static void ClearLogConsole() {
			ClearConsoleMethod.Invoke (new object (), null);
		}
	}

	public enum Dir {Up, Down, Left, Right}
}