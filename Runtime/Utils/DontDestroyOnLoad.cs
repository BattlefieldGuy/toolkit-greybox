using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	/// <summary>
	///    Utility component that marks its GameObject to persist across scene loads.
	///    Any child objects will also persist.
	/// </summary>
	[DefaultExecutionOrder(-100)]
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			MarkAsDontDestroyOnLoad(gameObject);
		}

		/// <summary>
		///    Marks the given GameObject to not be destroyed on scene load.
		///    If moveToRootIfNeeded is true, the GameObject will be moved to
		///    the root of the scene hierarchy before marking it, as Unity requires
		///    DontDestroyOnLoad objects to be at the root.
		///    Note: If a parent GameObject is already marked as DontDestroyOnLoad,
		///    the child will already persist and this action is skipped. This should be
		///    kept in mind when removing objects from the DontDestroyOnLoad scene.
		/// </summary>
		/// <param name="obj">The GameObject to mark as DontDestroyOnLoad.</param>
		/// <param name="moveToRootIfNeeded">
		///    Whether to automatically move the GameObject to the root if it has a parent. Set to false if you
		///    want to be warned about this instead.
		/// </param>
		public static void MarkAsDontDestroyOnLoad(GameObject obj, bool moveToRootIfNeeded = true)
		{
			if (moveToRootIfNeeded && obj.transform.parent != null)
				obj.transform.SetParent(null);
			Logger.Log($" Marking {obj.name} as DontDestroyOnLoad.");
			DontDestroyOnLoad(obj);
		}
	}
}
