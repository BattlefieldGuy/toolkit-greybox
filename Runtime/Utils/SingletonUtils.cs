using UnityEngine;

namespace NL.XRLab.ToolkitGreybox.Utils
{
	public static class SingletonUtils
	{
		/// <summary>
		///    Ensures a single instance of a MonoBehaviour singleton exists.
		///    If multiple exist, destroys the extra ones. Sets DontDestroyOnLoad on the instance.
		/// </summary>
		/// <typeparam name="T">The type of the MonoBehaviour singleton.</typeparam>
		/// <param name="instance">Reference to the static instance field.</param>
		/// <param name="thisObject">The current MonoBehaviour instance calling this.</param>
		/// <returns>True if this instance is the singleton, false if it was destroyed.</returns>
		public static bool HandleSingletonCreation<T>(ref T instance, MonoBehaviour thisObject) where T : MonoBehaviour
		{
			if (instance != null && instance != thisObject)
			{
				Object.Destroy(thisObject.gameObject);
				return false;
			}

			instance = (T)thisObject;
			Object.DontDestroyOnLoad(thisObject.gameObject);
			return true;
		}
	}
}
