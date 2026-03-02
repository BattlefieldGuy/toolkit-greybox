using System.Collections.Generic;
using NL.XRLab.Toolkit.Greybox.Director.Loader;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using NL.XRLab.Toolkit.Greybox.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.Director
{
	/// <summary>
	///    Manages the lifecycle of gameplay modules in the application. Acts as a singleton
	///    responsible for loading, activating, and tracking gameplay modules.
	/// </summary>
	public class GameplayDirector : MonoBehaviour
	{
		/// <summary>
		///    Internal backing field for the singleton instance.
		/// </summary>
		private static GameplayDirector _instance;

		/// <summary>
		///    List of all gameplay modules available in the game. Configurable in the Unity Inspector.
		/// </summary>
		[Tooltip("All gameplay modules in the game. Non-sequential.")] [SerializeField]
		private List<GameplayModuleData> _gameplayModules = new();

		[SerializeField] private GameplayModuleDataEntry _startingModule;

		/// <summary>
		///    Event triggered when a module finishes preloading (~90% loaded).
		/// </summary>
		public UnityEvent<GameplayModuleData> OnModuleReady = new();

		/// <summary>
		///    Event triggered when a module is fully activated in the scene.
		/// </summary>
		public UnityEvent<GameplayModule> OnModuleActivated = new();

		/// <summary>
		///    Dictionary to track active modules. Maps GameplayModuleData to GameplayModule instances.
		/// </summary>
		private readonly Dictionary<GameplayModuleData, GameplayModule> _activeModules = new();

		/// <summary>
		///    Dictionary to track preloaded modules. Maps GameplayModuleData to AsyncOperation instances.
		/// </summary>
		private readonly Dictionary<GameplayModuleData, AsyncOperation> _readyModules = new();

		/// <summary>
		///    Read-only property to access the StartingModule.
		/// </summary>
		[Tooltip("The gameplay module that should be loaded upon game start.")]
		public GameplayModuleDataEntry StartingModule => _startingModule;

		/// <summary>
		///    Read-only property to access the list of all gameplay modules.
		/// </summary>
		public List<GameplayModuleData> GameplayModules => _gameplayModules;

		/// <summary>
		///    Singleton instance of the Director.
		/// </summary>
		public static GameplayDirector Instance => _instance;

		public void Awake()
		{
			SingletonUtils.HandleSingletonCreation(ref _instance, this);
		}

		/// <summary>
		///    Unity Start lifecycle method. Logs startup and loads the StartingModule if configured.
		/// </summary>
		public void Start()
		{
			Logger.Log("Director starting up...");
			LoadStartingModule();
		}

		/// <summary>
		///    Loads the StartingModule specified in the Unity Inspector.
		///    Validates the configuration before initiating the load process.
		/// </summary>
		private void LoadStartingModule()
		{
			if (StartingModule == null)
			{
				Logger.LogError("StartingModule is not set in the Director.");
				return;
			}

			Logger.Log($"Loading Starting Module: '{StartingModule.GameplayModuleData.name}'");
			LoadModule(StartingModule.GameplayModuleData, StartingModule.LoadModuleMode);
		}

		/// <summary>
		///    Initiates the loading of a gameplay module. Configures callbacks to track module readiness and activation.
		/// </summary>
		/// <param name="moduleToLoad">The GameplayModuleData to load.</param>
		/// <param name="loadModuleMode">Specifies the loading mode (Single, Additive, Preload).</param>
		/// <param name="sceneReadyEvent">Optional event triggered when the module is preloaded.</param>
		/// <param name="sceneActivatedEvent">Optional event triggered when the module is activated.</param>
		public void LoadModule(
			GameplayModuleData moduleToLoad,
			LoadModuleMode loadModuleMode,
			UnityEvent<AsyncOperation, GameplayModuleData> sceneReadyEvent = null,
			UnityEvent<GameplayModule> sceneActivatedEvent = null
		)
		{
			// Add callback listeners to track loaded modules.
			sceneReadyEvent ??= new UnityEvent<AsyncOperation, GameplayModuleData>();
			sceneReadyEvent.AddListener(AddReadyModule);
			sceneActivatedEvent ??= new UnityEvent<GameplayModule>();
			sceneActivatedEvent.AddListener(AddActiveModule);

			// Log and delegate the actual loading to the ModuleLoader.
			Logger.Log($"Loading Module: '{moduleToLoad.name}'");
			ModuleLoader.LoadModule(
				moduleToLoad,
				loadModuleMode,
				sceneReadyEvent,
				sceneActivatedEvent
			);
		}

		/// <summary>
		///    Callback to track preloaded modules. Adds the module to the ready dictionary and triggers the OnModuleReady event.
		/// </summary>
		/// <param name="loadOperation">The AsyncOperation representing the preload process.</param>
		/// <param name="gameplayModuleData">The GameplayModuleData associated with the module.</param>
		private void AddReadyModule(
			AsyncOperation loadOperation,
			GameplayModuleData gameplayModuleData
		)
		{
			if (!_readyModules.TryAdd(gameplayModuleData, loadOperation))
			{
				Logger.LogError(
					$"Module '{gameplayModuleData.name}' is already loaded. This should not happen."
				);
				return;
			}

			Logger.Log($"Ready module added: '{gameplayModuleData.name}'");
			OnModuleReady.Invoke(gameplayModuleData);
		}

		/// <summary>
		///    Callback to track activated modules. Adds the module to the active dictionary and triggers the OnModuleActivated
		///    event.
		/// </summary>
		/// <param name="loadedModule">The GameplayModule instance representing the activated module.</param>
		private void AddActiveModule(GameplayModule loadedModule)
		{
			if (!_activeModules.TryAdd(loadedModule.GameplayModuleData, loadedModule))
			{
				Logger.LogError(
					$"Module '{loadedModule.GameplayModuleData.name}' is already active. This should not happen."
				);
				return;
			}

			loadedModule.OnModuleCompleted.AddListener(LoadConnectedModules);

			Logger.Log($"Active module added: '{loadedModule.GameplayModuleData.name}'");
			OnModuleActivated.Invoke(loadedModule);
		}

		/// <summary>
		///    Loads modules that are connected to the given module.
		/// </summary>
		/// <param name="module">The module which provides the connected modules.</param>
		private void LoadConnectedModules(GameplayModule module)
		{
			foreach (
				GameplayModuleDataEntry gameplayModuleDataEntry in module
					.GameplayModuleData
					.ConnectedModules
			)
				LoadModule(
					gameplayModuleDataEntry.GameplayModuleData,
					gameplayModuleDataEntry.LoadModuleMode
				);

			if (module.GameplayModuleData.UnloadSceneOnCompletion)
				SceneManager.UnloadSceneAsync(module.GameplayModuleData.ScenePath);
		}
	}
}
