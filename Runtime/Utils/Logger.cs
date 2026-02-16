using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	/// <summary>
	/// Wrapper class for Unity's Debug class to provide consistent logging with a "[Greybox]" prefix for all log messages.
	/// </summary>
	public static class Logger
	{
		/// <summary>
		/// Logs a message to the Unity console with a "[Greybox]" prefix.
		/// </summary>
		/// <param name="message">Rest of the message.</param>
		public static void Log(string message)
		{
			Debug.Log($"[Greybox] {message}");
		}

		/// <summary>
		/// Logs a warning message to the Unity console with a "[Greybox]" prefix.
		/// </summary>
		/// <param name="message">Rest of the message.</param>
		public static void LogWarning(string message)
		{
			Debug.LogWarning($"[Greybox] {message}");
		}

		/// <summary>
		/// Logs an error message to the Unity console with a "[Greybox]" prefix.
		/// </summary>
		/// <param name="message">Rest of the message.</param>
		public static void LogError(string message)
		{
			Debug.LogError($"[Greybox] {message}");
		}
	}
}
