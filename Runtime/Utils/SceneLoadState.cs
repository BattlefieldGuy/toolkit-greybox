using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	/// <summary>
	/// The current state of a scene load operation.
	/// Wrapper for the AsyncOperation returned by SceneManager.LoadSceneAsync and the Task<Scene> returned by the LoadSceneAsync extension method.
	/// </summary>
	public class SceneLoadState
	{
		public SceneLoadState(AsyncOperation op, Task<Scene> sceneTask)
		{
			SceneLoadOperation = op;
			SceneTask = sceneTask;
		}

		/// <summary>
		/// The AsyncOperation returned by SceneManager.LoadSceneAsync.
		/// This can be used to check the progress of the scene load operation and to determine when it is complete.
		/// </summary>
		public AsyncOperation SceneLoadOperation { get; }

		/// <summary>
		/// The Task<Scene> returned by the LoadSceneAsync extension method.
		/// This can be awaited to get the loaded Scene once the load operation is complete, or to catch any exceptions that occur during loading.
		/// </summary>
		public Task<Scene> SceneTask { get; }
	}
}
