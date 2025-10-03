using UnityEditor;
using System.Reflection;
using System;
using UnityEngine;

namespace UtilsModule {
	public static class MyUtils
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

		public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
		{
			T component = gameObject.GetComponent<T>();
			if (!component) component = gameObject.AddComponent<T>();
			return component;
		}

		public static int LinearMapping(int currentValue, int currentMin, int currentMax, int newMin, int newMax)
		{
			int newSize = newMax - newMin;
            int oldSize = currentMax - currentMin;
            int oldScale = currentValue - currentMin;
            return (newSize * oldScale / oldSize) + newMin;
		}

		public static float LinearMapping(float currentValue, float currentMin, float currentMax, float newMin, float newMax) {
            float newSize = newMax - newMin;
            float oldSize = currentMax - currentMin;
            float oldScale = currentValue - currentMin;
            return (newSize * oldScale / oldSize) + newMin;
        }
	}

	public enum Dir {Up, Down, Left, Right}
}