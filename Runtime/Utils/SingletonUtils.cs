using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	public static class SingletonUtils
	{
		/// <summary>
		///    Ensures a single instance of a MonoBehaviour singleton exists.
		///    If multiple exist, destroys the extra ones.
		/// </summary>
		/// <typeparam name="T">The type of the MonoBehaviour singleton.</typeparam>
		/// <param name="instance">Reference to the static instance field.</param>
		/// <param name="thisObject">The current MonoBehaviour instance calling this.</param>
		/// <param name="dontDestroyOnLoad">Whether to mark the singleton to persist across scene loads.</param>
		/// <returns>True if this instance is the singleton, false if it was destroyed.</returns>
		public static bool HandleSingletonCreation<T>(ref T instance, MonoBehaviour thisObject,
			bool dontDestroyOnLoad = true)
			where T : MonoBehaviour
		{
			if (instance != null && instance != thisObject)
			{
				Object.Destroy(thisObject.gameObject);
				return false;
			}

			instance = (T)thisObject;

			if (!dontDestroyOnLoad) return true;
			Scene dontDestroyOnLoadScene = SceneManager.GetSceneByName("DontDestroyOnLoad");

			// See if any parent is in DontDestroyOnLoad scene
			GameObject current = thisObject.gameObject;
			while (current.transform.parent != null)
			{
				if (dontDestroyOnLoadScene.IsValid() && current.transform.parent.gameObject.scene == dontDestroyOnLoadScene)
				{
					// A parent is already in DontDestroyOnLoad scene, no action needed
					Logger.Log(
						$"SingletonUtils: A parent of {thisObject.gameObject.name} ({current.transform.parent.gameObject.name}) already lives in the DontDestroyOnLoad scene, not marking it as DestroyOnLoad.");
					return true;
				}

				if (current.transform.parent.GetComponent<DontDestroyOnLoad>())
				{
					// A parent has DontDestroyOnLoad component, no action needed
					Logger.Log(
						$"SingletonUtils: A parent of {thisObject.gameObject.name} ({current.transform.parent.gameObject.name}) already has a DontDestroyOnLoad component, not marking it as DestroyOnLoad.");
					return true;
				}

				current = current.transform.parent.gameObject;
			}

			// See if this object is already in DontDestroyOnLoad scene
			if (dontDestroyOnLoadScene.IsValid())
			{
				GameObject[] rootObjects = dontDestroyOnLoadScene.GetRootGameObjects();
				if (rootObjects.Any(root => root == thisObject.gameObject))
				{
					Logger.LogWarning(
						$"SingletonUtils: {thisObject.gameObject.name} is already in DontDestroyOnLoad scene, not marking it as DestroyOnLoad.");
					return true;
				}
			}

			DontDestroyOnLoad.MarkAsDontDestroyOnLoad(thisObject.gameObject);

			return true;
		}
	}
}
