using UnityEngine;

namespace NL.XRLab.ToolkitGreybox.Utils
{
	public static class Logger
	{
		public static void Log(string message)
		{
			Debug.Log($"[Greybox] {message}");
		}

		public static void LogWarning(string message)
		{
			Debug.LogWarning($"[Greybox] {message}");
		}

		public static void LogError(string message)
		{
			Debug.LogError($"[Greybox] {message}");
		}
	}
}
