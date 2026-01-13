using System.Collections;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.Director.Loader
{
	/// <summary>
	///    Singleton that handles loading and unloading gameplay modules (Unity scenes) asynchronously.
	///    Supports additive/single loading, preloading (deferred activation), and module/scene callbacks.
	/// </summary>
	public static class ModuleLoader
	{
		/// <summary>
		///    Starts loading a gameplay module (scene) asynchronously.
		/// </summary>
		/// <param name="moduleToLoad">The gameplay module data that contains metadata including the scene path.</param>
		/// <param name="loadModuleMode">
		///    Mode to load the module in. Typically determines whether the scene is loaded additively or singularly,
		///    and whether activation should be deferred (preload).
		/// </param>
		/// <param name="sceneReadyEvent">
		///    Optional event invoked when a preloaded scene reaches the ready state (~90% loaded).
		///    The event receives the <see cref="AsyncOperation" /> and the <see cref="GameplayModuleData" />.
		///    External code can use this to set <see cref="AsyncOperation.allowSceneActivation" /> to true.
		///    Provided events are ignored for non-preload modes.
		/// </param>
		/// <param name="sceneActivatedEvent">
		///    Optional event invoked after the scene has been loaded and activated.
		///    The event receives the <see cref="GameplayModule" /> instance found in the loaded scene (or null if not found).
		/// </param>
		public static void LoadModule(
			GameplayModuleData moduleToLoad,
			LoadModuleMode loadModuleMode,
			UnityEvent<AsyncOperation, GameplayModuleData> sceneReadyEvent = null,
			UnityEvent<GameplayModule> sceneActivatedEvent = null)
		{
			// Validate entry conditions.
			// Module must not be null (we would have nothing to load).
			if (moduleToLoad == null)
			{
				Logger.LogError("ModuleLoader: ModuleToLoad is null!");
				return;
			}

			// ScenePath must be valid (each module should define a valid scene to load).
			string scenePath = moduleToLoad.ScenePath;
			if (string.IsNullOrEmpty(scenePath))
			{
				Logger.LogError($"ModuleLoader: ScenePath is null or empty for module '{moduleToLoad.name}'");
				return;
			}

			// Warn if sceneReadyEvent is provided but loadModuleMode is not Preload
			if (loadModuleMode != LoadModuleMode.Preload && sceneReadyEvent != null)
				Logger.LogWarning(
					"ModuleLoader: sceneReadyEvent provided but loadModuleMode is not Preload. The event will be ignored.");

			// Determine Unity's LoadSceneMode based on the requested module load mode.
			var loadMode = LoadSceneMode.Additive;

			// Begin asynchronous scene load.
			var asyncOp = SceneManager.LoadSceneAsync(scenePath, loadMode);
			if (asyncOp == null)
			{
				// If the AsyncOperation is null, the scene couldn't be requested for loading.
				Logger.LogError("AsyncOperation is null when trying to load scene: " + scenePath);
				return;
			}

			// For preload mode, prevent immediate activation so external code can prepare before activation.
			asyncOp.allowSceneActivation = loadModuleMode != LoadModuleMode.Preload;

			// Start coroutine to monitor the loading process and fire relevant events.
			GameplayDirector.Instance.StartCoroutine(WaitForSceneLoad(moduleToLoad, asyncOp, scenePath,
				loadModuleMode == LoadModuleMode.Preload,
				sceneReadyEvent,
				sceneActivatedEvent));
		}

		/// <summary>
		///    Coroutine that monitors the asynchronous scene load and triggers events at appropriate times.
		/// </summary>
		/// <param name="moduleToLoad">The module data being loaded.</param>
		/// <param name="loadSceneAsyncOperation">
		///    The async operation returned from
		///    <see cref="SceneManager.LoadSceneAsync(string, LoadSceneMode)" />.
		/// </param>
		/// <param name="scenePath">The path of the scene being loaded.</param>
		/// <param name="preload">
		///    True if the scene was requested in preload mode (activation deferred until explicitly allowed).
		/// </param>
		/// <param name="sceneReadyEvent">
		///    Event invoked when a preloaded scene reaches the ready state (~90% progress). May be null.
		/// </param>
		/// <param name="sceneActivatedEvent">
		///    Event invoked once the scene is fully loaded and activated. May be null.
		/// </param>
		/// <returns>IEnumerator for coroutine execution.</returns>
		private static IEnumerator WaitForSceneLoad(
			GameplayModuleData moduleToLoad,
			AsyncOperation loadSceneAsyncOperation,
			string scenePath,
			bool preload,
			UnityEvent<AsyncOperation, GameplayModuleData> sceneReadyEvent,
			UnityEvent<GameplayModule> sceneActivatedEvent)
		{
			// Wait until the scene loading is done.
			while (!loadSceneAsyncOperation.isDone)
			{
				// For preload mode, fire the sceneReadyEvent when loading is ~90% done.
				// Unity reports progress up to 0.9 and waits for allowSceneActivation to be true.
				if (preload && loadSceneAsyncOperation.progress >= 0.9f)
				{
					// Fire the scene ready event, allowing external code to set allowSceneActivation to true.
					sceneReadyEvent?.Invoke(loadSceneAsyncOperation, moduleToLoad);

					// Wait until allowSceneActivation is true (set externally) before continuing.
					while (!loadSceneAsyncOperation.allowSceneActivation) yield return null;
					// Exit the outer while loop to continue to scene activation handling.
					break;
				}

				yield return null;
			}

			// Scene is now loaded and activated (or was loaded directly when not preloading).
			var loadedScene = SceneManager.GetSceneByPath(scenePath);

			// Validate that the scene loaded successfully.
			if (!loadedScene.IsValid())
			{
				Logger.LogError("Scene failed to load: " + scenePath);
				yield break;
			}

			// Fire the scene activated event, providing the GameplayModule MonoBehaviour found in the scene.
			sceneActivatedEvent?.Invoke(GetModuleMonoBehaviourFromLoadedScene(loadedScene));
		}

		/// <summary>
		///    Finds a <see cref="GameplayModule" /> component on one of the root objects in the provided scene.
		/// </summary>
		/// <param name="scene">The loaded scene to search.</param>
		/// <returns>
		///    The first found <see cref="GameplayModule" /> component on any root object, or null if none was found.
		/// </returns>
		private static GameplayModule GetModuleMonoBehaviourFromLoadedScene(Scene scene)
		{
			var rootObjects = scene.GetRootGameObjects();
			foreach (var rootObject in rootObjects)
			{
				rootObject.TryGetComponent(out GameplayModule module);
				if (module) return module;
			}

			// If no GameplayModule is present on any root object, log an error to aid debugging.
			Logger.LogError(
				$"No GameplayModule found in loaded scene: {scene.name}. Make sure the scene has a GameplayModule component in one of its root objects.");
			return null;
		}
	}
}
