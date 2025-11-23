using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	public class SceneLoadState
	{
		public SceneLoadState(AsyncOperation op, Task<Scene> sceneTask)
		{
			SceneLoadOperation = op;
			SceneTask = sceneTask;
		}

		public AsyncOperation SceneLoadOperation { get; }
		public Task<Scene> SceneTask { get; }
	}
}
